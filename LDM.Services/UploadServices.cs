using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using LDM.Core;
using LDM.Data;
using LDM.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LDM.Services
{
    public class UploadServices : IUploadServices
    {
        private readonly IUpload _upload;
        private readonly IUser _user;
        private readonly IAmazonS3 S3Client = null;
        private static RegionEndpoint bucketRegion = null;      
        public UploadServices()
        {
            _upload = new Upload();
            _user = new User();
            bucketRegion = Constants.GetRegion();          
            this.S3Client = new AmazonS3Client(bucketRegion);
        }       
        public async Task<DocumentDetailsModels> UploadDocumentDetails(UploadDocumentDetailsModels uploadDocumentDetails)
        {
            DocumentDetailsModels documentDetails = null;
            try
            {
                if (Constants.AllowExtensions(EnvVeriables.AllowExtensions, uploadDocumentDetails.FileName))
                {
                    string FileName = string.Empty;
                    string ItemId = string.Empty;
                    bool IsSuccess = false;
                    bool isDateCorrect = true;
                    List<string> columnList = new List<string>();
                    List<object> columnValuesList = new List<object>();
                    List<CategoryColumnModels> categoryColumns = await _upload.GetCategoryColumns(uploadDocumentDetails.CategoryCode);
                    if (categoryColumns.Count > 0)
                    {
                        foreach (var item in categoryColumns)
                        {
                            var propValue = uploadDocumentDetails.UploadFields.Where(x => x.Field == item.ColumnName).Select(x => x.Value).FirstOrDefault();
                            if (propValue != null)
                            {
                                columnList.Add(item.ColumnName);
                                if (item.ColumnName.Contains(Constants.DateField) || item.ColumnName == Constants.TimeStampField)
                                {
                                    if (Constants.CheckDateFormat(propValue.ToString()))
                                    {
                                        string value = Constants.ConvertStringToDate(propValue.ToString());
                                        columnValuesList.Add(value);
                                    }
                                    else
                                    {
                                        isDateCorrect = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    columnValuesList.Add("'" + propValue + "'");
                                }
                            }
                        }
                        if (isDateCorrect)
                        {
                            string columnNames = string.Join(",", columnList);
                            string columnValues = string.Join(",", columnValuesList);
                            CategoryDetailsModels categoryDetails =await _upload.GetCategoryDetails(uploadDocumentDetails.CategoryCode);
                            if (categoryDetails != null && !string.IsNullOrEmpty(columnNames) && !string.IsNullOrEmpty(columnValues))
                            {
                                ItemId = Constants.GetItemID();
                                IsSuccess = await _upload.AddDocumentDetails(ItemId, uploadDocumentDetails.UserCode, categoryDetails.TableName, columnNames, columnValues);
                            }
                            if (IsSuccess)
                            {
                                FileName = uploadDocumentDetails.FileName;
                                IsSuccess = await _upload.AddDocumentMapping(uploadDocumentDetails.CategoryCode, uploadDocumentDetails.UserCode, ItemId, FileName);
                            }
                            if (IsSuccess)
                            {
                                string URL = await GetUploadPreSignedUrl(uploadDocumentDetails.CategoryCode, ItemId, FileName);
                                if (!string.IsNullOrEmpty(URL))
                                {
                                    documentDetails = new DocumentDetailsModels();
                                    documentDetails.UploadUrl = URL;
                                    documentDetails.ITEM_ID = ItemId;
                                }
                            }
                        }
                    }
                }
                return documentDetails;
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> UpdateDocumentSuccess(UploadSuccessModels model)
        {
            bool isSuccess = false;
            try
            {
                CategoryDetailsModels categoryDetails =await _upload.GetCategoryDetails(model.CategoryCode);
                if (categoryDetails != null)
                {
                    isSuccess = await _upload.UpdateDocumentSuccess(model, categoryDetails);
                }
                return isSuccess;
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<DownloadDocumentDetailsModels>> DownloadDocument(DownloadDocumentModels downloadDocument)
        {
            List<DownloadDocumentDetailsModels> documentList = null;
            try
            {
                List<CategoryFileNameModels> categoryFileNames = null;
                string delimiter = "/";
                string filepath = string.Empty;
                documentList = new List<DownloadDocumentDetailsModels>();
                CategoryDetailsModels categoryDetails = await _upload.GetCategoryDetails(downloadDocument.CategoryCode);
                foreach (var item in downloadDocument.ItemIds)
                {
                    DownloadDocumentDetailsModels itemDocuments;
                    CategoryFileMappingDetails categoryFolderNames = await _upload.GetCategoryFolderNames(item, categoryDetails);
                    if (categoryFolderNames!=null)
                    {
                        filepath = categoryDetails.CategoryName + delimiter + categoryFolderNames.Year + delimiter +
                            categoryFolderNames.MainFolder + delimiter + categoryFolderNames.SubFolder + delimiter;
                        categoryFileNames = await _upload.GetDownloadDocuments(item, downloadDocument.CategoryCode);
                    }                   

                    if (categoryFileNames != null )
                    {
                        if (categoryFileNames.Count>0)
                        {
                            List<FileModels> filesList = new List<FileModels>();

                            foreach (var cat in categoryFileNames)
                            {
                                FileModels filesModels = new FileModels();
                                filesModels.FileName = cat.FileName;
                                filesModels.FileURL = await GetDownloadsPreSignedUrl(filepath + cat.FileName, categoryDetails.S3BucketName);
                                if (!string.IsNullOrEmpty(filesModels.FileURL))
                                {
                                    filesList.Add(filesModels);
                                }
                            }

                            if (filesList.Count > 0)
                            {
                                itemDocuments = new DownloadDocumentDetailsModels();
                                itemDocuments.ItemId = item;
                                itemDocuments.Files = filesList;
                                documentList.Add(itemDocuments);
                            }
                        }                        
                    }
                }
                return documentList;
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<RiskTechDocumentByIdResultModel>> DownloadDocumentsByDocId(RiskTechDocumentByIdModel downloadDocument)
        {
            List<RiskTechDocumentByIdResultModel> documentDetails = new List<RiskTechDocumentByIdResultModel>();
            try
            {
                string delimiter = "/";
                List<CategoryFileNameModels> categoryFileNames = null;
                CategoryDetailsModels categoryDetails = await _upload.GetCategoryDetails(downloadDocument.CategoryCode);
                foreach (string DocId in downloadDocument.DocumentIds)
                {

                    CategoryFileMappingDetails categoryFolderNames = await _upload.GetCategoryFolderNamesByDocumentId(DocId, categoryDetails);
                    string filepath = categoryDetails.CategoryName + delimiter + categoryFolderNames.Year + delimiter +
                        categoryFolderNames.MainFolder + delimiter + categoryFolderNames.SubFolder + delimiter;
                    categoryFileNames = await _upload.GetDownloadDocumentsByDocumentId(DocId, downloadDocument.CategoryCode);
                    if (categoryFileNames != null)
                    {
                        categoryFileNames.ForEach(async fileName =>
                        {
                            RiskTechDocumentByIdResultModel documentIdDetails = new RiskTechDocumentByIdResultModel();
                            documentIdDetails.Document_Id = DocId;
                            documentIdDetails.FileUrl = await GetDownloadsPreSignedUrl(filepath + fileName.FileName, categoryDetails.S3BucketName);
                            documentDetails.Add(documentIdDetails);
                        });
                    }

                }
            }
            catch
            {
                throw;
            }
            return documentDetails;
        }
        public async Task<FileDownloadDetails> GetFileDownloadDetails(FileDownloadDetailsModels model)
        {
            string zipFilePath = string.Empty;
            string delimiter = "/";
            FileDownloadDetails details = null;
            try
            {
                string itemId = model.ItemIds[0];
          
                CategoryDetailsModels categoryDetails = await _upload.GetCategoryDetails(model.CategoryCode);
                CategoryFileMappingDetails categoryFolderNames = await _upload.GetCategoryFolderNames(itemId, categoryDetails);
                string filepath = categoryDetails.CategoryName + delimiter + categoryFolderNames.Year + delimiter +
                    categoryFolderNames.MainFolder + delimiter + categoryFolderNames.SubFolder + delimiter;

                details = new FileDownloadDetails();
                bool IsUnderLimit = await CheckDownloadLimit(categoryDetails.S3BucketName, filepath);
                if (IsUnderLimit)
                {
                    details.DownloadMethod = Constants.BrowserDownload;
                }
                else
                {
                    details.DownloadMethod = Constants.EmailDownload;
                }
                return details;
            }
            catch
            {
                throw;
            }
        }
        public async Task<EmailFileModels> DownloadDocumentZip(DownloadDocumentZIPModels downloadDocument)
        {
            string zipFilePath = string.Empty;
            string delimiter = "/";
            EmailFileModels fileModel = null;
            try
            {
                string itemId = downloadDocument.ItemIds[0];

                List<string> fileObject = new List<string>();
                List<CategoryFileNameModels> categoryFileNames = null;
                CategoryDetailsModels categoryDetails = await _upload.GetCategoryDetails(downloadDocument.CategoryCode);
                CategoryFileMappingDetails categoryFolderNames = await _upload.GetCategoryFolderNames(itemId, categoryDetails);
                string filepath = categoryDetails.CategoryName + delimiter + categoryFolderNames.Year + delimiter +
                    categoryFolderNames.MainFolder + delimiter + categoryFolderNames.SubFolder + delimiter;

                if (!string.IsNullOrEmpty(filepath))
                {
                    string tempPath = System.IO.Path.GetTempPath() + "ZipFile/";
                    if (!Directory.Exists(tempPath))
                    {
                        System.IO.Directory.CreateDirectory(tempPath);
                    }                  
                    string zipName = categoryFolderNames.MainFolder + "_" + categoryFolderNames.SubFolder + ".zip";
                    string zipTempFilePath = System.IO.Path.GetTempPath() + zipName;
                   
                    categoryFileNames = await _upload.GetDownloadDocuments(itemId, downloadDocument.CategoryCode);
                    foreach (var cat in categoryFileNames)
                    {
                        GetObjectResponse response = await GetS3Object(filepath + cat.FileName, categoryDetails.S3BucketName);
                        response.WriteResponseStreamToFileAsync(tempPath + cat.FileName, false, System.Threading.CancellationToken.None).Wait();

                        using (var zipArchive = ZipFile.Open(zipTempFilePath, ZipArchiveMode.Update))
                        {
                            var fileInfo = new FileInfo(tempPath + cat.FileName);
                            zipArchive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
                        }
                        File.Delete(tempPath + cat.FileName);                        
                    }
                    Directory.Delete(tempPath, true);
                    string s3FilePathPath = "TEMP" + delimiter + zipName;
                    PutObjectResponse putResponse = await PutZipFileToS3(categoryDetails.S3BucketName, s3FilePathPath, zipTempFilePath);
                    if (putResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        zipFilePath = await GetDownloadsPreSignedUrl(s3FilePathPath, categoryDetails.S3BucketName);
                    }
                    File.Delete(zipTempFilePath);
                    fileModel = new EmailFileModels()
                    {
                        FileName = zipName,
                        FileURL = zipFilePath,
                        EmailZip = false
                    };
                }                              
                return fileModel;
            }
            catch
            {
                throw;
            }
        }
        public async Task<SendEmailModel> EmailDocumentZIP(DownloadDocumentModels downloadDocument)
        {
            string zipFilePath = string.Empty;
            string delimiter = "/";
            try
            {
                string itemId = downloadDocument.ItemIds[0];

                List<CategoryFileNameModels> categoryFileNames = null;
                CategoryDetailsModels categoryDetails = await _upload.GetCategoryDetails(downloadDocument.CategoryCode);
                CategoryFileMappingDetails categoryFolderNames = await _upload.GetCategoryFolderNames(itemId, categoryDetails);
                string filepath = categoryDetails.CategoryName + delimiter + categoryFolderNames.Year + delimiter +
                    categoryFolderNames.MainFolder + delimiter + categoryFolderNames.SubFolder + delimiter;

                string tempPath = System.IO.Path.GetTempPath() + "ZipFile/";
                if (!Directory.Exists(tempPath))
                {
                    System.IO.Directory.CreateDirectory(tempPath);
                }
                string zipName = categoryFolderNames.MainFolder + "_" + categoryFolderNames.SubFolder + ".zip";
                string zipTempFilePath = System.IO.Path.GetTempPath() + zipName;

                //DirectoryInfo dirInfo = new DirectoryInfo(tempPath);
                //long dirSize = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
                //dirSize = (((dirSize / 1024) / 1024) / 1024);               

                categoryFileNames = await _upload.GetDownloadDocuments(itemId, downloadDocument.CategoryCode);
                foreach (var cat in categoryFileNames)
                {
                    if (!File.Exists(tempPath + cat.FileName))
                    {
                        GetObjectResponse response = GetS3Object(filepath + cat.FileName, categoryDetails.S3BucketName).GetAwaiter().GetResult();
                        response.WriteResponseStreamToFileAsync(tempPath + cat.FileName, false, System.Threading.CancellationToken.None).Wait();
                    }
                    using (var zipArchive = ZipFile.Open(zipTempFilePath, ZipArchiveMode.Update))
                    {
                        var fileInfo = new FileInfo(tempPath + cat.FileName);
                        zipArchive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
                    }
                    File.Delete(tempPath + cat.FileName);
                    
                }
                Directory.Delete(tempPath, true);

                string s3FilePathPath = "TEMP" + delimiter + zipName;

                PutObjectResponse putResponse = await PutZipFileToS3(categoryDetails.S3BucketName, s3FilePathPath, zipTempFilePath);

                if (putResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    zipFilePath = await GetDownloadsPreSignedUrl(s3FilePathPath, categoryDetails.S3BucketName);                    
                }
                File.Delete(zipTempFilePath);

                SendEmailModel emailModel = await GetEmailConfiguration();
                var user= await _user.GetUserAlldetails(downloadDocument.UserCode);
                emailModel.Subject = string.Format(emailModel.Subject, categoryDetails.CategoryAlias, categoryFolderNames.MainFolder);
                emailModel.Body = string.Format(emailModel.Body, user.UserName,zipFilePath);
                emailModel.ToEmail = user.WindowsEmail;
                return emailModel;
            }
            catch 
            {               
                throw;
            }
        }

        #region Private functions
        private async Task<SendEmailModel> GetEmailConfiguration()
        {
            SendEmailModel emailModel = null;
            try
            {               
                List<EmailConfigModels> emailConfigs = await _user.GetEmailConfiguration();
                emailModel = new SendEmailModel();
                EmailConfiguration config = new EmailConfiguration();
                config.SMTPEmail = emailConfigs.Where(x => x.ConfigKey == Constants.SMTPEmail).Select(x => x.ConfigValue).FirstOrDefault();
                config.SMTPEmailPwd = emailConfigs.Where(x => x.ConfigKey == Constants.SMTPEmailPwd).Select(x => x.ConfigValue).FirstOrDefault();
                config.SMTPPort = Convert.ToInt32(emailConfigs.Where(x => x.ConfigKey == Constants.SMTPPort).Select(x => x.ConfigValue).FirstOrDefault());
                config.SMTPHost = emailConfigs.Where(x => x.ConfigKey == Constants.SMTPHost).Select(x => x.ConfigValue).FirstOrDefault();
                config.EmailCC = emailConfigs.Where(x => x.ConfigKey == Constants.EmailCC).Select(x => x.ConfigValue).FirstOrDefault();
                emailModel.configuration = config;
                emailModel.Subject = emailConfigs.Where(x => x.ConfigKey == Constants.EmailSubject).Select(x => x.ConfigValue).FirstOrDefault();
                emailModel.Body = emailConfigs.Where(x => x.ConfigKey == Constants.EmailBody).Select(x => x.ConfigValue).FirstOrDefault();
            }
            catch
            {
                throw;
            }
            return emailModel;
        }
        private async Task<bool> CheckDownloadLimit(string S3BucketName, string filePath)
        {
            bool isUnderLimit = false;
            try
            {
                ListObjectsRequest request2 = new ListObjectsRequest
                {
                    BucketName = S3BucketName,
                    Prefix = filePath
                };
                double mbsize = 0;
                ListObjectsResponse response2 = await S3Client.ListObjectsAsync(request2);
                if (response2 != null && response2.S3Objects != null)
                {
                    long bytes = response2.S3Objects.Sum(s => s.Size);
                    mbsize = (bytes / 1024f) / 1024f;
                }
                if (mbsize <= EnvVeriables.DownloadLimit)
                {
                    isUnderLimit = true;
                }                
            }
            catch
            {
                throw;
            }
            return isUnderLimit;
        }
        private async Task<PutObjectResponse> PutZipFileToS3(string S3BucketName, string s3FilePathPath, string zipTempFilePath)
        {
            var putRequest1 = new PutObjectRequest
            {
                BucketName = S3BucketName,
                Key = s3FilePathPath,
                FilePath = zipTempFilePath
            };
            PutObjectResponse response = await S3Client.PutObjectAsync(putRequest1);
            return response;
        }
        private async Task WriteResponseToFile(GetObjectResponse response, string tmpPath)
        {
            await response.WriteResponseStreamToFileAsync(tmpPath, false, System.Threading.CancellationToken.None);
        }
        private async Task<GetObjectResponse> GetS3Object(string filepath, string S3BucketName)
        {
            GetObjectRequest request = new GetObjectRequest()
            {
                BucketName = S3BucketName,
                Key = filepath
            };
            GetObjectResponse response = await S3Client.GetObjectAsync(request);
            return response;
        }
        
        private async Task<string> GetUploadPreSignedUrl(string categoryCode, string ItemId, string fileName)
        {
            string URL = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    CategoryDetailsModels categoryFolderDetails = await _upload.GetCategoryDetails(categoryCode);
                    CategoryFileMappingDetails categoryFolderNames = await _upload.GetCategoryFolderNames(ItemId, categoryFolderDetails);
                    string delimiter = "/";
                    string filepath = string.Empty;
                    int year = DateTime.Now.Year;
                    if (!string.IsNullOrEmpty(categoryFolderNames.MainFolder) && !string.IsNullOrEmpty(categoryFolderNames.SubFolder))
                    {
                        filepath = categoryFolderDetails.CategoryName + delimiter + year + delimiter +
                            categoryFolderNames.MainFolder + delimiter + categoryFolderNames.SubFolder + delimiter + fileName;
                        URL = await GenerateUploadPreSignedURL(filepath, categoryFolderDetails.S3BucketName);
                    }

                }
            }
            catch
            {
                throw;
            }
            return URL;

        }
        private async Task<string> GenerateUploadPreSignedURL(string fileKey, string S3BucketName)
        {
            string urlString = string.Empty;
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = S3BucketName,
                    Key = fileKey,
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.Now.AddMinutes(EnvVeriables.UploadUrlExpires)
                };
                urlString = S3Client.GetPreSignedURL(request);
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return urlString;
        }
        private async Task<List<S3Object>> GetFilesFromBucket(string filepath, string S3BucketName)
        {
            List<S3Object> S3Objects = null;
            try
            {
                var response = await this.S3Client.ListObjectsV2Async(new ListObjectsV2Request
                {
                    BucketName = S3BucketName,
                    Prefix = filepath
                });
                if (response.S3Objects.Count > 0)
                {
                    S3Objects = response.S3Objects;
                }
            }
            catch
            {
                throw;
            }
            return S3Objects;
        }
        private async Task<string> GetDownloadsPreSignedUrl(string fileKey, string S3BucketName)
        {
            string urlString = string.Empty;
            try
            {
                GetPreSignedUrlRequest request1 = new GetPreSignedUrlRequest
                {
                    BucketName = S3BucketName,
                    Key = fileKey,
                    Verb = HttpVerb.GET,
                    Expires = DateTime.Now.AddMinutes(EnvVeriables.DownloadUrlExpires)
                };
                urlString = S3Client.GetPreSignedURL(request1);
                await Task.CompletedTask;
            }
            catch
            {
                throw;
            }
            return urlString;
        }
        #endregion
        
    }
}
