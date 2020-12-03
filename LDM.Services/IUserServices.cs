using LDM.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Services
{
    public interface IUserServices
    {
        Task<List<CategoryModels>> GetUserCategories(UserCategoriesModels model);
        Task<UserModels> GetUserDetails(UserDetailsModels userModel);
        Task<bool> CheckUserAuth(string UserCode);      
    }
}
