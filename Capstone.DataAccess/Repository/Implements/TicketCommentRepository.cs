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
                .AsQueryable()
                .Include(m => m.ProjectMember).ThenInclude(pm => pm.Users)
                .Include(m => m.ProjectMember).ThenInclude(pm => pm.Users.Status)
                .Where(x => x.TaskId == taskId && x.ReplyTo == null && x.DeleteAt == null)
                .OrderBy(c => c.CreateAt)
                .Select(x => new GetCommentResponse
                {
                    CommentId = x.CommentId,
                    Content = x.Content,
                    CreateAt = x.CreateAt == null ? null : x.CreateAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    DeleteAt = x.DeleteAt == null ? null : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    UpdateAt = x.UpdateAt == null ? null : x.UpdateAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    TaskId = x.TaskId, // No need for type conversion
                    ReplyTo = x.ReplyTo,
                    User = new GetUserCommentResponse
                    {
                        UserId = x.ProjectMember.UserId,
                        UserName = x.ProjectMember.Users.UserName,
                        Fullname = x.ProjectMember.Users.Fullname,
                        Email = x.ProjectMember.Users.Email,
                        IsFirstTime = x.ProjectMember.Users.IsFirstTime,
                        IsAdmin = x.ProjectMember.Users.IsAdmin,
                        Status = x.ProjectMember.Users.Status.Title
                    },
                    SubComments = _context.TaskComments
                        .AsQueryable()
                        .Include(m => m.ProjectMember).ThenInclude(pm => pm.Users)
                        .Where(m => m.ReplyTo == x.CommentId && m.DeleteAt == null)
                        .OrderBy(c => c.CreateAt)
                        .Select(sub => new GetCommentResponse
                        {
                            CommentId = sub.CommentId,
                            Content = sub.Content,
                            CreateAt = sub.CreateAt == null ? null : sub.CreateAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                            DeleteAt = sub.DeleteAt == null ? null : sub.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                            UpdateAt = sub.UpdateAt == null ? null : sub.UpdateAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                            TaskId = sub.TaskId, // No need for type conversion
                            ReplyTo = sub.ReplyTo,
                            User = new GetUserCommentResponse
                            {
                                UserId = sub.ProjectMember.UserId,
                                UserName = sub.ProjectMember.Users.UserName,
                                Fullname = sub.ProjectMember.Users.Fullname,
                                Email = sub.ProjectMember.Users.Email,
                                IsFirstTime = sub.ProjectMember.Users.IsFirstTime,
                                IsAdmin = sub.ProjectMember.Users.IsAdmin,
                                Status = sub.ProjectMember.Users.Status.Title
                            },
                        })
                        .ToList()
                })
                .ToListAsync();

            return commentList;
        }
    }
}
