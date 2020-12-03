using LDM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Data
{
    public interface ICategory
    {       
        Task<List<CategoryColumnModels>> GetCategoryParams(string CategoryCode);
    }
}
