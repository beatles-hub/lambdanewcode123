using LDM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LDM.Services
{
    public interface IUploadServices
    {
        Task<DocumentDetailsModels> UploadDocumentDetails(UploadDocumentDetailsModels uploadDocumentDetails);
        Task<bool> UpdateDocumentSuccess(UploadSuccessModels model);      
        Task<List<DownloadDocumentDetailsModels>> DownloadDocument(DownloadDocumentModels model);       
        Task<List<RiskTechDocumentByIdResultModel>> DownloadDocumentsByDocId(RiskTechDocumentByIdModel model);
        Task<FileDownloadDetails> GetFileDownloadDetails(FileDownloadDetailsModels model);
        Task<EmailFileModels> DownloadDocumentZip(DownloadDocumentZIPModels model);
        Task<SendEmailModel> EmailDocumentZIP(DownloadDocumentModels model);      
    }
}
