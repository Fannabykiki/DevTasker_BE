using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Capstone.Common.DTOs.GoogleAPI;
using Capstone.DataAccess.Repository.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Capstone.Service.AttachmentServices
{
    public class AttachmentServices : IAttachmentServices
    {
        private readonly DriveService _driveService;
        private readonly IAttachmentRepository _attachmentRepository;
        //public static string[] Scopes = { DriveService.Scope.Drive };
        public static string[] Scopes = { Google.Apis.Drive.v3.DriveService.Scope.Drive };

        public static DriveService GetDriveService()
        {
            //get Credentials from client_secret.json file 
            UserCredential credential;
            //Root Folder of project
            string jsonPath = "config/client_secret.json";
            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("Drive.Auth.Store")).Result;
            }
            //create Drive API service.
            DriveService service = new Google.Apis.Drive.v3.DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "devtasker",
            });
            return service;
        }
        public AttachmentServices(IAttachmentRepository attachmentRepository)
        {
            _driveService = GetDriveService(); 
            _attachmentRepository = attachmentRepository;
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

        public async Task< List<GoogleDriveFile>> GetDriveFiles()
        {
            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = _driveService.Files.List();

            // for getting folders only.
            //FileListRequest.Q = "mimeType='application/vnd.google-apps.folder'";
            FileListRequest.Fields = "nextPageToken, files(*)";
            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFile> FileList = new List<GoogleDriveFile>();
            // For getting only folders
            // files = files.Where(x => x.MimeType == "application/vnd.google-apps.folder").ToList();
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    GoogleDriveFile File = new GoogleDriveFile
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        Version = file.Version,
                        CreatedTime = file.CreatedTime,
                        Parents = file.Parents,
                        MimeType = file.MimeType
                    };
                    FileList.Add(File);
                }
            }
            return FileList;
        }
        //public async Task<List<GoogleDriveFiles>> ListFiles()
        //{
        //    FilesResource.ListRequest FileListRequest = _driveService.Files.List();

        //    FileListRequest.Fields = "nextPageToken, files(id, name, size, version, createdTime)";

        //    IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;

        //    var flieList = files.Select(x => new GoogleDriveFiles
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        Size = x.Size,
        //        Version = x.Version,
        //        CreatedTime = x.CreatedTime
        //    });

        //    return (List<GoogleDriveFiles>)flieList;
        //}

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
