using Capstone.API.Extentions;
using Capstone.Service.BlobStorage;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.API.Controllers
{
	[Route("api/attachment-management")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly AzureBlobService _azureBlobService;

        public AttachmentController(AzureBlobService azureBlobService)
        {
			_azureBlobService = azureBlobService;
        }

        [HttpGet("attachments")]
        public async Task<IActionResult> ListAllBlobs()
        {
            var result = await _azureBlobService.ListAllBlob();
            return Ok();
        }

        [HttpPost("attachments")]
        public async Task<IActionResult> UploadFile(IFormFile file,Guid taskId)
        {
            var userId = this.GetCurrentLoginUserId();
            if (userId == Guid.Empty)
            {
                return BadRequest("You need to login first");
            }
            var files = await _azureBlobService.UploadFile(userId, file, taskId);
            return Ok(files);
        }

        [HttpGet("attachments/download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var file = await  _azureBlobService.DownLoadFile(fileName);
            if(file == null)
            {
                return NotFound("File not exist");
            }
            return File(file.Content,file.ContentType,file.Name);
        }

        [HttpDelete("attachments/{fileName}")]
        public async Task<IActionResult> Delete(string fileName)
        {
			var file = await _azureBlobService.DeleteFile(fileName);
			return Ok(file);
        }
    }
}
