

using LDM.Core;
using LDM.Utility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;
using Dapper;


namespace LDM.Data
{
    public class User : IUser
    {
        private readonly AppSettingsModels _appSettingsModels;
        public User()
        {
            _appSettingsModels = new AppSettingsModels();
            _appSettingsModels.ConnectionString = EnvVeriables.ConnectionString;
        }
        public User(IOptions<AppSettingsModels> appSettingsModels)
        {
            _appSettingsModels = appSettingsModels.Value;          
        }

        public async Task<bool> CheckUserAuth(string UserCode)
        {
            bool isSuccess = false;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oUserCode", UserCode);
                    var reader = conn.QueryMultiple("LDM_SP_CheckUserCategoriesForAuth", parameters, commandType: CommandType.StoredProcedure);
                    var user = reader.Read<string>().FirstOrDefault();
                    if (!string.IsNullOrEmpty(user))
                    {
                        isSuccess = true;
                    }
                }
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return isSuccess;
        }

        public async Task<List<CategoryModels>> GetUserCategories(UserCategoriesModels model)
        {
            List<CategoryModels> categories = null;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oUserCode", model.UserCode);
                    var reader = conn.QueryMultiple("LDM_SP_GetUserCategories", parameters, commandType: CommandType.StoredProcedure);
                    categories = reader.Read<CategoryModels>().ToList();
                }
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return categories;
        }

        public  async Task<UserModels> GetUserDetails(UserDetailsModels userModel)
        {
            UserModels user = null;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oUserCode", userModel.UserCode);
                    var reader = conn.QueryMultiple("LDM_SP_GetUserDetails", parameters, commandType: CommandType.StoredProcedure);
                    user = reader.Read<UserModels>().FirstOrDefault();
                }
                await Task.CompletedTask;
            }
            catch 
            {
                throw;
            }
            return user;
        }
        public async Task<UserAllDetailsModels> GetUserAlldetails(string UserCode)
        {
            UserAllDetailsModels userDetails = null;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oUserCode", UserCode);
                    var reader = conn.QueryMultiple("LDM_SP_GetUserAllDetails", parameters, commandType: CommandType.StoredProcedure);
                    userDetails = reader.Read<UserAllDetailsModels>().FirstOrDefault();                   
                }
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return userDetails;
        }
        public async Task<List<EmailConfigModels>> GetEmailConfiguration()
        {
            List<EmailConfigModels> configModels = null;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();                  
                    var reader = conn.QueryMultiple("LDM_SP_GetEmailConfiguration", parameters, commandType: CommandType.StoredProcedure);
                    configModels = reader.Read<EmailConfigModels>().ToList();                   
                }
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return configModels;
        }
    }
}
