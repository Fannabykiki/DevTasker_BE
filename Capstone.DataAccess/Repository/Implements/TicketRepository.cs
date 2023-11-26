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
								.Where(x => x.Interation.BoardId == projectId && x.IsDelete == false).OrderBy(x=>x.CreateTime)
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
														.Where(m => m.PrevId == x.TaskId && m.IsDelete == false).OrderBy(x => x.CreateTime)
														.Select(m => new SubTask
                                                        {
															TaskId = m.TaskId,
															StatusName = m.Status.Title,
															StatusId = m.StatusId,
															StartDate = m.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
															TypeId = m.TypeId,
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
							.Where(x => x.Interation.BoardId == projectId && x.StatusId == statusId && x.IsDelete == false)
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
									TypeId = x.TypeId,
									TaskId = x.TaskId,
									TypeName = x.TicketType.Title,
									ExpireTime = x.ExprireTime == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
								}).ToListAsync();
			return taskList;
		}

		public async Task<TaskDetailViewModel> GetTaskDetail(Guid taskId)
		{
			var taskList = await _context.Tasks
								.Where(x => x.TaskId == taskId).Include(x => x.ProjectMember).ThenInclude(x => x.Users).ThenInclude(x => x.Status)
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
									.Where(m => m.TaskId == x.TaskId).Include(x => x.ProjectMember).ThenInclude(x => x.Users).ThenInclude(x => x.Status)
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
										User = new GetUserCommentResponse
										{
											Email = m.ProjectMember.Users.Email,
											UserId = m.ProjectMember.Users.UserId,
											UserName = m.ProjectMember.Users.UserName,
										},
										SubComments = _context.TaskComments.Where(c => c.ReplyTo == m.CommentId).Include(x => x.ProjectMember).ThenInclude(x => x.Users).ThenInclude(x => x.Status)
													.Select(c => new GetCommentResponse
													{
														CommentId = c.CommentId,
														Content = c.Content,
														CreateAt = c.CreateAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
														ReplyTo = m.CommentId,
														DeleteAt = c.DeleteAt == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
														UpdateAt = c.UpdateAt == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
														TaskId = c.TaskId,
														User = new GetUserCommentResponse
														{
															Email = c.ProjectMember.Users.Email,
															UserId = c.ProjectMember.Users.UserId,
															UserName = c.ProjectMember.Users.UserName,
														},
													}).ToList()
									}).ToList(),
									AttachmentResponse = _context.Attachments
									.Where(a => a.TaskId == x.TaskId)
									.Select(a => new Common.DTOs.Attachment.AttachmentViewModel
									{
										CreateAt = a.CreateAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
										TaskId = a.TaskId,
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
									}).ToList(),
									TaskHistories = _context.TaskHistories
									.Where(h => h.TaskId == taskId)
									.Select(h => new TaskHistoryViewModel
									{
										TaskId = taskId,
										ChangeAt = h.ChangeAt == null
  ? null
  : h.ChangeAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
										HistoryId = h.HistoryId,
										Title = h.Title
									}).ToList()				
								}).FirstOrDefaultAsync();
			return taskList;
		}

		public async Task<int> GetTaskDone(Guid projectId)
		{
			var taskList = await _context.Tasks
								.Where(x => x.Interation.BoardId == projectId && x.IsDelete == true && x.StatusId == Guid.Parse("53F76F08-FF3C-43EB-9FF4-C9E028E513D5"))
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
									TypeId = x.TypeId,
									TaskId = x.TaskId,
									TypeName = x.TicketType.Title,
								}).ToListAsync();
			return taskList.Count();
		}
	}
}
