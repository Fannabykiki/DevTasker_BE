using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Capstone.Common.DTOs.GoogleAPI;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Capstone.Service.AttachmentServices
{
    public class AttachmentServices : IAttachmentServices
    {
        private readonly DriveService _driveService;
        public static string[] Scopes = { DriveService.Scope.Drive };

        public static DriveService GetDriveService()
        {
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = "421380454099-dshij1rr3m1csp98vucu6mnb05fv7ee8.apps.googleusercontent.com",
                    ClientSecret = "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"
                },
                new[] { DriveService.Scope.Drive },
                "user",
                CancellationToken.None);
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = (Google.Apis.Http.IConfigurableHttpClientInitializer)credential,
                // ...
            });
        }
        public AttachmentServices()
        {
            _driveService = GetDriveService(); 
        }
        public async Task<bool> DeleteAttachment(string id)
        {
            try
            {
                await _driveService.Files.Delete(id).ExecuteAsync();
                // Impliment to remove file information from data base

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occurred: " + ex.Message);
                return false;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = file.FileName
            };

            var request = _driveService.Files.Create(fileMetadata, file.OpenReadStream(), file.ContentType);
            var result = await request.UploadAsync();
            // Get file by name to retrieve ID
            var fileUploaded = await _driveService.Files.Get(file.Name).ExecuteAsync();
            // Impliment to save file information to data base



            return fileUploaded.Id;
        }

        public Stream DownloadFile(string id)
        {
            var request = _driveService.Files.Get(id);
            var stream = new MemoryStream();
            request.Download(stream);
            return stream;
        }

        public async Task<List<GoogleDriveFiles>> ListFiles()
        {
            FilesResource.ListRequest FileListRequest = _driveService.Files.List();

            FileListRequest.Fields = "nextPageToken, files(id, name, size, version, createdTime)";

            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;

            var flieList = files.Select(x => new GoogleDriveFiles
            {
                Id = x.Id,
                Name = x.Name,
                Size = x.Size,
                Version = x.Version,
                CreatedTime = x.CreatedTime
            });

            return (List<GoogleDriveFiles>)flieList;
        }

        public Task<bool> RemoveAttachment(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UploadAttachment(IFormFile file)
        {
            //var file = Request.Form.Files[0];
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = file.FileName
            };
            var request = _driveService.Files.Create(fileMetadata, file.OpenReadStream(), file.ContentType);
            var result = await request.UploadAsync();

            return true;
        }
    }
}
