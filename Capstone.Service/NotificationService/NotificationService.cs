using AutoMapper;
using AutoMapper.Execution;
using Capstone.Common.Constants;
using Capstone.Common.DTOs.Notification;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.Hubs;
using Google.Apis.Drive.v3.Data;
using MailKit.Security;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Capstone.Service.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskCommentRepository _taskCommentRepository;
        private readonly PresenceTracker _presenceTracker;

        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly IMapper _mapper;
        public NotificationService(INotificationRepository notificationRepository,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            IProjectMemberRepository projectMemberRepository,
            ITaskRepository taskRepository,
            ITaskCommentRepository taskCommentRepository,
            PresenceTracker presenceTracker,
            IHubContext<NotificationHub> hubContext,
            IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _projectMemberRepository = projectMemberRepository;
            _taskRepository = taskRepository;
            _taskCommentRepository = taskCommentRepository;
            _presenceTracker = presenceTracker;
            _hubContext = hubContext;
            _mapper = mapper;
        }
        public async Task<List<NotificationViewModel>> GetLatestNotifications(Guid userId, int page = 10)
        {
            var results = await _notificationRepository.GetQuery().Where(x => x.RecerverId == userId).OrderByDescending(y => y.CreateAt).Take(page).ToListAsync();
            return _mapper.Map<List<NotificationViewModel>>(results);
        }

        public async System.Threading.Tasks.Task SendNotificationChangeProjectStatus(string projectId, string userId)
        {
            var project = await _projectRepository.GetQuery().Include("Status").FirstOrDefaultAsync(x => x.ProjectId.ToString() == projectId);
            var projectMembers = await _projectMemberRepository.GetQuery().Include("Role").Where(x => x.ProjectId.ToString() == projectId).ToListAsync();
            if (project == null) return;
            var lstReceived = projectMembers.Where(x => x.UserId.ToString() != userId).Select(x => x.UserId);
            var lstNotification = lstReceived.Select(x => new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = "Status change",
                Description = $"The status of project {project.ProjectName} has been changed to {project.Status.Title}",
                CreateAt = DateTime.Now,
                IsRead = false,
                RecerverId = x,
                TargetUrl = $"https://devtasker.azurewebsites.net/project/{projectId}"
            }).ToList();
            foreach (var notif in lstNotification)
            {
                await _notificationRepository.CreateAsync(notif);
            }
            await _notificationRepository.SaveChanges();
            //Send Notif
            foreach (var user in lstReceived)
            {
                if (!await _presenceTracker.IsOnlineUser(user.ToString()))
                {
                    continue;
                }
                await _hubContext.Clients.Group(user.ToString()).SendAsync("EmitNotification");
            }
            //Send Mail
            var lstProjectAdmin = projectMembers
                .Where(x => x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor).Select(y => y.UserId);
            await SendMailForNotification(lstProjectAdmin.ToList(), lstNotification.ToList());
        }
        public async System.Threading.Tasks.Task SendNotificationChangeTaskStatus(string taskId, string userId)
        {
            var task = await _taskRepository.GetQuery()
                .Include(t=>t.ProjectMember)
                .Include(x => x.Status)
                .Include(t => t.Interation)
                .ThenInclude(it => it.Board)
                .ThenInclude(b => b.Project)
                .ThenInclude(prj=>prj.ProjectMembers).ThenInclude(prjMem => prjMem.Role)
                .FirstOrDefaultAsync(x => x.TaskId.ToString() == taskId);
            if (task == null) return;
            if (task.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.ToDo
                && task.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.Done
                && task.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.Deleted) return;

            var lstProjectAdmin = task.Interation.Board.Project.ProjectMembers
                .Where(x => (x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor) && x.UserId.ToString() != userId ).Select(y => y.UserId);
            var createdBy = await _projectMemberRepository.GetQuery().FirstOrDefaultAsync(x => x.UserId == task.CreateBy);
            var listReceiver = lstProjectAdmin;
            var title = "";
            var description = "";
            var TargetUrl = $"https://devtasker.azurewebsites.net/project/{task.Interation.BoardId}";

            switch (task.Status.Title)
            {
                case CapstoneNameConstant.TaskStatusNameConstant.ToDo:

                    title = "[Task Is Ready]";
                    description = $"The task {task.Title} has been set to {CapstoneNameConstant.TaskStatusNameConstant.ToDo}";
                    listReceiver = listReceiver.Append(task.ProjectMember.UserId).Distinct();
                    break;

                case CapstoneNameConstant.TaskStatusNameConstant.Done:

                    title = "[Task Is Done]";
                    description = $"The task {task.Title} has been set to {CapstoneNameConstant.TaskStatusNameConstant.Done}";
                    listReceiver = listReceiver.Append(task.ProjectMember.UserId).Distinct();
                    listReceiver = listReceiver.Append(createdBy.UserId).Distinct();
                    break;

                case CapstoneNameConstant.TaskStatusNameConstant.Deleted:

                    title = "[Task Is Deleted]";
                    description = $"The task {task.Title} has been set to {CapstoneNameConstant.TaskStatusNameConstant.Deleted}";
                    listReceiver = listReceiver.Append(task.ProjectMember.UserId).Distinct();
                    listReceiver = listReceiver.Append(createdBy.UserId).Distinct();
                    break;

            }
            var listNotification = listReceiver.Select(id => new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = title,
                Description = description,
                CreateAt = DateTime.Now,
                TargetUrl = TargetUrl,
                IsRead = false,
                RecerverId = id
            });
            foreach (var notif in listNotification)
            {
                await _notificationRepository.CreateAsync(notif);
            }

            await _notificationRepository.SaveChanges();

            await SendMailForNotification(lstProjectAdmin.ToList(), listNotification.ToList());

            foreach (var user in listReceiver)
            {
                if (!await _presenceTracker.IsOnlineUser(user.ToString()))
                {
                    continue;
                }
                await _hubContext.Clients.Group(user.ToString()).SendAsync("EmitNotification");
            }
            //send mail for admins
            await SendMailForNotification(lstProjectAdmin.ToList(), listNotification.ToList());
            
            
        }
        public async System.Threading.Tasks.Task SendNotificationCommentTask(string commentId, string userId, string action)
        {
            var comment = await _taskCommentRepository.GetQuery().FirstOrDefaultAsync(x => x.CommentId.ToString() == commentId);
            if (comment == null) return;
            var task = await _taskRepository.GetQuery()
                .Include(t => t.ProjectMember)
                .Include(x => x.Status)
                .Include(t => t.Interation)
                .ThenInclude(it => it.Board)
                .ThenInclude(b => b.Project)
                .ThenInclude(prj => prj.ProjectMembers).ThenInclude(prjMem => prjMem.Role)
                .Include(t => t.Interation)
                .ThenInclude(it => it.Board)
                .ThenInclude(b => b.Project)
                .ThenInclude(prj => prj.ProjectMembers).ThenInclude(prjMem => prjMem.Users)
                .FirstOrDefaultAsync(x => x.TaskId == comment.TaskId);
            if (task == null) return;
            var cmtUser = await _userRepository.GetAsync(x => x.UserId.ToString() == userId, null);
            if (cmtUser == null) return;
            
            
            comment.Task = task;
            var lstReceived = comment.Task.Interation.Board.Project.ProjectMembers
                .Where(x => x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor || x.MemberId == comment.Task.AssignTo)
                .Select(y => new
                {
                    UserId = y.UserId,
                    Name = y.Users.UserName,
                    Role = y.Role.RoleName
                }).Where(dt=>dt.UserId.ToString() != userId).ToList();

            var lstNotification = new List<Notification>();
            switch (action)
            {
                case CommentActionCconstant.Create:
                    lstNotification = lstReceived.Select(x => new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Comment created",
                        Description = $"A comment has been created in task {comment.Task.Title} by {cmtUser.UserName} ",
                        CreateAt = DateTime.Now,
                        IsRead = false,
                        RecerverId = x.UserId,
                        TargetUrl = $"https://devtasker.azurewebsites.net/project/{comment.Task.Interation.BoardId}"
                    }).ToList();
                    break;

                case CommentActionCconstant.Edit:
                    lstNotification = lstReceived.Select(x => new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Comment status change",
                        Description = $"Comment has been edited in task {comment.Task.Title} by {cmtUser.UserName} ",
                        CreateAt = DateTime.Now,
                        IsRead = false,
                        RecerverId = x.UserId,
                        TargetUrl = $"https://devtasker.azurewebsites.net/project/{comment.Task.Interation.BoardId}"
                    }).ToList();
                    break;

                case CommentActionCconstant.Delete:
                    lstNotification = lstReceived.Select(x => new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Comment status change",
                        Description = $"A comment has been deleted in task {comment.Task.Title} by {cmtUser.UserName} ",
                        CreateAt = DateTime.Now,
                        IsRead = false,
                        RecerverId = x.UserId,
                        TargetUrl = $"https://devtasker.azurewebsites.net/project/{comment.Task.Interation.BoardId}"
                    }).ToList();
                    break;

            }
            using var transaction = _notificationRepository.DatabaseTransaction();
            foreach (var notif in lstNotification)
            {
                await _notificationRepository.CreateAsync(notif);
            }
            foreach (var user in lstReceived)
            {
                if (!await _presenceTracker.IsOnlineUser(user.UserId.ToString()))
                {
                    continue;
                }
                await _hubContext.Clients.Group(user.UserId.ToString()).SendAsync("EmitNotification");
            }
            await _notificationRepository.SaveChanges();
            transaction.Commit();
            //Send Email
            var lstProjectAdmin = lstReceived.Where(x => x.Role == RoleNameConstant.ProductOwner || x.Role == RoleNameConstant.Supervisor).Select(d => d.UserId);
            await SendMailForNotification(lstProjectAdmin.ToList(), lstNotification.ToList());

        }
        public async System.Threading.Tasks.Task SendNotificationTaskDeadline()
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
                .Where(x => (x.DueDate <= DateTime.Now.AddDays(3) && x.DueDate >= DateTime.Now.AddDays(2)
                || x.DueDate <= DateTime.Now.AddDays(1)) && 
                !x.IsDelete.Value && x.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.Done)
                .ToListAsync();

            var t = await _taskRepository.GetQuery()
                .Select(x => new
                {
                    DueDate = x.DueDate,
                    Now = DateTime.Now,
                    NowAdd3Day = DateTime.Now.AddDays(3),
                    NowAdd2Day = DateTime.Now.AddDays(2),
                    NowAdd1Day = DateTime.Now.AddDays(1)
                }).ToListAsync();
            if (taskList.Count ==0) return;
            var notificationEmailList = new List<NotificationEmailRequest>();
            foreach (var task in taskList)
            {
                var subject = "[Task Deadline Remind]";
                var message = GetTaskMailMessage(task.Title, task.Interation.Board.Project.ProjectName, task.DueDate);
                var listMem = task.Interation.Board.Project.ProjectMembers;

                var listNotifs = task.Interation.Board.Project.ProjectMembers
                    .Where(x => (x.MemberId == task.AssignTo || x.UserId == task.CreateBy || x.Role.RoleName == RoleNameConstant.ProductOwner
                    || x.Role.RoleName == RoleNameConstant.Supervisor) && x.StatusId == new Guid(StatusGuidConstant.StatusInTeamGuid))
                    .Select(dt => new NotificationEmailRequest
                    {
                        EmailAddress = dt.Users.Email,
                        NotificationContent = new Notification
                        {
                            NotificationId = Guid.NewGuid(),
                            Title = subject,
                            Description = message,
                            CreateAt = DateTime.Now,
                            IsRead = false,
                            RecerverId = dt.Users.UserId,
                            TargetUrl = $"https://devtasker.azurewebsites.net/project/{task.Interation.BoardId}"
                        }
                    }).ToList();
                notificationEmailList.AddRange(listNotifs);  
            }

            foreach (var request in notificationEmailList)
            {
                await _notificationRepository.CreateAsync(request.NotificationContent);
            }

            await _notificationRepository.SaveChanges();

            //Send Notification
            var listReceiver = notificationEmailList.Select(x => x.NotificationContent.RecerverId).Distinct().ToList();
            foreach (var user in listReceiver)
            {
                if (!await _presenceTracker.IsOnlineUser(user.ToString()))
                {
                    continue;
                }
                await _hubContext.Clients.Group(user.ToString()).SendAsync("EmitNotification");
            }
            //Send Email
            foreach (var request in notificationEmailList)
            {
                try
                {
                    await Send(request.EmailAddress, request.NotificationContent.Title, request.NotificationContent.Description);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

        }
        private string GetTaskMailMessage(string taskTitle, string projectName, DateTime dueDate)
        {
            return $"Task <strong>{taskTitle}</strong> of project <strong>{projectName}</strong> will be due on <strong>{dueDate}</strong>";
        }
        private async System.Threading.Tasks.Task SendMailForNotification(List<Guid> lstReceiver, List<Notification> listNotifications)
        {
            try
            {
                var listEmails = await _userRepository.GetQuery().Where(x => lstReceiver.Contains(x.UserId)).Select(x => new
                {
                    UserrId = x.UserId,
                    Email = x.Email
                }).ToListAsync();
                foreach (var email in listEmails)
                {
                    var notification = listNotifications.FirstOrDefault(x => x.RecerverId == email.UserrId);
                    if (notification == null) continue;
                    await Send(email.Email, notification.Title, notification.Description);
                };
            }catch (Exception ex)
            {
                return;
            }
            
        }
        private async System.Threading.Tasks.Task Send(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("devtaskercapstone@gmail.com"));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("devtaskercapstone@gmail.com", "fbacmmlfxlmchkmc");
                client.Send(email);
                client.Disconnect(true);
            }
        }
    }
    public class NotificationEmailRequest
    {
        public string EmailAddress { get; set; }
        public Notification NotificationContent { get; set; }
    }
}
