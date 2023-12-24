using Capstone.Common.Constants;
using Capstone.Common.DTOs.Task;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.NotificationService;
using Capstone.Service.TicketService;
using Microsoft.EntityFrameworkCore;

namespace Capstone.API.Jobs
{
    public class UpdateAndRemindOverdueJob : IFailedJob
    {
        private readonly INotificationService _notificationService;
        private readonly ITaskService _taskService;
        private readonly ITaskRepository _taskRepository;
        public UpdateAndRemindOverdueJob(INotificationService notificationService, ITaskService taskService, ITaskRepository taskRepository)
        {
            _notificationService = notificationService;
            _taskService = taskService;
            _taskRepository = taskRepository;
        }
        public async Task RunJob()
        {
            try
            {
                await UpdateAndSendNotificationOverdueTask();
            }
            catch (Exception ex)
            {
                ;
            }
        }
        private async Task UpdateAndSendNotificationOverdueTask()
        {
            var taskList = await _taskRepository.GetQuery()

                .Include(x => x.Status)
                .Include(t => t.Interation)
                .ThenInclude(it => it.Board)
                .ThenInclude(b => b.Project)
                .ThenInclude(prj => prj.ProjectMembers).ThenInclude(prjMem => prjMem.Users)
                .Include(t => t.Interation)
                .ThenInclude(it => it.Board)
                .ThenInclude(b => b.Project)
                .ThenInclude(prj => prj.ProjectMembers).ThenInclude(prjMem => prjMem.Role)
                .Where(x => x.DueDate < DateTime.UtcNow &&
                !x.IsDelete.Value && x.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.Done &&
                x.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.Fail &&
                x.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.Deleted &&
                (x.Interation.Board.Project.StatusId != Guid.Parse("855C5F2C-8337-4B97-ACAE-41D12F31805C") ||
                x.Interation.Board.Project.StatusId != Guid.Parse("C59F200A-C557-4492-8D0A-5556A3BA7D31")))
                .ToListAsync();
            if (taskList.Count == 0) return;
            foreach (var task in taskList)
            {
                var failedStatus = (await _taskService.GetAllTaskStatus(task.Interation.BoardId)).FirstOrDefault(x => x.Title == CapstoneNameConstant.TaskStatusNameConstant.Fail);
                if (failedStatus == null) continue;
                var admin = task.Interation.Board.Project.ProjectMembers.FirstOrDefault(x => x.UserId == Guid.Parse("AFA06CDD-7713-4B81-9163-C45556E4FA4C"));
                if (admin == null) continue;
                var updateRequest = new UpdateTaskRequest
                {
                    TaskId = task.TaskId,
                    InterationId = task.InterationId,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    StartDate = task.StartDate,
                    AssignTo = task.AssignTo,
                    TypeId = task.TypeId,
                    PriorityId = task.PriorityId,
                    StatusId = failedStatus.BoardStatusId.Value,
                    MemberId = admin.MemberId
                };
                var updateResult = await _taskService.UpdateTask(updateRequest);
                if (updateResult != null)
                {
                    await _notificationService.SendNotificationTaskFailedDeadline(task.TaskId, admin.UserId);
                }
            }
        }
    }
}
