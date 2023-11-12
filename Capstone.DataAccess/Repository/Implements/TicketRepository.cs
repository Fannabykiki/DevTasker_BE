using Capstone.Common.DTOs.Task;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.DataAccess.Repository.Implements
{
    public class TicketRepository : BaseRepository<Entities.Task>, ITaskRepository
    {
        public TicketRepository(CapstoneContext context) : base(context)
        {

        }

		public async Task<List<TaskViewModel>> GetAllTask(Guid projectId )
		{
			var interationList = await _context.Interations.Where(x => x.BoardId == projectId ).ToListAsync();
			var taskList = new List<TaskViewModel>();
			
			foreach (var interation in interationList)
			{
				var result = _context.Tasks.Where(x => x.InterationId == interation.InterationId)
									   .Include(x => x.ProjectMember).ThenInclude(pm => pm.Users)
									   .Include(x => x.Status)
									   .Include(x => x.TicketType)
									   .Include(x => x.Interation)
									   .Include(x => x.PriorityLevel).AsQueryable();

				foreach (var item in result)
				{
					var subtask = await _context.SubTask.Where(x => x.TaskId == item.TaskId).Include(x => x.ProjectMember).ThenInclude(pm => pm.Users)
									   .Include(x => x.Status)
									   .Include(x => x.TaskType)
									   .Include(x => x.Interation)
									   .Include(x => x.PriorityLevel).ToListAsync();

					var task = new TaskViewModel
					{
						AssignTo = item.ProjectMember.Users.UserName,
						CreateBy = item.ProjectMember.Users.UserName,
						CreateTime = item.CreateTime,
						Decription = item.Decription,
						DeleteAt = item.DeleteAt,
						DueDate = item.DueDate,
						InterationName = interation.InterationName,
						IsDelete = item.IsDelete,
						PriorityName = item.PriorityLevel.Title,
						StartDate = item.StartDate,
						StatusName = item.Status.Title,
						StatusId = item.StatusId,
						Title = item.Title,
						TaskId = item.TaskId,
						TypeName = item.TicketType.Title,
						SubTask = subtask.Select(m => new TaskViewModel
						{
							TaskId = m.TaskId,
							StatusName = m.Status.Title,
							StatusId = item.StatusId,
							StartDate = m.StartDate,
							TypeName = m.TaskType.Title,
							Title = m.Title,
							Decription = m.Decription,
							PriorityName = m.PriorityLevel.Title,
							CreateBy = m.ProjectMember.Users.UserName,
							AssignTo = m.ProjectMember.Users.UserName,
							DueDate = m.DueDate,
							CreateTime = m.CreateTime,
							InterationName = m.Interation.InterationName,
							IsDelete=m.IsDelete,
							DeleteAt=m.DeleteAt,
						}).ToList(),
					};
					taskList.Add(task);
				}
			}
			return taskList.ToList();
		}
	}
}
