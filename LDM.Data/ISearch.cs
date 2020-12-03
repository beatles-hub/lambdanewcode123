using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LDM.Core;

namespace LDM.Data
{
    public interface ISearch
    {
        Task<List<dynamic>> GetSearchResults(CategoryDetailsModels categoryDetails,string categorySelectColumns,string categoryWhereClause);
        Task<List<SearchOpeators>> GetSearchOperators(SearchOperatorsModels model);
        Task<List<SearchParametersDisplay>> GetCategoryHeaders(string categoryCode);
        Task<List<RiskTechFileModels>> GetFilesForRiskTech(string resultItemID, string categoryCode);
    }
}
