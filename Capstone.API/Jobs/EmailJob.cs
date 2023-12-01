using Capstone.API.Helper;
using Capstone.Common.Constants;
using Capstone.DataAccess;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.TicketService;
using Microsoft.EntityFrameworkCore;

namespace Capstone.API.Jobs
{
    
    public class EmailJob:IEmailJob
    {
        
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IMailHelper _mailHelper;
        public EmailJob(ITaskRepository taskRepository, IProjectMemberRepository projectMemberRepository, IMailHelper mailHelper)
        {
            _taskRepository = taskRepository;
            _projectMemberRepository = projectMemberRepository;
            _mailHelper = mailHelper;
        }
        public async Task RunJob()
        {
            Console.WriteLine($"Deadline email check and send at {DateTime.Now}");
            var lstTaskToSendEmail = await _taskRepository.GetQuery()
                .Where(x => x.DueDate >= DateTime.Today.AddDays(-3) 
                && !x.IsDelete.Value && x.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.Done)
                .ToListAsync();
            var lstAssigned = lstTaskToSendEmail.Select(x => x.AssignTo).ToList();
            var lstCreatedBBy = lstTaskToSendEmail.Select(x => x.CreateBy).ToList();
            var lstMemberToSendMail = await _projectMemberRepository.GetQuery().Where(x => lstAssigned.Contains(x.MemberId) || lstCreatedBBy.Contains(x.MemberId)).Select(d=> new
            {
                MemberId = d.MemberId,
                Email = d.Users.Email

            }).ToListAsync();
            foreach(var member in lstMemberToSendMail)
            {
                var lstTaskToSend = lstTaskToSendEmail.Where(x => x.AssignTo == member.MemberId || x.CreateBy == member.MemberId).ToList();
                foreach(var task in lstTaskToSend)
                {
                    var message = GetTaskMailMessage(task);
                    var subject = "Task deadline incoming!";
                    var to = member.Email;
                    await _mailHelper.Send(to, subject, message);
                }
            }
        }
        public string GetTaskMailMessage(DataAccess.Entities.Task task)
        {
            if(DateTime.Today.AddDays(-1) <= task.DueDate)
            {
                return $"Deadline for task {task.Title} is within next 24h.";
            }
            else
            {
                return $"Deadline for task {task.Title} is within next 72h.";
            }
        }
    }
}
