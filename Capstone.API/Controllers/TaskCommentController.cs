using Capstone.API.Extentions;
using Capstone.Common.DTOs.Comments;
using Capstone.Common.DTOs.TicketComment;
using Capstone.Service.TicketCommentService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Capstone.API.Controllers
{
    [Route("api/comment-management")]
    [ApiController]
    public class TaskCommentController : ControllerBase
    {
        private readonly ITaskCommentService _commentService;

        public TaskCommentController(ITaskCommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("comment")]
        public async Task<IActionResult> CreateComment(CreateCommentRequest comment)
        {
            var userId = this.GetCurrentLoginUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("You need to login first");
            }
            var newComment = await _commentService.CreateComment(userId, comment);
            if (newComment == null)
            {
                return BadRequest("Failed to create a new comment.");
            }

            return Ok(newComment);
        }

        //  2C5354D7-B5A0-487A-AD0D-E380A0592021 - Delete Own Comments
        //  4340744E-E232-4509-A792-F1FC1554D4B1 - Delete All Comments
        [HttpDelete("comment/{commentId}")]
        public async Task<IActionResult> RemoveComment(Guid commentId)
        {
            var success = await _commentService.RemoveComment(commentId);
            if (!success)
            {
                return NotFound("Comment not found or Unable to delete.");
            }

            return Ok("Comment deleted.");
        }


        [HttpPost("comment/reply/{commentId}")]
        public async Task<IActionResult> CreateComment(Guid commentId, ReplyCommentRequest comment)
        {
            var userId = this.GetCurrentLoginUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized("You need to login first");
            }
            var newComment = await _commentService.ReplyComment(commentId, userId, comment);
            if (newComment == null)
            {
                return BadRequest("Failed to create a new comment.");
            }

            return Ok(newComment);
        }

        [EnableQuery]
        [HttpGet("comment/{taskId}")]
        public async Task<IActionResult> GetAllCommentByTaskID(Guid taskId)
        {
            var comments = await _commentService.GetAllCommentByTaskID(taskId);
            return Ok(comments);
        }
        //1 C922CC8A-F023-45B8-B656-260B4463A351 - Edit Own Comments
        // 76A0B105-C0C2-4290-B264-D8FE2699C69C - Edit All Comments
        [HttpPut("comment")]
        public async Task<IActionResult> UpdateComment( ReplyCommentRequest updatedComment)
        {
            var comment = await _commentService.CheckExist(updatedComment.CommentId);
            if (!comment)
            {
                return NotFound("Comment not exist");
            }
            var updated = await _commentService.UpdateComment(updatedComment.CommentId, updatedComment);
            if (updated == null)
            {
                return NotFound("Comment not found or Unable to update.");
            }

            return Ok(updated);
        }
    }
}
