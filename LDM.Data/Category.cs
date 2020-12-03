using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Dapper;
using LDM.Core;
using LDM.Utility;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace LDM.Data
{
    public class Category : ICategory
    {

        private readonly AppSettingsModels _appSettingsModels;
        public Category()
        {
            _appSettingsModels = new AppSettingsModels();
            _appSettingsModels.ConnectionString = EnvVeriables.ConnectionString;
        }       
        public async Task<List<CategoryColumnModels>> GetCategoryParams(string CategoryCode)
        {
            List<CategoryColumnModels> categoryColumns = null;
            try
            {
                LambdaLogger.Log("\nConnection String: " + _appSettingsModels.ConnectionString+"\n");
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oCategoryCode", CategoryCode);
                    var reader = conn.QueryMultiple("LDM_SP_GetCategoryColumns", parameters, commandType: CommandType.StoredProcedure);
                    categoryColumns = reader.Read<CategoryColumnModels>().ToList();
                }
                    await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return categoryColumns;
        }
    }
}
