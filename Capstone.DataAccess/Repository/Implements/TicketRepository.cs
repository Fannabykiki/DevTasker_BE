using Capstone.Common.DTOs.Task;
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
									CreateTime = x.CreateTime,
									Decription = x.Decription,
									DeleteAt = x.DeleteAt,
									DueDate = x.DueDate,
									InterationName = x.Interation.InterationName,
									IsDelete = x.IsDelete,
									PriorityName = x.PriorityLevel.Title,
									StartDate = x.StartDate,
									StatusName = x.Status.Title,
									StatusId = x.StatusId,
									Title = x.Title,
									TaskId = x.TaskId,
									TypeName = x.TicketType.Title,
									SubTask = _context.Tasks
														.Where(m => m.PrevId == x.TaskId)
														.Select(m => new TaskViewModel
															{
															TaskId = m.TaskId,
															StatusName = m.Status.Title,
															StatusId = x.StatusId,
															StartDate = m.StartDate,
															TypeName = m.TicketType.Title,
															Title = m.Title,
															Decription = m.Decription,
															PriorityName = m.PriorityLevel.Title,
															CreateBy = m.ProjectMember.Users.UserName,
															AssignTo = m.ProjectMember.Users.UserName,
															DueDate = m.DueDate,
															CreateTime = m.CreateTime,
															InterationName = m.Interation.InterationName,
															IsDelete = m.IsDelete,
															DeleteAt = m.DeleteAt,
															}).ToList(),
								}).ToListAsync();
			return taskList;
		}
	}
}
