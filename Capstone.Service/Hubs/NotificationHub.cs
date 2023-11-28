﻿using Capstone.Common.Constants;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Google.Apis.Drive.v3.Data;
using MailKit.Security;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Capstone.Common.Constants.CapstoneNameConstant;

namespace Capstone.Service.Hubs
{
    public class NotificationHub : Hub
    {
        //private readonly CapstoneContext _capstoneContext;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskCommentRepository _taskCommentRepository;
        private readonly PresenceTracker _presenceTracker;
        //private readonly IMailHelper _mailHelper;
        public NotificationHub(INotificationRepository notificationRepository,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            IProjectMemberRepository projectMemberRepository,
            ITaskRepository taskRepository,
            ITaskCommentRepository taskCommentRepository,
            PresenceTracker presenceTracker
            )
        {
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _projectMemberRepository = projectMemberRepository;
            _notificationRepository = notificationRepository;
            _taskRepository = taskRepository;
            _taskCommentRepository = taskCommentRepository;
            _presenceTracker = presenceTracker;
        }
        public override async System.Threading.Tasks.Task OnConnectedAsync()
        {
            var UserId = Context.User.FindFirstValue("UserId");
            if (!String.IsNullOrEmpty(UserId))
            {
                //var userName = _capstoneContext.Users.FirstOrDefault(u => u.UserId.ToString() == UserId).UserName;
                await _presenceTracker.UserConnected(UserId, Context.ConnectionId);
            }
        }
        public override async System.Threading.Tasks.Task OnDisconnectedAsync(Exception? exception)
        {
            var UserId = Context.User.FindFirstValue("UserId");
            await _presenceTracker.UserDisconnected(UserId, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        public async System.Threading.Tasks.Task SendNotificationChangeTaskStatus(string taskId, string userId)
        {
            var task = await _taskRepository.GetQuery().FirstOrDefaultAsync(x => x.TaskId.ToString() == taskId);
            if (task == null) return;
            if (task.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.ToDo
                || task.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.Done
                || task.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.Deleted) return;

            var lstProjectAdmin = task.Interation.Board.Project.ProjectMembers
                .Where(x => x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor).Select(y => y.UserId);
            var createdBy = await _projectMemberRepository.GetQuery().FirstOrDefaultAsync(x => x.MemberId == task.CreateBy);
            var listReceiver = lstProjectAdmin;
            var title = "";
            var description = "";
            var TargetUrl = $"https://devtasker.azurewebsites.net/task/{taskId}";

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
                NotificationId = new Guid(),
                Title = title,
                Description = description,
                CreateAt = DateTime.Now,
                TargetUrl = TargetUrl,
                IsRead = false,
                RecerverId = id
            });
            foreach(var notif in listNotification)
            {
                await _notificationRepository.UpdateAsync(notif);
            }
            
            await _notificationRepository.SaveChanges();

            //send mail for admins
            await SendMailForNotification(lstProjectAdmin.ToList(), listNotification.ToList());

            foreach(var user in listReceiver)
            {
                if (!await _presenceTracker.IsOnlineUser(user.ToString()))
                {
                    continue;
                }
                await Clients.User(user.ToString()).SendAsync("EmitNotification", new NotificationMessage
                {
                    Message = "You got new Notifications",
                    Reload = true,
                });
            }
            
        }
        public async System.Threading.Tasks.Task SendNotificationChangeProjectStatus(string projectId, string userId)
        {
            var project = await _projectRepository.GetQuery().FirstOrDefaultAsync(x => x.ProjectId.ToString() == projectId);
            if (project == null) return;
            var lstReceived = project.ProjectMembers.Where(x => x.UserId.ToString() != userId).Select(x => x.UserId);
            var lstNotification = lstReceived.Select(x => new Notification
            {
                NotificationId = new Guid(),
                Title = "Status change",
                Description = $"The status of project {project.ProjectName} has been changed to {project.Status.Title}",
                CreateAt = DateTime.Now,
                IsRead = false,
                RecerverId = x,
                TargetUrl = $"https://devtasker.azurewebsites.net/project/{projectId}"
            }).ToList();
            foreach (var notif in lstNotification)
            {
                await _notificationRepository.UpdateAsync(notif);
            }
            await _notificationRepository.SaveChanges();
            //Send Mail
            var lstProjectAdmin = project.ProjectMembers
                .Where(x => x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor).Select(y => y.UserId);
            await SendMailForNotification(lstProjectAdmin.ToList(), lstNotification.ToList());

            foreach (var user in lstReceived)
            {
                if (!await _presenceTracker.IsOnlineUser(user.ToString()))
                {
                    continue;
                }
                await Clients.User(user.ToString()).SendAsync("EmitNotification");
            }
        }

        public async System.Threading.Tasks.Task SendNotificationCommentTask(string commentId, string userId, string action)
        {
            var comment = await _taskCommentRepository.GetQuery().FirstOrDefaultAsync(x => x.CommentId.ToString() == commentId);
            if (comment == null) return;
            var lstReceived = comment.Task.Interation.Board.Project.ProjectMembers
                .Where(x => x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor || x.MemberId == comment.Task.AssignTo)
                .Select(y => new
                {
                    UserId = y.UserId,
                    Name = y.Users.UserName,
                    Role = y.Role.RoleName
                }).ToList();

            var lstNotification = new List<Notification>();
            switch (action)
            {
                case CommentActionCconstant.Create:
                    lstNotification = lstReceived.Select(x => new Notification
                    {
                        NotificationId = new Guid(),
                        Title = "Comment created",
                        Description = $"A comment has been created in task {comment.Task.Title} by {comment.ProjectMember.Users.UserName} ",
                        CreateAt = DateTime.Now,
                        IsRead = false,
                        RecerverId = x.UserId,
                        TargetUrl = $"https:localhost/{comment.TaskId}"
                    }).ToList();
                    break;

                case CommentActionCconstant.Edit:
                    lstNotification = lstReceived.Select(x => new Notification
                    {
                        NotificationId = new Guid(),
                        Title = "Comment status change",
                        Description = $"Comment has been edited in task {comment.Task.Title} by {comment.ProjectMember.Users.UserName} ",
                        CreateAt = DateTime.Now,
                        IsRead = false,
                        RecerverId = x.UserId,
                        TargetUrl = $"https:localhost/{comment.TaskId}"
                    }).ToList();
                    break;

                case CommentActionCconstant.Delete:
                    lstNotification = lstReceived.Select(x => new Notification
                    {
                        NotificationId = new Guid(),
                        Title = "Comment status change",
                        Description = $"A comment has been deleted in task {comment.Task.Title} by {comment.ProjectMember.Users.UserName} ",
                        CreateAt = DateTime.Now,
                        IsRead = false,
                        RecerverId = x.UserId,
                        TargetUrl = $"https://devtasker.azurewebsites.net/task/{comment.TaskId}"
                    }).ToList();
                    break;

            }
            foreach (var notif in lstNotification)
            {
                await _notificationRepository.UpdateAsync(notif);
            }
            foreach (var user in lstReceived)
            {
                if (!await _presenceTracker.IsOnlineUser(user.ToString()))
                {
                    continue;
                }
                await Clients.User(user.ToString()).SendAsync("EmitNotification");
            }
            await _notificationRepository.SaveChanges();

            //Send Email
            var lstProjectAdmin = lstReceived.Where(x => x.Role == RoleNameConstant.ProductOwner || x.Role == RoleNameConstant.Supervisor).Select(d => d.UserId);
            await SendMailForNotification(lstProjectAdmin.ToList(), lstNotification.ToList());

        }
        public async System.Threading.Tasks.Task SendMailForNotification(List<Guid> lstReceiver, List<Notification> listNotifications)
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
        }
        public async System.Threading.Tasks.Task Send(string to, string subject, string body)
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
    public class NotificationMessage
    {
        public string Message { get; set; }
        public bool Reload { get; set; }
    }
}
