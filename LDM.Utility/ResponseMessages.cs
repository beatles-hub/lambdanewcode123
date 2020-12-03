using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Utility
{
    public static class ResponseMessages
    {
        public static readonly string ValidationError = "A validation error occurred.";
        public static readonly string UserAuthError = "User doesn't have the right permission to access categories. Please contact the System Administrator.";
        public static readonly string UserRecordNotFound = "User record not found. Please contact the System Administrator.";
        public static readonly string CategoryRecordsNotFound = "Category record not found. Please contact the System Administrator.";
        public static readonly string DocumentDownloadRecordsNotFound = "Documents record not found for download.";
        public static readonly string CategoryParamRecordsNotFound = "Requested category parameter records not found. Please contact the System Administrator.";
        public static readonly string SearchMetaDataRecordsNotFound = "Search results record not found.";
        public static readonly string SearchOperatorsRecordsNotFound = "Search operator's record not found. Please contact the System Administrator";
        public static readonly string InternalServerError = "A Server Error Occurred. Please Contact the System Administartor.";
        public static readonly string UpdateUploadError = "Internal server error for update upload success.";
        public static readonly string UploadDocumentError = "Internal server error for upload document.";
        public static readonly string UserAuthAdminError = "User doesn't have the right permission to upload a document. Please contact the System Administrator.";
        public static readonly string UserCodeError = "User Code is required.";
        public static readonly string CategoryCodeError = "Category Code is required.";
        public static readonly string ItemIdsError = "ItemIds are required.";
        public static readonly string FileNameError = "FileName is required.";
        public static readonly string UploadFieldsError = "UploadFields are required.";
        public static readonly string ITEM_IDError = "ITEM_ID is required.";
        public static readonly string ConditionError = "AND or OR Condition is required.";
        public static readonly string SearchFieldsError = "Search Fields are required.";
        public static readonly string DocumentIdsError = "DocumentIds are required.";
        public static readonly string FileDownlodDetailsNotFound = "File download details record not found.";

    }
}
