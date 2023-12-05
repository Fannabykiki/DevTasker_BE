using Azure.Storage;
using Azure.Storage.Blobs;
using Capstone.Common.DTOs.Attachment;
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
		private readonly BlobServiceClient _blobService;
		private readonly IAttachmentRepository _attachmentRepository;
		private readonly IProjectMemberRepository _projectMemberRepository;
		private readonly IInterationRepository _interationRepository;
		private readonly ITaskRepository _taskRepository;


		public AzureBlobService(IAttachmentRepository attachmentRepository, IProjectMemberRepository projectMemberRepository, IInterationRepository interationRepository, ITaskRepository taskRepository)
		{
			var credential = new StorageSharedKeyCredential(_storageAccount, _accessKey);
			var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
			var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
			_blobServiceClient = blobServiceClient.GetBlobContainerClient("files");
			_attachmentRepository = attachmentRepository;
			_projectMemberRepository = projectMemberRepository;
			_interationRepository = interationRepository;
			_blobService = new BlobServiceClient(new Uri(blobUri), credential);
			_taskRepository = taskRepository;
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

		public async Task<BlobResponse> UploadFile(Guid userId, IFormFile file, Guid taskId)
		{
			using var transaction = _attachmentRepository.DatabaseTransaction();
			try
			{
				var task = await _taskRepository.GetAsync(x => x.TaskId == taskId, null);
				var containerClient = _blobService.GetBlobContainerClient(task.TaskId.ToString().Trim().ToLower());
				if (!await containerClient.ExistsAsync())
				{
					await containerClient.CreateAsync();
				}

				var interation = await _taskRepository.GetAsync(x => x.TaskId == taskId, null);
				var project = await _interationRepository.GetAsync(i => i.InterationId == interation.InterationId, null);
				var member = await _projectMemberRepository.GetAsync(x => x.UserId == userId && x.ProjectId == project.BoardId, null);
				BlobClient client = containerClient.GetBlobClient(file.FileName);

				var allowedFileCode = new[] { ".jpg", ".png", ".pdf", ".doc", ".xls", ".xlsx", ".csv", ".txt", ".doc", ".zip", ".rar", ".docx", ".ppt", ".pptx", ".gif", ".jpeg", ".mov", ".mp4", ".xml", ".cs", ".json", ".html", ".sql" };

				var extension = Path.GetExtension(file.FileName);
				if (!allowedFileCode.Contains(extension))
				{
					return new BlobResponse
					{
						IsSucceed = false,
						Message = "File format not allowed"
					};
				}

				var fileSize = file.Length;
				if (fileSize > 10 * 1024 * 1024)
				{
					return new BlobResponse
					{
						IsSucceed = false,
						Message = "Please choose attachment smaller than 10MB !!!"
					};
				}

				if (await client.ExistsAsync())
				{
					var filenameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
					var fileExtension = Path.GetExtension(file.FileName);
					var countAttachments = (await _attachmentRepository.GetAllWithOdata(x => x.Title.StartsWith(filenameWithoutExtension) && x.TaskId == taskId && x.Title.EndsWith(fileExtension), null)).Count();
					var newFileName = $"{filenameWithoutExtension}({countAttachments}){fileExtension}";
					BlobClient _client = containerClient.GetBlobClient(newFileName);

					await using (Stream? data = file.OpenReadStream())
					{

						var newAttachment = new Attachment
						{
							AttachmentId = Guid.NewGuid(),
							TaskId = taskId,
							CreateAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
							CreateBy = member.MemberId,
							Title = newFileName,
							IsDeleted = false
						};

						await _attachmentRepository.CreateAsync(newAttachment);
						await _attachmentRepository.SaveChanges();
						transaction.Commit();
						await _client.UploadAsync(data);
					}

					return new BlobResponse
					{
						Message = $"Attachment {newFileName} Upload successfully",
						IsSucceed = true,
						Blob = new BlobViewModel
						{
							Uri = client.Uri.AbsoluteUri,
							Name = client.Name,
						}
					};
				}

				await using (Stream? data = file.OpenReadStream())
				{
					var newAttachment = new Attachment
					{
						AttachmentId = Guid.NewGuid(),
						TaskId = taskId,
						CreateAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						CreateBy = member.MemberId,
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
			finally
			{
				transaction.Dispose();
			}

		}

		public async Task<BlobViewModel> DownLoadFile(string fileName, Guid taskId)
		{
			var task = await _taskRepository.GetAsync(x => x.TaskId == taskId, null);
			var containerClient = _blobService.GetBlobContainerClient(task.TaskId.ToString().Trim().ToLower());
			if (!await containerClient.ExistsAsync())
			{
				return null;
			}

			BlobClient file = containerClient.GetBlobClient(fileName);

			if (await file.ExistsAsync())
			{
				var data = await file.OpenReadAsync();
				Stream blobContent = data;

				var content = await file.DownloadContentAsync();

				string name = fileName;
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

		public async Task<BlobResponse> DeleteFile(string blobFile, Guid taskId)
		{
			using var transaction = _attachmentRepository.DatabaseTransaction();
			try
			{
				var task = await _taskRepository.GetAsync(x => x.TaskId == taskId, null);
				var containerClient = _blobService.GetBlobContainerClient(task.TaskId.ToString().Trim().ToLower());
				if (!await containerClient.ExistsAsync())
				{
					await containerClient.CreateAsync();
				}
				BlobClient file = containerClient.GetBlobClient(blobFile);
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
