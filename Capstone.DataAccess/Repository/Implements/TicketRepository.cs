using Capstone.Common.DTOs.Task;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Capstone.DataAccess.Repository.Implements
{
    public class TicketRepository : BaseRepository<Entities.Task>, ITicketRepository
    {
        public TicketRepository(CapstoneContext context) : base(context)
        {

        }

		public async Task<List<TaskViewModel>> GetAllTask(Guid projectId )
		{
			var interationList = _context.Interations.Where(x => x.BoardId == projectId );
			var taskList = new List<TaskViewModel>();
			
			var subTaskList = new List<TaskViewModel>();
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
					if (result.Any(x => x.TaskId == item.PrevId))
					{
						subTaskList.Add(new TaskViewModel
						{
							AssignTo = item.ProjectMember.Users.UserName,
							CreateBy = item.ProjectMember.Users.UserName,
							CreateTime = item.CreateTime,
							Decription = item.Decription,
							DeleteAt = item.DeleteAt,
							DueDate = item.DueDate,
							InterationName = interation.InterationName,
							IsDelete = item.IsDelete,
							PrevId = item.PrevId,
							PriorityName = item.PriorityLevel.Title,
							StartDate = item.StartDate,
							StatusName = item.Status.Title,
							Title = item.Title,
							TaskId = item.TaskId,
							TypeName = item.TicketType.Title,
						});
					}
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
						Title = item.Title,
						TaskId = item.TaskId,
						TypeName = item.TicketType.Title,
						SubTask = subTaskList
					};
					taskList.Add(task);
				}
			}
			return taskList.ToList();
		}
	}
}
