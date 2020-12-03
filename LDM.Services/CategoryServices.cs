using LDM.Core;
using LDM.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Services
{
    public class CategoryServices:ICategoryServices
    {
        private readonly ICategory _category;      
        public CategoryServices()
        {
            _category = new Category();
        }      
        public async Task<List<CategoryColumnModels>> GetCategoryParams(string CategoryCode)
        {
            List<CategoryColumnModels> categoryColumns = null;
            try
            {
                categoryColumns = await _category.GetCategoryParams(CategoryCode);
            }
            catch
            {
                throw;
            }
            return categoryColumns;
        }
    }
}
