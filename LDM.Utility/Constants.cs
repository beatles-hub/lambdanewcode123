using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using LDM.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Utility
{
    public static class Constants
    {
        public const string BrowserDownload = "Browser";
        public const string EmailDownload = "Email";
        public const string SMTPEmail = "SMTPEmail";
        public const string SMTPEmailPwd = "SMTPEmailPwd";
        public const string SMTPPort = "SMTPPort";
        public const string SMTPHost = "SMTPHost";
        public const string EmailCC = "EmailCC";
        public const string EmailSubject = "EmailSubject";
        public const string EmailBody = "EmailBody";


        public const string CORSAllowUrl = "CORSAllowUrl";
        public const string BrowserDownloadLimit = "BrowserDownloadLimit";
        public const string EmailFunction = "EmailFunction";
        public const string SendEmailFunction = "SendEmailFunction";
        public const string SecretManagerKey = "SecretManagerKey";
        public const string Region = "Region";
        public const string Server = "Server";
        public const string Uid = "Uid";
        public const string Pwd = "Pwd";
        public const string DBName = "DBName";
        public const string AllowExtension = "AllowExtensions";
        public const string DownloadUrlTimeOut = "DownloadUrlTimeOut";
        public const string UploadUrlTimeOut = "UploadUrlTimeOut";

        public const string LogFilePath = "LogFilePath";
        public const string Domain = "Domain";
        public const string ApiIdentifier = "ApiIdentifier";

        public const string ItemIdField = "ITEM_ID";
        public const string ItemIdHeader = "ITEM_ID";
        public const string ConditionOr = "OR";
        public const string CondistionAnd = "AND";
        public const string DateFormatSql = "%m/%d/%Y";
        public const string DateFormat = "mm/dd/yyyy";

        public const string DateField = "DATE";
        public const string TimeStampField = "TIMESTAMP";

        public const string DateOperator = "Date";
        public const string TextOperator = "Text";
        public const string NumberOperator = "Number";

        
        #region-- Search Operators --
        public const string EQUALTO = "=";
        public const string NOTEQUALTO = "!=";
        public const string LIKE = "LIKE";
        public const string NOTLIKE = "NOT LIKE";
        public const string IN = "IN";
        public const string NOTIN = "NOT IN";
        public const string LESSTHANEQUALTO = "<=";
        public const string LESSTHAN = "<";
        public const string GREATERTHANEQUALTO = ">=";
        public const string GREATERTHAN = ">";
        public const string BETWEEN = "BETWEEN";
        #endregion
              

        public static object GetPropertyValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        public static RegionEndpoint GetRegion()
        {
            return RegionEndpoint.GetBySystemName(EnvVeriables.Region);
        }
        public static string ConvertStringToDate(string input)
        {
            string output = string.Empty;
            try
            {
                output = " STR_TO_DATE('" + input + "', '" + Constants.DateFormatSql + "') ";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return output;
        }
        public static bool CheckDateFormat(string input)
        {
            bool output = false;
            try
            {
                string[] arrayStr = input.Split('/');
                if (Convert.ToInt32(arrayStr[0]) <= 12)
                {
                    output = true;
                }
                if (output == true && Convert.ToInt32(arrayStr[1]) <= 31)
                {
                    output = true;
                }
                else
                {
                    output = false;
                }
                if (output == true && arrayStr[2].Length == 4)
                {
                    output = true;
                }
                else
                {
                    output = false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return output;
        }
        public static bool AllowExtensions(string extensions, string FileName)
        {
            bool isValid = true;
            extensions = extensions.ToLower();
            List<string> allowedExtensions = extensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (FileName != null)
            {
                var fileName = FileName.ToLower();
                isValid = allowedExtensions.Any(y => fileName.EndsWith(y));
            }
            return isValid;
        }
        public static string GetItemID()
        {
            return Convert.ToString(DateTime.Now.Ticks);
        }
        public static string ConvertDateToString(string input)
        {
            string output = string.Empty;
            try
            {
                //output = " DATE_FORMAT(" + input + ", '" + Constants.DateFormatSql + "') as "+ input+" ";
                output = input;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return output;
        }
        public static bool ValidateGetCategoryParams(CategoryParamModels model, out string validationMsg)
        {
            bool flag = true;
            validationMsg = string.Empty;
            if (string.IsNullOrEmpty(model.UserCode))
            {
                flag = false;
                validationMsg = ResponseMessages.UserCodeError;
            }
            if (string.IsNullOrEmpty(model.CategoryCode) && flag == true)
            {
                validationMsg = ResponseMessages.CategoryCodeError;
            }
            return flag;
        }

        public static bool ValidateDownloadDocument(DownloadDocumentModels model, out string validationMsg)
        {
            bool flag = true;
            validationMsg = string.Empty;
            if (string.IsNullOrEmpty(model.UserCode))
            {
                flag = false;
                validationMsg = ResponseMessages.UserCodeError;
            }
            if (string.IsNullOrEmpty(model.CategoryCode) && flag == true)
            {
                flag = false;
                validationMsg = ResponseMessages.CategoryCodeError;
            }
            if (model.ItemIds == null && flag == true)
            {
                flag = false;
                validationMsg = ResponseMessages.ItemIdsError;
            }
            if (model.ItemIds != null && flag == true)
            {
                if (model.ItemIds.Count == 0)
                {
                    flag = false;
                    validationMsg = ResponseMessages.ItemIdsError;
                }
            }
            return flag;
        }

        public static bool ValidateUploadDocumentDetails(UploadDocumentDetailsModels model, out string validationMsg)
        {
            bool flag = true;
            validationMsg = string.Empty;
            if (string.IsNullOrEmpty(model.UserCode))
            {
                flag = false;
                validationMsg = ResponseMessages.UserCodeError;
            }
            if (string.IsNullOrEmpty(model.CategoryCode) && flag == true)
            {
                flag = false;
                validationMsg = ResponseMessages.CategoryCodeError;
            }
            if (string.IsNullOrEmpty(model.FileName) && flag == true)
            {
                flag = false;
                validationMsg = ResponseMessages.FileNameError;
            }
            if (model.UploadFields == null && flag == true)
            {
                flag = false;
                validationMsg = ResponseMessages.UploadFieldsError;
            }
            if (model.UploadFields != null && flag == true)
            {
                if (model.UploadFields.Count == 0)
                {
                    flag = false;
                    validationMsg = ResponseMessages.UploadFieldsError;
                }
            }
            return flag;
        }

        public static bool ValidateUpdateDocumentSuccess(UploadSuccessModels model, out string validationMsg)
        {
            bool flag = true;
            validationMsg = string.Empty;
            if (string.IsNullOrEmpty(model.UserCode))
            {
                flag = false;
                validationMsg = ResponseMessages.UserCodeError;
            }
            if (string.IsNullOrEmpty(model.CategoryCode) && flag == true)
            {
                validationMsg = ResponseMessages.CategoryCodeError;
            }
            if (string.IsNullOrEmpty(model.ITEM_ID) && flag == true)
            {
                validationMsg = ResponseMessages.ITEM_IDError;
            }
            return flag;
        }

        public static bool ValidateDownloadDocumentZIP(DownloadDocumentZIPModels model, out string validationMsg)
        {
            bool flag = true;
            validationMsg = string.Empty;
            if (string.IsNullOrEmpty(model.UserCode))
            {
                flag = false;
                validationMsg = ResponseMessages.UserCodeError;
            }
            if (string.IsNullOrEmpty(model.CategoryCode) && flag == true)
            {
                flag = false;
                validationMsg = ResponseMessages.CategoryCodeError;
            }
            if (model.ItemIds == null && flag == true)
            {
                flag = false;
                validationMsg = ResponseMessages.ItemIdsError;
            }
            if (model.ItemIds != null && flag == true)
            {
                if (model.ItemIds.Count == 0)
                {
                    flag = false;
                    validationMsg = ResponseMessages.ItemIdsError;
                }
            }
            return flag;
        }

        public static bool ValidateGetUserDetails(UserDetailsModels model, out string validationMsg)
        {
            bool flag = true;
            validationMsg = string.Empty;
            if (string.IsNullOrEmpty(model.UserCode))
            {
                flag = false;
                validationMsg = ResponseMessages.UserCodeError;
            }
            return flag;
        }

        public static bool ValidateGetUserCategories(UserCategoriesModels model, out string validationMsg)
        {
            bool flag = true;
            validationMsg = string.Empty;
            if (string.IsNullOrEmpty(model.UserCode))
            {
                flag = false;
                validationMsg = ResponseMessages.UserCodeError;
            }
            return flag;
        }
        public static bool ValidateGetSearchResults(SearchResultsModels model, out string validationMsg)
        {
            bool flag = true;
            validationMsg = string.Empty;
            if (string.IsNullOrEmpty(model.UserCode))
            {
                flag = false;
                validationMsg = ResponseMessages.UserCodeError;
            }
            if (string.IsNullOrEmpty(model.CategoryCode) && flag == true)
            {
                flag = false;
                validationMsg = ResponseMessages.CategoryCodeError;
            }
            if (string.IsNullOrEmpty(model.Condition) && flag == true)
            {
                flag = false;
                validationMsg = ResponseMessages.ConditionError;
            }
            if (model.SearchFields != null && flag == true)
            {
                if (model.SearchFields.Count == 0)
                {
                    flag = false;
                    validationMsg = ResponseMessages.SearchFieldsError;
                }
            }

            return flag;
        }

        public static bool ValidateGetSearchOperators(SearchOperatorsModels model, out string validationMsg)
        {
            bool flag = true;
            validationMsg = string.Empty;
            if (string.IsNullOrEmpty(model.UserCode))
            {
                flag = false;
                validationMsg = ResponseMessages.UserCodeError;
            }

            return flag;
        }

        public static bool ValidateDownloadDocumentById(RiskTechDocumentByIdModel model, out string validationMsg)
        {
            bool flag = true;
            validationMsg = string.Empty;
            if (string.IsNullOrEmpty(model.UserCode))
            {
                flag = false;
                validationMsg = ResponseMessages.UserCodeError;
            }
            if (string.IsNullOrEmpty(model.CategoryCode) && flag == true)
            {
                flag = false;
                validationMsg = ResponseMessages.CategoryCodeError;
            }
            if (model.DocumentIds != null && flag == true)
            {
                if (model.DocumentIds.Count == 0)
                {
                    flag = false;
                    validationMsg = ResponseMessages.DocumentIdsError;
                }
            }

            return flag;
        }

        public static bool ValidateGetFileDownloadDetails(FileDownloadDetailsModels model, out string validationMsg)
        {
            bool flag = true;
            validationMsg = string.Empty;
            if (string.IsNullOrEmpty(model.UserCode))
            {
                flag = false;
                validationMsg = ResponseMessages.UserCodeError;
            }
            if (string.IsNullOrEmpty(model.CategoryCode) && flag == true)
            {
                flag = false;
                validationMsg = ResponseMessages.CategoryCodeError;
            }
            if (model.ItemIds == null && flag == true)
            {
                flag = false;
                validationMsg = ResponseMessages.ItemIdsError;
            }
            if (model.ItemIds != null && flag == true)
            {
                if (model.ItemIds.Count == 0)
                {
                    flag = false;
                    validationMsg = ResponseMessages.ItemIdsError;
                }
            }
            return flag;
        }

        public static Dictionary<string, string> GetResponseHeaders()
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            keyValues.Add("Access-Control-Allow-Origin", EnvVeriables.AllowOrigin);
            keyValues.Add("Access-Control-Allow-Headers", "Content-Type,Authorization");
            keyValues.Add("Access-Control-Allow-Methods", "OPTIONS,POST");
            keyValues.Add("Content-Type", "application/json");
            return keyValues;
        }
        public static string GetConnectionString()
        {
            SecretManagerModels secretManager = GetSecretValue(EnvVeriables.SecretManagerKey);
            string Uid = secretManager.username;
            string Pwd = secretManager.password;
            string Host = secretManager.host;
            string conString= @"SERVER=" + Host + ";DATABASE=" + EnvVeriables.DBName + ";UID=" + Uid + ";PASSWORD=" + Pwd + ";convert zero datetime=True;";
            return conString;
        }
        public static SecretManagerModels GetSecretValue(string key)
        {
            SecretManagerModels secretManager = new SecretManagerModels();
            string value = string.Empty;
            try
            {   
                MemoryStream memoryStream = new MemoryStream();
                IAmazonSecretsManager client = new AmazonSecretsManagerClient(Constants.GetRegion());
                GetSecretValueRequest request = new GetSecretValueRequest();
                request.SecretId = key;
                request.VersionStage = "AWSCURRENT"; 
                GetSecretValueResponse response = null;
                response = client.GetSecretValueAsync(request).Result;
                if (response.SecretString != null)
                {
                    value = response.SecretString;
                }
                else {
                    memoryStream = response.SecretBinary;
                    StreamReader reader = new StreamReader(memoryStream);
                    value = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
                }
            }
            catch
            {
                throw;
            }
            secretManager = JsonConvert.DeserializeObject<SecretManagerModels>(value);
            return secretManager;
        }
    }
}


