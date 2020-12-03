using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LDM.Core;
using LDM.Utility;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace LDM.Data
{
    public class Upload : IUpload
    {
        private readonly AppSettingsModels _appSettingsModels;
        public Upload()
        {
            _appSettingsModels = new AppSettingsModels();
            _appSettingsModels.ConnectionString = EnvVeriables.ConnectionString;
        }        
        public async Task<bool> AddDocumentDetails(string itemId, string userCode, string tableName, string columnNames, string columnValues)
        {
            bool IsSuccess = false;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oITEM_ID", itemId);
                    parameters.Add("@oUserCode", userCode);
                    parameters.Add("@oTableName", tableName);
                    parameters.Add("@oColumnNames", columnNames);
                    parameters.Add("@oColumnValues", columnValues);
                    int success=conn.Execute("LDM_SP_AddDocumentMetaDetails", parameters, commandType: CommandType.StoredProcedure);
                    if (success == 0)
                    {
                        IsSuccess = true;
                    }
                }
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return IsSuccess;
        }

        public async Task<bool> AddDocumentMapping(string categoryCode, string userCode, string itemId, string fileName)
        {
            bool IsSuccess = false;
            try
            {
                DynamicParameters parameters = null;
                CategoryDetailsModels categoryDetails = await GetCategoryDetails(categoryCode);
                CategoryFileMappingDetails categoryFileMapping = await GetCategoryFolderNames(itemId, categoryDetails);
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oCategoryCode", categoryCode);
                    parameters.Add("@oMainFolder", categoryFileMapping.MainFolder);
                    parameters.Add("@oITEM_ID", categoryFileMapping.SubFolder);
                    parameters.Add("@oFileName", fileName);
                    parameters.Add("@oUserCode", userCode);
                    int success=conn.Execute("LDM_SP_AddDocument", parameters, commandType: CommandType.StoredProcedure);
                    if (success==1)
                    {
                        IsSuccess = true;
                    }
                }
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return IsSuccess;
        }

        public async Task<List<CategoryColumnModels>> GetCategoryColumns(string categoryCode)
        {
            List<CategoryColumnModels> categoryColumns = null;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oCategoryCode", categoryCode);
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

        public async Task<CategoryDetailsModels> GetCategoryDetails(string categoryCode)
        {
            CategoryDetailsModels categoryDetails = null;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oCategoryCode", categoryCode);
                    var reader = conn.QueryMultiple("LDM_SP_GetCategoryDetails", parameters, commandType: CommandType.StoredProcedure);
                    categoryDetails = reader.Read<CategoryDetailsModels>().FirstOrDefault();
                }
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return categoryDetails;
        }

        public async Task<CategoryFileMappingDetails> GetCategoryFolderNames(string ITEM_ID, CategoryDetailsModels categoryFolderDetails)
        {
            CategoryFileMappingDetails categoryFolderNames = null;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oITEM_ID", ITEM_ID);
                    parameters.Add("@oMain_Folder_Column", categoryFolderDetails.Main_Folder_Column);
                    parameters.Add("@oFolder_Column", categoryFolderDetails.Folder_Column);
                    parameters.Add("@oTableName", categoryFolderDetails.TableName);
                    var reader = conn.QueryMultiple("LDM_SP_GetCategoryItemIdFolderNames", parameters, commandType: CommandType.StoredProcedure);
                    categoryFolderNames = reader.Read<CategoryFileMappingDetails>().FirstOrDefault();
                }
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return categoryFolderNames;
        }

        public async Task<bool> UpdateDocumentSuccess(UploadSuccessModels model, CategoryDetailsModels categoryDetails)
        {
            bool isSuccess = false;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oITEM_ID", model.ITEM_ID);
                    parameters.Add("@oTableName", categoryDetails.TableName);
                    int success=conn.Execute("LDM_SP_UpdateDocumentMetaStatus", parameters, commandType: CommandType.StoredProcedure);
                    if (success == 0)
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

        public async Task<List<CategoryFileNameModels>> GetDownloadDocuments(string ItemId, string categoryCode)
        {
            List<CategoryFileNameModels> categoryFileNames = null;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oITEM_ID", ItemId);
                    parameters.Add("@oCategoryCode", categoryCode);
                    var reader = conn.QueryMultiple("LDM_SP_GetCategoryItemIdFileNames", parameters, commandType: CommandType.StoredProcedure);
                    categoryFileNames = reader.Read<CategoryFileNameModels>().ToList();
                }
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return categoryFileNames;
        }

        public async Task<List<CategoryFileNameModels>> GetDownloadDocumentsByDocumentId(string docId, string categoryCode)
        {
            List<CategoryFileNameModels> categoryFileNames = null;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oDocument_Id", docId);
                    parameters.Add("@oCategoryCode", categoryCode);
                    var reader = conn.QueryMultiple("LDM_SP_GetCategoryDocumentIdFileNames", parameters, commandType: CommandType.StoredProcedure);
                    categoryFileNames = reader.Read<CategoryFileNameModels>().ToList();
                }
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return categoryFileNames;
        }

        public async Task<CategoryFileMappingDetails> GetCategoryFolderNamesByDocumentId(string docId, CategoryDetailsModels categoryFolderDetails)
        {
            CategoryFileMappingDetails categoryFolderNames = null;
            try
            {
                DynamicParameters parameters = null;
                using (var conn = new MySqlConnection(_appSettingsModels.ConnectionString))
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@oDocument_Id", docId);
                    parameters.Add("@oMain_Folder_Column", categoryFolderDetails.Main_Folder_Column);
                    parameters.Add("@oFolder_Column", categoryFolderDetails.Folder_Column);
                    parameters.Add("@oTableName", categoryFolderDetails.TableName);
                    var reader = conn.QueryMultiple("LDM_SP_GetCategoryDocumentIdFolderNames", parameters, commandType: CommandType.StoredProcedure);
                    categoryFolderNames = reader.Read<CategoryFileMappingDetails>().FirstOrDefault();
                }
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return categoryFolderNames;
        }
    }
}
