using Capstone.Common.DTOs.Comments;
using Capstone.Service.TicketCommentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketCommentController : ControllerBase
    {
        private readonly ITicketCommentService _commentService;

        public TicketCommentController(ITicketCommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("/comment")]
        public async Task<IActionResult> CreateComment(CreateCommentRequest comment)
        {
            var newComment = await _commentService.CreateComment(comment);
            if (newComment == null)
            {
                return BadRequest("Failed to create a new comment.");
            }

            return Ok(newComment);
        }

        [HttpDelete("/comment/{commentId}")]
        public async Task<IActionResult> RemoveComment(Guid commentId)
        {
            var success = await _commentService.RemoveComment(commentId);
            if (!success)
            {
                return NotFound("Comment not found or unable to delete.");
            }

            return Ok("Comment deleted.");
        }

        [HttpGet("/comment/{ticketId}")]
        public async Task<IActionResult> GetAllCommentByTaskID(Guid ticketId)
        {
            var comments = await _commentService.GetAllCommentByTaskID(ticketId);
            return Ok(comments);
        }

        [HttpPut("/comment/{commentId}")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] CreateCommentRequest updatedComment)
        {
            var updated = await _commentService.UpdateComment(id, updatedComment);
            if (updated == null)
            {
                return NotFound("Comment not found or unable to update.");
            }

            return Ok(updated);
        }
    }
}
