using Capstone.Common.DTOs.GoogleAPI;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Service.AttachmentServices
{
    public interface IAttachmentServices
    {
        Task<string> UploadFileAsync(IFormFile file);
        Stream DownloadFile(string id);
        Task<List<GoogleDriveFiles>> ListFiles();
        Task<bool> RemoveAttachment(string id);
        Task<bool> DeleteAttachment(string id);

    }
}
