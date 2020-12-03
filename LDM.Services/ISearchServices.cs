using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LDM.Core;

namespace LDM.Services
{
    public interface ISearchServices
    {
        Task<dynamic> GetSearchResults(SearchResultsModels model);
        Task<SearchOpeatorResultModels> GetSearchOperators(SearchOperatorsModels model);
    }
}
