using LDM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Services
{
    public interface ICategoryServices
    {
        Task<List<CategoryColumnModels>> GetCategoryParams(string UserCode);
    }
}
