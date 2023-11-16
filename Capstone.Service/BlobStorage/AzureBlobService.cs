using Azure.Storage;
using Azure.Storage.Blobs;
using Capstone.Common.DTOs.BlobAzure;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Reflection.Metadata;

namespace Capstone.Service.BlobStorage
{
	public class AzureBlobService
	{
		private readonly string _storageAccount = "devtasker";
		private readonly string _accessKey = "N2vam2LLK2IusA3BDTUbeIkjy8kR0eqyHPG8g467C1L1mu29I5RCBhWA8MTXbxpREhqCjQ7n3czQ+AStBFDgKg==";
		private readonly BlobContainerClient _blobServiceClient;
		private readonly IAttachmentRepository _attachmentRepository;

		public AzureBlobService(IAttachmentRepository attachmentRepository)
		{
			var credential = new StorageSharedKeyCredential(_storageAccount, _accessKey);
			var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
			var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
			_blobServiceClient = blobServiceClient.GetBlobContainerClient("files");
			_attachmentRepository = attachmentRepository;
		}

		public async Task<List<BlobViewModel>> ListAllBlob()
		{
			List<BlobViewModel> files = new List<BlobViewModel>();

			await foreach (var file in _blobServiceClient.GetBlobsAsync())
			{
				string uri = _blobServiceClient.Uri.ToString();
				var name = file.Name;
				var fullUri = $"{uri}/{name}";

				files.Add(new BlobViewModel
				{
					Uri = fullUri,
					Name = name,
					ContentType = file.Properties.ContentType,
				});
			}
			return files;
		}

		public async Task<BlobResponse> UploadFile(Guid userId, IFormFile file, Guid commentId)
		{
			using var transaction = _attachmentRepository.DatabaseTransaction();
			try
			{
				BlobClient client = _blobServiceClient.GetBlobClient(file.FileName);
				var fileSize = file.Length;
				if(fileSize > 20 * 1024 * 1024)
				{
					return new BlobResponse
					{
						IsSucceed = false,
						Message = "Please choose attachment smaller than 20MB !!!"
					};
				}
				if (await client.ExistsAsync())
				{
					return new BlobResponse
					{
						IsSucceed = false,
						Message = "Attachment is exist. Please rename and try again!!!"
					};
				}
				await using (Stream? data = file.OpenReadStream())
				{	
					var newAttachment = new Attachment
					{
						AttachmentId = Guid.NewGuid(),
						CommentId = commentId,
						CreateAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						CreateBy = userId,
						Title = file.FileName,
						IsDeleted = false
					};
					await _attachmentRepository.CreateAsync(newAttachment);
					await _attachmentRepository.SaveChanges();
					transaction.Commit();
					await client.UploadAsync(data);
				}

				return new BlobResponse
				{
					Message = $"Attachment {file.FileName} Upload successfully",
					IsSucceed = true,
					Blob = new BlobViewModel
					{
						Uri = client.Uri.AbsoluteUri,
						Name = client.Name,
					}
				};
			}
			catch
			{
				transaction.RollBack();
				return new BlobResponse
				{
					IsSucceed = false,
					Message = "Upload Attachment fail"
				};
			}

		}

		public async Task<BlobViewModel> DownLoadFile(string blobFile)
		{
			BlobClient file = _blobServiceClient.GetBlobClient(blobFile);

			if (await file.ExistsAsync())
			{
				var data = await file.OpenReadAsync();
				Stream blobContent = data;

				var content = await file.DownloadContentAsync();

				string name = blobFile;
				string contentType = content.Value.Details.ContentType;

				return new BlobViewModel
				{
					Content = blobContent,
					Name = name,
					ContentType = contentType
				};
			}
			return null;
		}

		public async Task<BlobResponse> DeleteFile(string blobFile)
		{
			using var transaction = _attachmentRepository.DatabaseTransaction();
			try
			{
				BlobClient file = _blobServiceClient.GetBlobClient(blobFile);
				if (await file.ExistsAsync())
				{
					await file.DeleteIfExistsAsync();
					var attachment = await _attachmentRepository.GetAsync(x => x.Title.Equals(blobFile), null);
					await _attachmentRepository.DeleteAsync(attachment);
					await _attachmentRepository.SaveChanges();
					transaction.Commit();
				}

				return new BlobResponse
				{
					IsSucceed = true,
					Message = $"Attachment : {blobFile} has been remove successfully!!!"
				};
			}
			catch
			{
				transaction.RollBack();
				return new BlobResponse
				{
					IsSucceed = false,
					Message = $"Attachment:{blobFile} don't exist.Cant delete!!"
				};
			}
		}
	}
}
