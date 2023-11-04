using Capstone.Service.AttachmentServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentServices _attachmentServices;

        public AttachmentController(IAttachmentServices attachmentServices)
        {
            _attachmentServices = attachmentServices;
        }

        [HttpGet("attachments")]
        public async Task<IActionResult> List()
        {
            var files = await _attachmentServices.GetDriveFiles();
            return Ok(files);
        }
        [HttpPost("attachments")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var fileId = await _attachmentServices.UploadFileAsync(file);
            return Ok(fileId);
        }

        [HttpGet("attachments/{id}/download")]
        public IActionResult DownloadFile(string id)
        {
            var stream = _attachmentServices.DownloadFile(id);
            return File(stream, "application/octet-stream");
        }

        [HttpDelete("attachments/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _attachmentServices.DeleteAttachment(id);
            return NoContent();
        }
    }
}
