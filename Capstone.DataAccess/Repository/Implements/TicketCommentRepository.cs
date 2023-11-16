using Capstone.Common.DTOs.Comments;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace Capstone.DataAccess.Repository.Implements
{
    public class TicketCommentRepository : BaseRepository<TaskComment>, ITaskCommentRepository
    {
        public TicketCommentRepository(CapstoneContext context) : base(context)
        {

        }

        public async Task<List<GetCommentResponse>> GetAllTaskComment(Guid taskId)
        {
            var commentList = await _context.TaskComments
                .Where(x => x.TaskId == taskId && x.ReplyTo == null && x.DeleteAt == null).OrderBy(c => c.CreateAt)
                .Select(x => new GetCommentResponse
                {
                    CommentId = x.CommentId,
                    Content= x.Content,
                    CreateAt = x.CreateAt == null ? null : x.CreateAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    DeleteAt = x.DeleteAt == null ? null : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    UpdateAt = x.UpdateAt == null ? null : x.UpdateAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    TaskId = (Guid)x.TaskId,
                    ReplyTo = x.ReplyTo == null ? null : x.ReplyTo,
                    User = _context.Users
                            .Select(u => new GetUserCommentResponse
                                {
                                    UserId = u.UserId,
                                    Fullname = u.Fullname,
                                    Email = u.Email,
                                    IsFirstTime = u.IsFirstTime,
                                    IsAdmin = u.IsAdmin,
                                    Status = _context.Status.FirstOrDefault(s => s.StatusId == u.StatusId)
                                            .Title,

                                })
                            .FirstOrDefault(u => u.UserId == x.CreateBy),
                    SubComments = _context.TaskComments
                .Where(m => m.ReplyTo == x.CommentId && m.DeleteAt == null).OrderBy(c => c.CreateAt)
                .Select(x => new GetCommentResponse
                {
                    CommentId = x.CommentId,
                    Content = x.Content,
                    CreateAt = x.CreateAt == null ? null : x.CreateAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    DeleteAt = x.DeleteAt == null ? null : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    UpdateAt = x.UpdateAt == null ? null : x.UpdateAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    TaskId = (Guid)x.TaskId,
                    ReplyTo = x.ReplyTo == null ? null : x.ReplyTo,
                    User = _context.Users
                            .Select(u => new GetUserCommentResponse
                            {
                                UserId = u.UserId,
                                Fullname = u.Fullname,
                                Email = u.Email,
                                IsFirstTime = u.IsFirstTime,
                                IsAdmin = u.IsAdmin,
                                Status = _context.Status.FirstOrDefault(s => s.StatusId == u.StatusId)
                                            .Title,

                            })
                            .FirstOrDefault(u => u.UserId == x.CreateBy),
                }).ToList()
                }).ToListAsync();

            return commentList;
        }
    }
}
