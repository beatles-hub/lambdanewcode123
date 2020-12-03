using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LDM.Core;
using LDM.Utility;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace LDM.Data
{
    public class Search : ISearch
    {
        private readonly AppSettingsModels _appSettingsModels;
        public Search()
        {
            _appSettingsModels = new AppSettingsModels();
            _appSettingsModels.ConnectionString = EnvVeriables.ConnectionString;
        }
        public async Task<List<SearchParametersDisplay>> GetCategoryHeaders(string categoryCode)
        {
            List<SearchParametersDisplay> searchParameters = null;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oCategoryCode", categoryCode);
                    var reader = conn.QueryMultiple("LDM_SP_GetCategoryResultHeaders", parameters, commandType: CommandType.StoredProcedure);
                    searchParameters = reader.Read<SearchParametersDisplay>().ToList();
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return searchParameters;
           
        }
        public async Task<List<dynamic>> GetSearchResults(CategoryDetailsModels categoryDetails, string categorySelectColumns, string categoryWhereClause)
        {
            List<dynamic> metaDatas = null;
            try
            {
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@oCategoryTableName", categoryDetails.TableName);
                    parameters.Add("@oSelectColumnns", categorySelectColumns);
                    parameters.Add("@oWhereClause", categoryWhereClause);
                    var reader = conn.QueryMultiple("LDM_SP_GetSearchResult", parameters, commandType: CommandType.StoredProcedure);
                    metaDatas = reader.Read().ToList();
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return metaDatas;
        }
        public async Task<List<SearchOpeators>> GetSearchOperators(SearchOperatorsModels model)
        {
            List<SearchOpeators> searchOperators = null;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    var reader = conn.QueryMultiple("LDM_SP_GetColumnTypeOperators", parameters, commandType: CommandType.StoredProcedure);
                    searchOperators = reader.Read<SearchOpeators>().ToList();
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return searchOperators;
        }
        public async Task<List<RiskTechFileModels>> GetFilesForRiskTech(string resultItemID, string categoryCode)
        {
            List<RiskTechFileModels> resultFilesForRiskTech = new List<RiskTechFileModels>();
            try
            {
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@oITEM_ID", resultItemID);
                    parameters.Add("@oCategoryCode", categoryCode);
                    var reader = conn.QueryMultiple("LDM_SP_GetItemIdFiles", parameters, commandType: CommandType.StoredProcedure);
                    resultFilesForRiskTech = reader.Read<RiskTechFileModels>().ToList();
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultFilesForRiskTech;
        }
    }
}
