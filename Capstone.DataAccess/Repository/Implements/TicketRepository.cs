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
								.Where(x => x.Interation.BoardId == projectId && x.IsDelete == false && x.PrevId == null).OrderBy(x => x.CreateTime).Include(x => x.PriorityLevel)
								.Select(x => new TaskViewModel
								{
									AssignTo = _context.ProjectMembers.Where(a => a.MemberId == x.AssignTo).Include(a => a.Users).Select(a => a.Users.UserName).FirstOrDefault(),
									CreateBy = _context.Users.Where(u => u.UserId == x.CreateBy).Select(a => a.UserName).FirstOrDefault(),
									CreateTime = x.CreateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Description = x.Description,
									DeleteAt = x.DeleteAt == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									DueDate = x.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									ExpireTime = x.ExprireTime == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									InterationId = x.InterationId,
									InterationName = x.Interation.InterationName,
									IsDelete = x.IsDelete,
									PriorityName = x.PriorityLevel.Title,
									StartDate = x.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									StatusName = x.Status.Title,
									StatusId = x.StatusId,
									Title = x.Title,
									TaskId = x.TaskId,
									TypeId = x.TypeId,
									Priority = x.PriorityId,
									TypeName = x.TicketType.Title,
									PriorityLevel = x.PriorityLevel.Level,
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
															InterationId = m.InterationId,
															InterationName = m.Interation.InterationName,
															IsDelete = m.IsDelete,
															PriorityLevel = m.PriorityLevel.Level,
															DeleteAt = m.DeleteAt == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
															Priority = m.PriorityId
														}).ToList(),
								}).ToListAsync();
			return taskList;
		}

		public async Task<List<TaskViewModel>> GetTaskByInterationId(Guid interationId)
		{
			var taskList = await _context.Tasks
								.Where(x => x.InterationId == interationId && x.IsDelete == false && x.PrevId == null).OrderBy(x => x.CreateTime).Include(x => x.PriorityLevel)
								.Select(x => new TaskViewModel
								{
									AssignTo = _context.ProjectMembers.Where(a => a.MemberId == x.AssignTo).Include(a => a.Users).Select(a => a.Users.UserName).FirstOrDefault(),
									CreateBy = _context.Users.Where(u => u.UserId == x.CreateBy).Select(a => a.UserName).FirstOrDefault(),
									CreateTime = x.CreateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Description = x.Description,
									DeleteAt = x.DeleteAt == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									DueDate = x.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									ExpireTime = x.ExprireTime == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									InterationId = x.InterationId,
									InterationName = x.Interation.InterationName,
									IsDelete = x.IsDelete,
									PriorityName = x.PriorityLevel.Title,
									StartDate = x.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									StatusName = x.Status.Title,
									StatusId = x.StatusId,
									Title = x.Title,
									TaskId = x.TaskId,
									TypeId = x.TypeId,
									Priority = x.PriorityId,
									TypeName = x.TicketType.Title,
									PriorityLevel = x.PriorityLevel.Level,
									TotalComment = _context.TaskComments
									.Where(m => m.TaskId == x.TaskId && m.DeleteAt == null).Count(),
									TotalAttachment = _context.Attachments
									.Where(a => a.TaskId == x.TaskId && a.IsDeleted == false).Count(),
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
															InterationId = m.InterationId,
															InterationName = m.Interation.InterationName,
															IsDelete = m.IsDelete,
															PriorityLevel = m.PriorityLevel.Level,
															DeleteAt = m.DeleteAt == null
  ? null
  : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
															Priority = m.PriorityId,
															TotalComment = _context.TaskComments
																		.Where(c => c.TaskId == m.TaskId).Count(),
															TotalAttachment = _context.Attachments
																		.Where(a => a.TaskId == m.TaskId).Count(),
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
								AssignTo = _context.ProjectMembers.Where(a => a.MemberId == x.AssignTo).Include(a => a.Users).Select(a => a.Users.UserName).FirstOrDefault(),
								CreateBy = _context.Users.Where(u => u.UserId == x.CreateBy).Select(a => a.UserName).FirstOrDefault(),
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
									AssignTo = _context.ProjectMembers.Where(a => a.MemberId == x.AssignTo).Include(a => a.Users).Select(a => a.Users.UserName).FirstOrDefault(),
									CreateBy = _context.Users.Where(u => u.UserId == x.CreateBy).Select(a => a.UserName).FirstOrDefault(),
									CreateTime = x.CreateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Description = x.Description,
									DeleteAt = x.DeleteAt == null ? null : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
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
									ExpireTime = x.ExprireTime == null ? null : x.ExprireTime.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
								}).ToListAsync();
			return taskList;
		}

		public async Task<TaskDetailViewModel> GetTaskDetail(Guid taskId)
		{
			var taskList = await _context.Tasks
								.Where(x => x.TaskId == taskId).Include(x => x.ProjectMember).ThenInclude(x => x.Users).ThenInclude(x => x.Status)
								.Select(x => new TaskDetailViewModel
								{
									AssignTo = _context.ProjectMembers.Where(a => a.MemberId == x.AssignTo).Include(a=>a.Users).Select(a=>a.Users.UserName).FirstOrDefault(),
									CreateBy = _context.Users.Where(u => u.UserId == x.CreateBy).Select(a => a.UserName).FirstOrDefault(),
									AssignToStatus = x.ProjectMember.Users.Status.Title,
									AssignToStatusId = x.ProjectMember.Users.Status.StatusId,
									CreateTime = x.CreateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Description = x.Description,
									DeleteAt = x.DeleteAt == null ? null : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									DueDate = x.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									ExpireTime = x.ExprireTime == null ? null : x.ExprireTime.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
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
									ProjectId = _context.BoardStatus.Where(b => b.BoardStatusId == x.StatusId).Include(a => a.Board).Select(a => a.Board.BoardId).FirstOrDefault(),
									CommentResponse = _context.TaskComments
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
				.ToList(),
									AttachmentResponse = _context.Attachments
									.Where(a => a.TaskId == x.TaskId)
									.Select(a => new Common.DTOs.Attachment.AttachmentViewModel
									{
										Title = a.Title,
										CreateAt = a.CreateAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
										TaskId = a.TaskId,
										AttachmentId = a.AttachmentId,
										DeleteAt = a.DeleteAt == null ? null : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
										CreateBy = a.ProjectMember.Users.UserName,
										ExprireTime = a.ExprireTime == null ? null : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
										IsDeleted = a.IsDeleted,
										TaskTitle = a.Task.Title
									}).ToList(),
									TaskHistories = _context.TaskHistories
									.Where(h => h.TaskId == taskId)
									.OrderBy(h => h.ChangeAt)
									.Select(h => new TaskHistoryViewModel
									{
										TaskId = taskId,
										ChangeAt = h.ChangeAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
										HistoryId = h.HistoryId,
										Title = h.Title
									}).ToList()
								}).FirstOrDefaultAsync();
			return taskList;
		}

		public async Task<int> GetTaskDone(Guid projectId)
		{
			var taskList = await _context.Tasks
								.Where(x => x.Interation.BoardId == projectId && x.IsDelete == true && x.StatusId == Guid.Parse("53F76F08-FF3C-43EB-9FF4-C9E028E513D5") && x.IsDelete == false)
								.Select(x => new TaskViewModel
								{
									AssignTo = x.ProjectMember.Users.UserName,
									CreateBy = x.ProjectMember.Users.UserName,
									CreateTime = x.CreateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Description = x.Description,
									DeleteAt = x.DeleteAt == null ? null : x.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
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
