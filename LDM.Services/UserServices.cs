using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LDM.Core;
using LDM.Data;
using Microsoft.Extensions.Logging;

namespace LDM.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUser _user;       
        public UserServices()
        {
            _user = new User();
        }      
        public async Task<UserModels> GetUserDetails(UserDetailsModels userModel)
        {
            try
            {               
                return await _user.GetUserDetails(userModel);
            }
            catch 
            {
                throw;
            }          
        }
        public async Task<List<CategoryModels>> GetUserCategories(UserCategoriesModels model)
        {
            List<CategoryModels> categories = null;
            try
            {
                categories = await _user.GetUserCategories(model);
                return categories;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> CheckUserAuth(string UserCode)
        {
             bool isSuccess = false;
            try
            {
                isSuccess = await _user.CheckUserAuth(UserCode);
                return isSuccess;
            }
            catch
            {
                throw;
            }
        }
    }
}
