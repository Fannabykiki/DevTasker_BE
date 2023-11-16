﻿using Capstone.API.Extentions;
using Capstone.Common.DTOs.Comments;
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

        [HttpPost("/comment")]
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

        [HttpDelete("/comment/{commentId}")]
        public async Task<IActionResult> RemoveComment(Guid commentId)
        {
            var success = await _commentService.RemoveComment(commentId);
            if (!success)
            {
                return NotFound("Comment not found or Unable to delete.");
            }

            return Ok("Comment deleted.");
        }


        [HttpPost("/comment/reply/{commentId}")]
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
        [HttpGet("/comment/{taskId}")]
        public async Task<IActionResult> GetAllCommentByTaskID(Guid taskId)
        {
            var comments = await _commentService.GetAllCommentByTaskID(taskId);
            return Ok(comments);
        }

        [HttpPut("/comment/{commentId}")]
        public async Task<IActionResult> UpdateComment(Guid commentId, ReplyCommentRequest updatedComment)
        {
            var updated = await _commentService.UpdateComment(commentId, updatedComment);
            if (updated == null)
            {
                return NotFound("Comment not found or Unable to update.");
            }

            return Ok(updated);
        }
    }
}
