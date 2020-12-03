using LDM.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Data
{
    public interface IUpload
    {       
        Task<List<CategoryColumnModels>> GetCategoryColumns(string categoryCode);
        Task<CategoryDetailsModels> GetCategoryDetails(string categoryCode);
        Task<bool> AddDocumentDetails(string itemId, string userCode, string tableName, string columnNames, string columnValues);
        Task<bool> AddDocumentMapping(string categoryCode, string userCode, string itemId, string fileName);
        Task<CategoryFileMappingDetails> GetCategoryFolderNames(string ITEM_ID, CategoryDetailsModels categoryFolderDetails);
        Task<bool> UpdateDocumentSuccess(UploadSuccessModels model, CategoryDetailsModels categoryDetails);
        Task<List<CategoryFileNameModels>> GetDownloadDocuments(string ItemId,string categoryCode);
        Task<List<CategoryFileNameModels>> GetDownloadDocumentsByDocumentId(string docId, string categoryCode);
        Task<CategoryFileMappingDetails> GetCategoryFolderNamesByDocumentId(string docId, CategoryDetailsModels categoryFolderDetails);
    }
}
