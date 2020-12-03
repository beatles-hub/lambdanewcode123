using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LDM.Core;
using LDM.Data;
using LDM.Utility;
using Microsoft.Extensions.Logging;

namespace LDM.Services
{
    public class SearchServices : ISearchServices
    {
        private readonly ISearch _search;
        private readonly IUpload _upload;       
        public SearchServices()
        {
            _search = new Search();
            _upload = new Upload();           
        }
        public async Task<dynamic> GetSearchResults(SearchResultsModels model)
        {
            dynamic searchResult = null;
            try
            {
                List<SearchParametersDisplay> searchParametersDisplays = await GetCategoryHeaders(model);
                if (searchParametersDisplays.Count > 0)
                {
                    string categoryWhereClause = await GetWhereClauseDetails(searchParametersDisplays, model);
                    string categorySelectColumns = await GetCategorySelectColumns(searchParametersDisplays);
                    CategoryDetailsModels categoryDetails = await _upload.GetCategoryDetails(model.CategoryCode);
                    List<dynamic> resultList = null;
                    if (!string.IsNullOrEmpty(categorySelectColumns) && !string.IsNullOrEmpty(categoryWhereClause) && categoryDetails != null)
                    {
                        resultList = await _search.GetSearchResults(categoryDetails, categorySelectColumns, categoryWhereClause);
                    }

                    if (model.UIRequest)
                    {
                        MetaDataModels metaDatas = new MetaDataModels();
                        metaDatas.Headers = await GetCategoryHeaders(model);
                        metaDatas.ResultLists = resultList;
                        metaDatas.TotalRows = resultList.Count;
                        searchResult = metaDatas;
                    }
                    else
                    {
                        MetaDataRiskTechModels metaDatas = new MetaDataRiskTechModels();
                        metaDatas.ResultLists = resultList;
                        metaDatas.ResultLists.ForEach(result => {
                            result.Files = _search.GetFilesForRiskTech(result.ITEM_ID, model.CategoryCode);
                        });
                        metaDatas.TotalRows = resultList.Count;
                        searchResult = metaDatas;
                    }
                }

                return searchResult;
            }
            catch
            {
                throw;
            }
        }
        public async Task<SearchOpeatorResultModels> GetSearchOperators(SearchOperatorsModels model)
        {          
            SearchOpeatorResultModels searchOpeatorResult = null;
            try
            {
                List<SearchOpeators>  searchOpeators = await _search.GetSearchOperators(model);
                searchOpeatorResult = new SearchOpeatorResultModels();
                searchOpeatorResult.DateOperators = searchOpeators.Where(x => x.ColumnType == Constants.DateOperator).ToList();
                searchOpeatorResult.TextOperators = searchOpeators.Where(x => x.ColumnType == Constants.TextOperator).ToList();
                searchOpeatorResult.NumberOperators = searchOpeators.Where(x => x.ColumnType == Constants.NumberOperator).ToList();
                return searchOpeatorResult;
            }
            catch
            {
                throw;
            }
        }

        #region Private Functions
        private async Task<string> GetWhereClauseDetails(List<SearchParametersDisplay> headers, SearchResultsModels model)
        {
            string whereString = string.Empty;
            string Condition = string.Empty;
            try
            {
                if (model.Condition == Constants.ConditionOr)
                    Condition = Constants.ConditionOr;
                else
                    Condition = Constants.CondistionAnd;
                var SearchFields = model.SearchFields.Where(x => x.Field != Constants.ItemIdField
                             && !string.IsNullOrEmpty(x.Value) && !string.IsNullOrEmpty(x.Operator)).ToList();
                foreach (var searchField in SearchFields)
                {
                    var searchData = searchField;
                    var header = headers.Where(x => x.Field == searchData.Field).FirstOrDefault();
                    if (searchData != null)
                    {
                        string fieldValue = searchData.Value;
                        bool isDate = false;
                        bool isDateCorrectFormat = true;
                        if (header.Field.Contains(Constants.DateField) || header.Field.Contains(Constants.TimeStampField))
                        {
                            List<string> inItems = fieldValue.Split(',').ToList();
                            foreach (var item in inItems)
                            {
                                if (!Constants.CheckDateFormat(item))
                                {
                                    isDateCorrectFormat = false;
                                }
                            }
                            if (isDateCorrectFormat)
                            {
                                fieldValue = Constants.ConvertStringToDate(fieldValue);
                                isDate = true;
                            }
                        }
                        if (isDateCorrectFormat)
                        {
                            switch (searchData.Operator)
                            {
                                case Constants.EQUALTO:
                                    {
                                        whereString = (string.IsNullOrEmpty(whereString) ? "" : whereString + " " + Condition + " ")
                                            + header.Field + " " + searchData.Operator;
                                        if (isDate)
                                            whereString += fieldValue;
                                        else
                                            whereString += " '" + fieldValue + "'";
                                        break;
                                    }
                                case Constants.NOTEQUALTO:
                                    {
                                        whereString = (string.IsNullOrEmpty(whereString) ? "" : whereString + " " + Condition + " ")
                                            + header.Field + " " + searchData.Operator;

                                        if (isDate)
                                            whereString += fieldValue;
                                        else
                                            whereString += " '" + fieldValue + "'";
                                        break;
                                    }
                                case Constants.LIKE:
                                    {

                                        string value = fieldValue;
                                        if (fieldValue.StartsWith("%"))
                                        {
                                            value = " '" + fieldValue + "%'";
                                        }
                                        else if (fieldValue.EndsWith("%"))
                                        {
                                            value = " '%" + fieldValue + "'";
                                        }
                                        else {
                                            value = " '%" + fieldValue + "%'";
                                        }
                                        whereString = (string.IsNullOrEmpty(whereString) ? "" : whereString + " " + Condition + " ")
                                           + header.Field + " " + searchData.Operator + value;
                                        break;
                                    }
                                case Constants.NOTLIKE:
                                    {
                                        string value = fieldValue;
                                        if (fieldValue.StartsWith("%"))
                                        {
                                            value = " '" + fieldValue + "%'";
                                        }
                                        else if (fieldValue.EndsWith("%"))
                                        {
                                            value = " '%" + fieldValue + "'";
                                        }
                                        else
                                        {
                                            value = " '%" + fieldValue + "%'";
                                        }
                                        whereString = (string.IsNullOrEmpty(whereString) ? "" : whereString + " " + Condition + " ")
                                           + header.Field + " " + searchData.Operator + value;
                                        break;
                                    }
                                case Constants.IN:
                                    {
                                        List<string> inItems = searchData.Value.Split(',').ToList();
                                        string inValues = string.Empty;
                                        if (isDate)
                                        {
                                            foreach (var item in inItems)
                                            {
                                                inValues = (string.IsNullOrEmpty(inValues)) ? " (" : (inValues + " " + Constants.ConditionOr + " ");
                                                inValues += header.Field + " " + Constants.EQUALTO + Constants.ConvertStringToDate(item);
                                            }
                                            whereString = inValues + ")";
                                        }
                                        else
                                        {
                                            foreach (var item in inItems)
                                            {
                                                inValues = (string.IsNullOrEmpty(inValues)) ? "'" + item + "'" : (inValues + "," + "'" + item + "'");
                                            }
                                            whereString = (string.IsNullOrEmpty(whereString)) ? "" : (whereString + " " + Condition + " ");
                                            whereString += header.Field + " " + searchData.Operator + " (" + inValues + ")";
                                        }
                                        break;
                                    }
                                case Constants.NOTIN:
                                    {

                                        List<string> inItems = searchData.Value.Split(',').ToList();
                                        string inValues = string.Empty;
                                        if (isDate)
                                        {
                                            foreach (var item in inItems)
                                            {
                                                inValues = (string.IsNullOrEmpty(inValues)) ? " (" : (inValues + " " + Constants.ConditionOr + " ");
                                                inValues += header.Field + " " + Constants.NOTEQUALTO + Constants.ConvertStringToDate(item);
                                            }
                                            whereString = inValues + ")";
                                        }
                                        else
                                        {
                                            foreach (var item in inItems)
                                            {
                                                inValues = (string.IsNullOrEmpty(inValues)) ? "'" + item + "'" : (inValues + "," + "'" + item + "'");
                                            }
                                            whereString = (string.IsNullOrEmpty(whereString)) ? "" : (whereString + " " + Condition + " ");
                                            whereString += header.Field + " " + searchData.Operator + " (" + inValues + ")";
                                        }
                                        break;
                                    }
                                case Constants.LESSTHAN:
                                    {
                                        whereString = (string.IsNullOrEmpty(whereString) ? "" : whereString + " " + Condition + " ")
                                            + header.Field + " " + searchData.Operator;
                                        if (isDate)
                                            whereString += fieldValue;
                                        else
                                            whereString += " '" + fieldValue + "'";
                                        break;
                                    }
                                case Constants.LESSTHANEQUALTO:
                                    {
                                        whereString = (string.IsNullOrEmpty(whereString) ? "" : whereString + " " + Condition + " ")
                                            + header.Field + " " + searchData.Operator;
                                        if (isDate)
                                            whereString += fieldValue;
                                        else
                                            whereString += " '" + fieldValue + "'";
                                        break;
                                    }
                                case Constants.GREATERTHAN:
                                    {
                                        whereString = (string.IsNullOrEmpty(whereString) ? "" : whereString + " " + Condition + " ")
                                            + header.Field + " " + searchData.Operator;
                                        if (isDate)
                                            whereString += fieldValue;
                                        else
                                            whereString += " '" + fieldValue + "'";
                                        break;
                                    }
                                case Constants.GREATERTHANEQUALTO:
                                    {
                                        whereString = (string.IsNullOrEmpty(whereString) ? "" : whereString + " " + Condition + " ")
                                            + header.Field + " " + searchData.Operator;
                                        if (isDate)
                                            whereString += fieldValue;
                                        else
                                            whereString += " '" + fieldValue + "'";
                                        break;
                                    }
                                case Constants.BETWEEN:
                                    {
                                        List<string> inItems = searchData.Value.Split(',').ToList();
                                        string inValues = string.Empty;
                                        if (inItems.Count == 2)
                                        {
                                            inValues = Constants.ConvertStringToDate(inItems[0]) + " AND " + Constants.ConvertStringToDate(inItems[1]);
                                            whereString = (string.IsNullOrEmpty(whereString)) ? "" : (whereString + " " + Condition + " ");
                                            whereString += header.Field + " " + searchData.Operator + " " + inValues;
                                        }
                                        break;
                                    }
                            }
                        }

                    }
                }
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return whereString;
        }
        private async Task<string> GetCategorySelectColumns(List<SearchParametersDisplay> headers)
        {
            string selectedColumns = string.Empty;
            try
            {
                List<SearchParametersDisplay> listHeader = new List<SearchParametersDisplay>();
                SearchParametersDisplay header = null;
                foreach (var item in headers)
                {
                    header = new SearchParametersDisplay();
                    header = item;
                    if (item.Field.Contains(Constants.DateField) || item.Field.Contains(Constants.TimeStampField))
                    {
                        header.Field = Constants.ConvertDateToString(item.Field);
                    }
                    listHeader.Add(header);
                }
                selectedColumns = string.Join(",", headers.Where(p => p.Field != Constants.ItemIdField)
                                 .Select(p => p.Field.ToString()));

                await Task.CompletedTask;
               
            }
            catch
            {
                throw;
            }
            return selectedColumns;
        }
        private async Task<List<SearchParametersDisplay>> GetCategoryHeaders(SearchResultsModels model)
        {
            List<SearchParametersDisplay> searchParameters = null;
            try
            {
                searchParameters = await _search.GetCategoryHeaders(model.CategoryCode);
                if (searchParameters.Count>0)
                {
                    searchParameters.Insert(0, new SearchParametersDisplay()
                    {
                        Header = Constants.ItemIdHeader,
                        Field = Constants.ItemIdField,
                        Hidden = true
                    });
                }  
            }
            catch
            {
                throw;
            }
            return searchParameters;
        }
        #endregion
    }
}
