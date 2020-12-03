using LDM.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Data
{
    public interface IUser
    {
        Task<UserModels> GetUserDetails(UserDetailsModels userModel);
        Task<List<CategoryModels>> GetUserCategories(UserCategoriesModels model);
        Task<bool> CheckUserAuth(string UserCode);
        Task<UserAllDetailsModels> GetUserAlldetails(string UserCode);
        Task<List<EmailConfigModels>> GetEmailConfiguration();
    }
}
