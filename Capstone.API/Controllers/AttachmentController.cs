using Capstone.API.Extentions;
using Capstone.API.Extentions.RolePermissionAuthorize;
using Capstone.Common.Constants;
using Capstone.Common.DTOs.Attachment;
using Capstone.Common.DTOs.Task;
using Capstone.Service.BlobStorage;
using Capstone.Service.NotificationService;
using Capstone.Service.TicketService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.API.Controllers
{
	[Route("api/attachment-management")]
	[ApiController]
	public class AttachmentController : ControllerBase
	{
		private readonly AzureBlobService _azureBlobService;
		private readonly ITaskService _taskService;
		private readonly IAuthorizationService _authorizationService;
		private readonly INotificationService _notificationService;
		public AttachmentController(AzureBlobService azureBlobService,
			ITaskService taskService,
			IAuthorizationService authorizationService,
            INotificationService notificationService)
		{
			_azureBlobService = azureBlobService;
			_taskService = taskService;
			_authorizationService = authorizationService;
			_notificationService = notificationService;
		}
		// E291ABC0-C869-4FF0-9E3C-48B74022577D - Create Attachments
		[HttpPost("attachments")]
		public async Task<IActionResult> UploadFile(IFormFileCollection file, Guid taskId)
		{
			//Authorize
			var projectId = await _taskService.GetProjectIdOfTask(taskId);
			var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
				new RolePermissionResource
				{
					ListProjectId = new List<Guid?> { projectId },
					ListPermissionAuthorized = new List<string> { PermissionNameConstant.CreateAttachments }
				}, AuthorizationRequirementNameConstant.RolePermission);
			if (!authorizationResult.Succeeded)
			{
				return Unauthorized(ErrorMessage.InvalidPermission);
			}
			List<string> errorFiles = new List<string>();

			var userId = this.GetCurrentLoginUserId();
			foreach (var fileItem in file)
			{
				await _azureBlobService.UploadFile(userId, fileItem, taskId);
			}
			foreach (var fileItem in file)
			{
				var result = await _azureBlobService.ScanMalware(taskId, fileItem.FileName);
				if (result != null)
				{
					errorFiles.Add(fileItem.FileName);
				}
			}
			if (errorFiles.Count > 0)
			{
				return BadRequest(string.Join(",", errorFiles) + " is malware.Can't upload this attachment");
			}
			else
			{
				await _notificationService.SendNotificationUploadAttachment(taskId, this.GetCurrentLoginUserId());
			}
			return Ok(file);
		}

		[HttpGet("attachments/download/{fileName}/{taskId}")]
		public async Task<IActionResult> DownloadFile(string fileName, Guid taskId)
		{
			var file = await _azureBlobService.DownLoadFile(fileName, taskId);
			if (file == null)
			{
				return NotFound("File not exist");
			}
			return File(file.Content, file.ContentType, file.Name);
		}

		//  FC0A6071-8F8D-4BD5-B365-81FF722CC174 - Delete All Attachments
		[HttpDelete("attachments/{fileName}/{taskId}")]
		public async Task<IActionResult> Delete(string fileName, Guid taskId)
		{

			//Authorize
			var projectId = await _taskService.GetProjectIdOfTask(taskId);
			var authorizationResult = await _authorizationService.AuthorizeAsync(this.HttpContext.User,
				new RolePermissionResource
				{
					ListProjectId = new List<Guid?> { projectId },
					ListPermissionAuthorized = new List<string> { PermissionNameConstant.DeleteAllAttachments }
				}, AuthorizationRequirementNameConstant.RolePermission);
			if (!authorizationResult.Succeeded)
			{
				return Unauthorized(ErrorMessage.InvalidPermission);
			}

			var file = await _azureBlobService.DeleteFile(fileName, taskId);
			if (file != null && file.IsSucceed.Value)
			{
				await _notificationService.SendNotificationDeleteAttachment(taskId, this.GetCurrentLoginUserId());
			}
			return Ok(file);
		}
	}
}
