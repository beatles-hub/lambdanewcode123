using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon;
using LDM.Core;
using LDM.Services;
using LDM.Utility;
using Newtonsoft.Json;
using Amazon.S3.Model;
using Amazon.S3;
using Amazon.Lambda;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LDM.API
{
    public class Functions
    {
        // This const is the name of the environment variable that the serverless.template will use to set       
        private readonly ICategoryServices _categoryService;
        private readonly IUserServices _userService;
        private readonly IUploadServices _uploadService;
        private readonly ISearchServices _searchServices;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public Functions()
        {
            _userService = new UserServices();
            _categoryService = new CategoryServices();
            _uploadService = new UploadServices();
            _searchServices = new SearchServices();
        }

        /// <summary>
        /// A Lambda function that get user details.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns> 
        public async Task<APIGatewayProxyResponse> GetUserDetails(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var response = new APIGatewayProxyResponse();
            response.Headers = Constants.GetResponseHeaders();
            int statusCode = (int)HttpStatusCode.OK;
            object statusMessage = string.Empty;
            var model = JsonConvert.DeserializeObject<UserDetailsModels>(request?.Body);
            try
            {
                if (model != null)
                {
                    string validationMsg = string.Empty;
                    if (!Constants.ValidateGetUserDetails(model, out validationMsg))
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.Body = JsonConvert.SerializeObject(validationMsg);
                        return response;
                    }
                    if (!await _userService.CheckUserAuth(model.UserCode))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        response.Body = JsonConvert.SerializeObject(ResponseMessages.UserAuthError);
                        return response;
                    }
                    var user = await _userService.GetUserDetails(model);
                    if (user != null)
                    {
                        statusCode = (int)HttpStatusCode.OK;
                        statusMessage = user;
                    }
                    else
                    {
                        statusCode = (int)HttpStatusCode.NotFound;
                        statusMessage = ResponseMessages.UserRecordNotFound;
                    }
                }
                else
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    statusMessage = ResponseMessages.ValidationError;
                }
            }
            catch (Exception ex)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                statusMessage = ResponseMessages.InternalServerError;
                LambdaLogger.Log("\nStackStrace: " + ex.StackTrace);
                LambdaLogger.Log("\nMessage: " + ex.Message + "\n");
            }
            response.StatusCode = statusCode;
            response.Body = JsonConvert.SerializeObject(statusMessage);
            return response;
        }
        
        /// <summary>
        /// A Lambda function that get user categories.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns> 
        public async Task<APIGatewayProxyResponse> GetUserCategories(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var response = new APIGatewayProxyResponse();
            response.Headers = Constants.GetResponseHeaders();
            int statusCode = (int)HttpStatusCode.OK;
            object statusMessage = string.Empty;
            var model = JsonConvert.DeserializeObject<UserCategoriesModels>(request?.Body);
            try
            {
                if (model != null)
                {
                    string validationMsg = string.Empty;
                    if (!Constants.ValidateGetUserCategories(model, out validationMsg))
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.Body = JsonConvert.SerializeObject(validationMsg);
                        return response;
                    }
                    if (!await _userService.CheckUserAuth(model.UserCode))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        response.Body = JsonConvert.SerializeObject(ResponseMessages.UserAuthError);
                        return response;
                    }
                    var userCategories = await _userService.GetUserCategories(model);
                    if (userCategories.Count > 0)
                    {
                        statusCode = (int)HttpStatusCode.OK;
                        statusMessage = userCategories;
                    }
                    else
                    {
                        statusCode = (int)HttpStatusCode.NotFound;
                        statusMessage = ResponseMessages.CategoryRecordsNotFound;
                    }
                }
                else
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    statusMessage = ResponseMessages.ValidationError;
                }
            }
            catch (Exception ex)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                statusMessage = ResponseMessages.InternalServerError;
                LambdaLogger.Log("\nStackStrace: " + ex.StackTrace);
                LambdaLogger.Log("\nMessage: " + ex.Message + "\n");
            }
            response.StatusCode = statusCode;
            response.Body = JsonConvert.SerializeObject(statusMessage);
            return response;
        }

        /// <summary>
        /// A Lambda function that get category parameters.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>              
        public async Task<APIGatewayProxyResponse> GetCategoryParams(APIGatewayProxyRequest request, ILambdaContext context)
        {          
            var response = new APIGatewayProxyResponse();
            response.Headers = Constants.GetResponseHeaders();
            int statusCode = (int)HttpStatusCode.OK;
            object statusMessage = string.Empty;
            try
            {
                var model = JsonConvert.DeserializeObject<CategoryParamModels>(request?.Body);
                if (model != null)
                {
                    string validationMsg = string.Empty;
                    bool IsValidModel = Constants.ValidateGetCategoryParams(model,out validationMsg);
                    if (!IsValidModel)
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.Body = JsonConvert.SerializeObject(validationMsg);
                        return response;
                    }
                    if (!await _userService.CheckUserAuth(model.UserCode))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        response.Body = JsonConvert.SerializeObject(ResponseMessages.UserAuthError);
                        return response;
                    }
                    var categoryColumns = await _categoryService.GetCategoryParams(model.CategoryCode);
                    if (categoryColumns != null)
                    {
                        if (categoryColumns.Count > 0)
                        {
                            statusCode = (int)HttpStatusCode.OK;
                            statusMessage = categoryColumns;
                        }
                        else
                        {
                            statusCode = (int)HttpStatusCode.BadRequest;
                            statusMessage = ResponseMessages.CategoryParamRecordsNotFound;
                        }
                    }
                    else
                    {
                        statusCode = (int)HttpStatusCode.BadRequest;
                        statusMessage = ResponseMessages.CategoryParamRecordsNotFound;
                    }
                }
                else
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    statusMessage = ResponseMessages.ValidationError;
                }                
            }
            catch (Exception ex)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                statusMessage = ResponseMessages.InternalServerError;
                LambdaLogger.Log("\nStackStrace: " + ex.StackTrace);
                LambdaLogger.Log("\nMessage: " + ex.Message + "\n");
            }
            response.StatusCode = statusCode;           
            response.Body = JsonConvert.SerializeObject(statusMessage);
            return response;
        }

        /// <summary>
        /// A Lambda function that get search operators
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> GetSearchOperators(APIGatewayProxyRequest request, ILambdaContext context)
        {           
            var response = new APIGatewayProxyResponse();
            response.Headers = Constants.GetResponseHeaders();
            int statusCode = (int)HttpStatusCode.OK;
            object statusMessage = string.Empty;
            var model = JsonConvert.DeserializeObject<SearchOperatorsModels>(request?.Body);
            try
            {
                if (model != null)
                {
                    string validationMsg = string.Empty;
                    if (!Constants.ValidateGetSearchOperators(model, out validationMsg))
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.Body = JsonConvert.SerializeObject(validationMsg);
                        return response;
                    }
                    if (!await _userService.CheckUserAuth(model.UserCode))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        response.Body = JsonConvert.SerializeObject(ResponseMessages.UserAuthError);
                        return response;
                    }
                    var searchOpertorsList = await _searchServices.GetSearchOperators(model);
                    if (searchOpertorsList != null)
                    {
                        statusCode = (int)HttpStatusCode.OK;
                        statusMessage = searchOpertorsList;
                    }
                    else
                    {
                        statusCode = (int)HttpStatusCode.NotFound;
                        statusMessage = ResponseMessages.SearchOperatorsRecordsNotFound;
                    }
                }
                else
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    statusMessage = ResponseMessages.ValidationError;
                }
            }
            catch (Exception ex)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                statusMessage = ResponseMessages.InternalServerError;
                LambdaLogger.Log("\nStackStrace: " + ex.StackTrace);
                LambdaLogger.Log("\nMessage: " + ex.Message + "\n");
            }
            response.StatusCode = statusCode;
            response.Body = JsonConvert.SerializeObject(statusMessage);
            return response;
        }

        /// <summary>
        /// A Lambda function that get search results
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns> 
        public async Task<APIGatewayProxyResponse> GetSearchResults(APIGatewayProxyRequest request, ILambdaContext context)
        {           
            var response = new APIGatewayProxyResponse();
            response.Headers = Constants.GetResponseHeaders();
            int statusCode = (int)HttpStatusCode.OK;
            object statusMessage = string.Empty;
            var model = JsonConvert.DeserializeObject<SearchResultsModels>(request?.Body);
            try
            {
                if (model != null)
                {
                    string validationMsg = string.Empty;
                    if (!Constants.ValidateGetSearchResults(model, out validationMsg))
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.Body = JsonConvert.SerializeObject(validationMsg);
                        return response;
                    }
                    if (!await _userService.CheckUserAuth(model.UserCode))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        response.Body = JsonConvert.SerializeObject(ResponseMessages.UserAuthError);
                        return response;
                    }
                    var searchResults = await _searchServices.GetSearchResults(model);
                    if (searchResults != null)
                    {
                        statusCode = (int)HttpStatusCode.OK;
                        statusMessage = searchResults;
                    }
                    else
                    {
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        statusMessage = ResponseMessages.InternalServerError;
                    }
                }
                else
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    statusMessage = ResponseMessages.ValidationError;
                }
            }
            catch (Exception ex)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                statusMessage = ResponseMessages.InternalServerError;
                LambdaLogger.Log("\nStackStrace: " + ex.StackTrace);
                LambdaLogger.Log("\nMessage: " + ex.Message + "\n");
            }
            response.StatusCode = statusCode;
            response.Body = JsonConvert.SerializeObject(statusMessage);
            return response;
        }
               
        /// <summary>
        /// A Lambda function that get download document urls.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns> 
        public async Task<APIGatewayProxyResponse> DownloadDocument(APIGatewayProxyRequest request, ILambdaContext context)
        {           
            var response = new APIGatewayProxyResponse();
            response.Headers = Constants.GetResponseHeaders();
            int statusCode = (int)HttpStatusCode.OK;
            object statusMessage = string.Empty;
            var model = JsonConvert.DeserializeObject<DownloadDocumentModels>(request?.Body);
            try
            {
                if (model != null)
                {
                    string validationMsg = string.Empty;
                    if (!Constants.ValidateDownloadDocument(model, out validationMsg))
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.Body = JsonConvert.SerializeObject(validationMsg);
                        return response;
                    }
                    if (!await _userService.CheckUserAuth(model.UserCode))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        response.Body = JsonConvert.SerializeObject(ResponseMessages.UserAuthError);
                        return response;
                    }
                    var documentList = await _uploadService.DownloadDocument(model);
                    if (documentList != null)
                    {
                        if (documentList.Count > 0)
                        {
                            statusCode = (int)HttpStatusCode.OK;
                            statusMessage = documentList;
                        }
                        else
                        {
                            statusCode = (int)HttpStatusCode.NotFound;
                            statusMessage = ResponseMessages.DocumentDownloadRecordsNotFound;
                        }
                    }
                    else
                    {
                        statusCode = (int)HttpStatusCode.NotFound;
                        statusMessage = ResponseMessages.DocumentDownloadRecordsNotFound;
                    }
                }
                else
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    statusMessage = ResponseMessages.ValidationError;
                }
            }
            catch (Exception ex)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                statusMessage = ResponseMessages.InternalServerError;
                LambdaLogger.Log("\nStackStrace: " + ex.StackTrace);
                LambdaLogger.Log("\nMessage: " + ex.Message + "\n");
            }
            response.StatusCode = statusCode;
            response.Body = JsonConvert.SerializeObject(statusMessage);
            return response;
        }
               
        /// <summary>
        /// A Lambda function that download document by document id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> DownloadDocumentById(APIGatewayProxyRequest request, ILambdaContext context)
        {          
            var response = new APIGatewayProxyResponse();
            response.Headers = Constants.GetResponseHeaders();
            int statusCode = (int)HttpStatusCode.OK;
            object statusMessage = string.Empty;
            var model = JsonConvert.DeserializeObject<RiskTechDocumentByIdModel>(request?.Body);
            try
            {
                if (model != null)
                {
                    string validationMsg = string.Empty;
                    if (!Constants.ValidateDownloadDocumentById(model, out validationMsg))
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.Body = JsonConvert.SerializeObject(validationMsg);
                        return response;
                    }
                    if (!await _userService.CheckUserAuth(model.UserCode))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        response.Body = JsonConvert.SerializeObject(ResponseMessages.UserAuthError);
                        return response;
                    }
                    var documentDetails = await _uploadService.DownloadDocumentsByDocId(model);
                    if (documentDetails != null)
                    {
                        statusCode = (int)HttpStatusCode.OK;
                        statusMessage = documentDetails;
                    }
                    else
                    {
                        statusCode = (int)HttpStatusCode.BadRequest;
                        statusMessage = ResponseMessages.DocumentDownloadRecordsNotFound;
                    }
                }
                else
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    statusMessage = ResponseMessages.ValidationError;
                }
            }
            catch (Exception ex)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                statusMessage = ResponseMessages.InternalServerError;
                LambdaLogger.Log("\nStackStrace: " + ex.StackTrace);
                LambdaLogger.Log("\nMessage: " + ex.Message + "\n");
            }
            response.StatusCode = statusCode;
            response.Body = JsonConvert.SerializeObject(statusMessage);
            return response;
        }

        /// <summary>
        /// A Lambda function that get download file details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns> 
        public async Task<APIGatewayProxyResponse> GetFileDownloadDetails(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var response = new APIGatewayProxyResponse();
            response.Headers = Constants.GetResponseHeaders();
            int statusCode = (int)HttpStatusCode.OK;
            object statusMessage = string.Empty;
            var model = JsonConvert.DeserializeObject<FileDownloadDetailsModels>(request?.Body);
            try
            {
                if (model != null)
                {
                    string validationMsg = string.Empty;
                    if (!Constants.ValidateGetFileDownloadDetails(model, out validationMsg))
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.Body = JsonConvert.SerializeObject(validationMsg);
                        return response;
                    }
                    if (!_userService.CheckUserAuth(model.UserCode).GetAwaiter().GetResult())
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        response.Body = JsonConvert.SerializeObject(ResponseMessages.UserAuthError);
                        return response;
                    }
                    var fileDetails = await _uploadService.GetFileDownloadDetails(model);

                    if (fileDetails != null)
                    {
                        statusCode = (int)HttpStatusCode.OK;
                        statusMessage = fileDetails;                       
                    }
                    else
                    {
                        statusCode = (int)HttpStatusCode.NotFound;
                        statusMessage = ResponseMessages.FileDownlodDetailsNotFound;
                    }
                }
                else
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    statusMessage = ResponseMessages.ValidationError;
                }
            }
            catch (Exception ex)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                statusMessage = ResponseMessages.InternalServerError;
                LambdaLogger.Log("\nStackStrace: " + ex.StackTrace);
                LambdaLogger.Log("\nMessage: " + ex.Message + "\n");
            }
            response.StatusCode = statusCode;
            response.Body = JsonConvert.SerializeObject(statusMessage);
            return response;
        }

        /// <summary>
        /// A Lambda function that download document zip file
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns> 
        public async Task<APIGatewayProxyResponse> DownloadDocumentZIP(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var response = new APIGatewayProxyResponse();
            response.Headers = Constants.GetResponseHeaders();
            int statusCode = (int)HttpStatusCode.OK;
            object statusMessage = string.Empty;        
            var model = JsonConvert.DeserializeObject<DownloadDocumentZIPModels>(request?.Body);
            try
            {
                if (model != null)
                {
                    string validationMsg = string.Empty;
                    if (!Constants.ValidateDownloadDocumentZIP(model, out validationMsg))
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.Body = JsonConvert.SerializeObject(validationMsg);
                        return response;
                    }
                    if (!_userService.CheckUserAuth(model.UserCode).GetAwaiter().GetResult())
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        response.Body = JsonConvert.SerializeObject(ResponseMessages.UserAuthError);
                        return response;
                    }
                    if (model.DownloadMethod == Constants.EmailDownload)
                    {
                        using (AmazonLambdaClient client = new AmazonLambdaClient(Constants.GetRegion()))
                        {
                            response.StatusCode = 200;
                            response.Body = JsonConvert.SerializeObject(model);
                            var iRequest = new Amazon.Lambda.Model.InvokeRequest
                            {
                                FunctionName = EnvVeriables.EmailFunction,
                                Payload = JsonConvert.SerializeObject(response),
                                InvocationType = Amazon.Lambda.InvocationType.Event
                            };
                            client.InvokeAsync(iRequest).GetAwaiter().GetResult();
                        }
                        EmailFileModels fileModels = new EmailFileModels();
                        fileModels.EmailZip = true;
                        statusCode = (int)HttpStatusCode.OK;
                        statusMessage = fileModels;
                    }
                    else
                    {
                        var fileModel = await _uploadService.DownloadDocumentZip(model);
                        if (fileModel != null)
                        {
                            statusCode = (int)HttpStatusCode.OK;
                            statusMessage = fileModel;
                        }
                        else
                        {
                            statusCode = (int)HttpStatusCode.NotFound;
                            statusMessage = ResponseMessages.DocumentDownloadRecordsNotFound;
                        }
                    }    
                }
                else
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                    statusMessage = ResponseMessages.ValidationError;
                }
            }
            catch (Exception ex)
            {               
                statusCode = (int)HttpStatusCode.InternalServerError;
                statusMessage = ResponseMessages.InternalServerError;
                LambdaLogger.Log("\nStackStrace: " + ex.StackTrace);
                LambdaLogger.Log("\nMessage: " + ex.Message + "\n");               
            }
            response.StatusCode = statusCode;
            response.Body = JsonConvert.SerializeObject(statusMessage);
            return response;
        }

        /// <summary>
        /// A Lambda function that generate the zip and email
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns> 
        public async Task EmailDocumentZIP(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var response = new APIGatewayProxyResponse();
            response.Headers = Constants.GetResponseHeaders();
            object statusMessage = string.Empty;
            SendEmailModel emailModel = null;
            try
            {
                var model = JsonConvert.DeserializeObject<DownloadDocumentModels>(request?.Body);
                var validationMsg = string.Empty;
                if (model != null)
                {
                    emailModel = await _uploadService.EmailDocumentZIP(model);

                    using (AmazonLambdaClient client = new AmazonLambdaClient(Constants.GetRegion()))
                    {
                        response.StatusCode = 200;
                        response.Body = JsonConvert.SerializeObject(emailModel);
                        var iRequest = new Amazon.Lambda.Model.InvokeRequest
                        {
                            FunctionName = EnvVeriables.SendEmailFunction,
                            Payload = JsonConvert.SerializeObject(response)
                        };
                        client.InvokeAsync(iRequest).GetAwaiter().GetResult();
                    }

                    LambdaLogger.Log("\nMessage: " + emailModel.ToEmail + " - emailed documents successfully!\n");
                }
                else
                {
                    LambdaLogger.Log("\nMessage: " + ResponseMessages.ValidationError + "\n");
                }
            }
            catch (Exception ex)
            {
                LambdaLogger.Log("\nMessage: Email Failed. Error:  " + ex.Message + "\n");
            }

            //return response;
        }

        /// <summary>
        /// A Lambda function that send email
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public void SendEmail(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                LambdaLogger.Log("\nMessage: SendEmail - Invoked! \n");
                var model = JsonConvert.DeserializeObject<SendEmailModel>(request?.Body);
                EmailClient emailClient = new EmailClient();
                emailClient.SendEmail(model);
                LambdaLogger.Log("\nMessage: SendEmail - Sent! \n");
            }
            catch (Exception ex)
            {
                LambdaLogger.Log("\nMessage: SendEmail - " + ex.StackTrace + "\n");
            }
        }

        #region Upload functionality API
        ///// <summary>
        ///// a lambda function that add new document
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns> 
        //public async task<apigatewayproxyresponse> uploaddocumentdetails(apigatewayproxyrequest request, ilambdacontext context)
        //{
        //    var response = new apigatewayproxyresponse();
        //    response.headers = constants.getresponseheaders();
        //    int statuscode = 200;
        //    object statusmessage = string.empty;
        //    var model = jsonconvert.deserializeobject<uploaddocumentdetailsmodels>(request?.body);
        //    try
        //    {
        //        if (model != null)
        //        {
        //            string validationmsg = string.empty;
        //            if (!constants.validateuploaddocumentdetails(model, out validationmsg))
        //            {
        //                response.statuscode = (int)httpstatuscode.badrequest;
        //                response.body = jsonconvert.serializeobject(validationmsg);
        //                return response;
        //            }
        //            if (!await _userservice.checkuserauth(model.usercode))
        //            {
        //                response.statuscode = (int)httpstatuscode.forbidden;
        //                response.body = jsonconvert.serializeobject(responsemessages.userautherror);
        //                return response;
        //            }
        //            var documentdetails = await _uploadservice.uploaddocumentdetails(model);
        //            if (documentdetails != null)
        //            {
        //                statuscode = (int)httpstatuscode.ok;
        //                statusmessage = documentdetails;
        //            }
        //            else
        //            {
        //                statuscode = (int)httpstatuscode.badgateway;
        //                statusmessage = responsemessages.uploaddocumenterror;
        //            }
        //        }
        //        else
        //        {
        //            statuscode = (int)httpstatuscode.badrequest;
        //            statusmessage = responsemessages.validationerror;
        //        }
        //    }
        //    catch (exception ex)
        //    {
        //        statuscode = (int)httpstatuscode.internalservererror;
        //        statusmessage = responsemessages.internalservererror;
        //        lambdalogger.log("\nstackstrace: " + ex.stacktrace);
        //        lambdalogger.log("\nmessage: " + ex.message + "\n");
        //    }
        //    response.statuscode = statuscode;
        //    response.body = jsonconvert.serializeobject(statusmessage);
        //    return response;
        //}

        ///// <summary>
        ///// a lambda function that update document success
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns> 
        //public async task<apigatewayproxyresponse> updatedocumentsuccess(apigatewayproxyrequest request, ilambdacontext context)
        //{
        //    var response = new apigatewayproxyresponse();
        //    response.headers = constants.getresponseheaders();
        //    int statuscode = 200;
        //    object statusmessage = string.empty;
        //    var model = jsonconvert.deserializeobject<uploadsuccessmodels>(request?.body);
        //    try
        //    {
        //        if (model != null)
        //        {
        //            string validationmsg = string.empty;
        //            if (!constants.validateupdatedocumentsuccess(model, out validationmsg))
        //            {
        //                response.statuscode = (int)httpstatuscode.badrequest;
        //                response.body = jsonconvert.serializeobject(validationmsg);
        //                return response;
        //            }
        //            if (!await _userservice.checkuserauth(model.usercode))
        //            {
        //                response.statuscode = (int)httpstatuscode.forbidden;
        //                response.body = jsonconvert.serializeobject(responsemessages.userautherror);
        //                return response;
        //            }
        //            bool issuccess = await _uploadservice.updatedocumentsuccess(model);
        //            if (issuccess)
        //            {
        //                statuscode = (int)httpstatuscode.ok;
        //                statusmessage = issuccess;
        //            }
        //            else
        //            {
        //                statuscode = (int)httpstatuscode.badgateway;
        //                statusmessage = responsemessages.updateuploaderror;
        //            }
        //        }
        //        else
        //        {
        //            statuscode = (int)httpstatuscode.badrequest;
        //            statusmessage = responsemessages.validationerror;
        //        }
        //    }
        //    catch (exception ex)
        //    {
        //        statuscode = (int)httpstatuscode.internalservererror;
        //        statusmessage = responsemessages.internalservererror;
        //        lambdalogger.log("\nstackstrace: " + ex.stacktrace);
        //        lambdalogger.log("\nmessage: " + ex.message + "\n");
        //    }
        //    response.statuscode = statuscode;
        //    response.body = jsonconvert.serializeobject(statusmessage);
        //    return response;
        //}
        #endregion
    }
}
