using Capstone.Common.DTOs.Comments;
using Capstone.Common.DTOs.Task;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Capstone.DataAccess.Repository.Implements
{
	public class TicketRepository : BaseRepository<Entities.Task>, ITaskRepository
	{
		public TicketRepository(CapstoneContext context) : base(context)
		{

		}

		public async Task<List<TaskViewModel>> GetAllTask(Guid projectId)
		{
			var taskList = await _context.Tasks
								.Where(x => x.Interation.BoardId == projectId)
								.Select(x => new TaskViewModel
								{
									AssignTo = x.ProjectMember.Users.UserName,
									CreateBy = x.ProjectMember.Users.UserName,
									CreateTime = x.CreateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Description = x.Description,
									DeleteAt = x.DeleteAt == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									DueDate = x.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									ExpireTime = x.ExprireTime == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									InterationName = x.Interation.InterationName,
									IsDelete = x.IsDelete,
									PriorityName = x.PriorityLevel.Title,
									StartDate = x.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									StatusName = x.Status.Title,
									StatusId = x.StatusId,
									Title = x.Title,
									TaskId = x.TaskId,
									TypeId = x.TypeId,
									TypeName = x.TicketType.Title,
									SubTask = _context.Tasks
														.Where(m => m.PrevId == x.TaskId)
														.Select(m => new TaskViewModel
														{
															TaskId = m.TaskId,
															StatusName = m.Status.Title,
															StatusId = x.StatusId,
															StartDate = m.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
															TypeId = x.TypeId,
															TypeName = m.TicketType.Title,
															Title = m.Title,
															Description = m.Description,
															PriorityName = m.PriorityLevel.Title,
															CreateBy = m.ProjectMember.Users.UserName,
															AssignTo = m.ProjectMember.Users.UserName,
															DueDate = m.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
															CreateTime = m.CreateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
															InterationName = m.Interation.InterationName,
															IsDelete = m.IsDelete,
															DeleteAt = m.DeleteAt == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
														}).ToList(),
								}).ToListAsync();
			return taskList;
		}

		public async Task<List<TaskViewModel>> GetAllTaskCompleted(Guid projectId, Guid statusId)
		{
			var taskList = await _context.Tasks
							.Where(x => x.Interation.BoardId == projectId && x.StatusId == statusId)
							.Select(x => new TaskViewModel
							{
								AssignTo = x.ProjectMember.Users.UserName,
								CreateBy = x.ProjectMember.Users.UserName,
								CreateTime = x.CreateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
								Description = x.Description,
								DeleteAt = x.DeleteAt == null
? null
: x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
								DueDate = x.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
								InterationName = x.Interation.InterationName,
								IsDelete = x.IsDelete,
								PriorityName = x.PriorityLevel.Title,
								StartDate = x.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
								StatusName = x.Status.Title,
								StatusId = x.StatusId,
								Title = x.Title,
								TaskId = x.TaskId,
								TypeName = x.TicketType.Title,
							}).ToListAsync();
			return taskList;
		}

		public async Task<List<TaskViewModel>> GetAllTaskDelete(Guid projectId)
		{
			var taskList = await _context.Tasks
								.Where(x => x.Interation.BoardId == projectId && x.IsDelete == true)
								.Select(x => new TaskViewModel
								{
									AssignTo = x.ProjectMember.Users.UserName,
									CreateBy = x.ProjectMember.Users.UserName,
									CreateTime = x.CreateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Description = x.Description,
									DeleteAt = x.DeleteAt == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									DueDate = x.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									InterationName = x.Interation.InterationName,
									IsDelete = x.IsDelete,
									PriorityName = x.PriorityLevel.Title,
									StartDate = x.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									StatusName = x.Status.Title,
									StatusId = x.StatusId,
									Title = x.Title,
									TaskId = x.TaskId,
									TypeName = x.TicketType.Title,
								}).ToListAsync();
			return taskList;
		}

		public async Task<TaskDetailViewModel> GetTaskDetail(Guid taskId)
		{
			var taskList = await _context.Tasks
								.Where(x => x.TaskId == taskId)
								.Select(x => new TaskDetailViewModel
								{
									AssignTo = x.ProjectMember.Users.UserName,
									CreateBy = x.ProjectMember.Users.UserName,
									CreateTime = x.CreateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Description = x.Description,
									DeleteAt = x.DeleteAt == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									DueDate = x.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									ExpireTime = x.ExprireTime == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									InterationName = x.Interation.InterationName,
									IsDelete = x.IsDelete,
									PriorityName = x.PriorityLevel.Title,
									StartDate = x.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									StatusName = x.Status.Title,
									StatusId = x.StatusId,
									Title = x.Title,
									TaskId = x.TaskId,
									TypeId = x.TypeId,
									TypeName = x.TicketType.Title,
									CommentResponse = _context.TaskComments
									.Where(m => m.TaskId == x.TaskId)
									.Select(m => new GetCommentResponse
									{
										Content = m.Content,
										CommentId = m.CommentId,
										UpdateAt = m.UpdateAt == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
										CreateAt = m.CreateAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
										DeleteAt = m.DeleteAt == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
										ReplyTo = m.ReplyTo,
										TaskId = m.TaskId,

									}).ToList(),
									AttachmentResponse = _context.Attachments
									.Where(a => a.TaskId == x.TaskId)
									.Select(a => new Common.DTOs.Attachment.AttachmentViewModel
									{
										CreateAt = a.CreateAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
										TaskId= a.TaskId,
										AttachmentId = a.AttachmentId,
										DeleteAt = a.DeleteAt == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
										CreateBy = a.ProjectMember.Users.UserName,
										ExprireTime = a.ExprireTime == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
										IsDeleted = a.IsDeleted,
										TaskTitle = a.Task.Title
									}).ToList()
								}).FirstOrDefaultAsync();
			return taskList;
		}
	}
}
