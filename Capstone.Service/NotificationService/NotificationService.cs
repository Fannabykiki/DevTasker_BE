using AutoMapper;
using AutoMapper.Execution;
using Capstone.Common.Constants;
using Capstone.Common.DTOs.Notification;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Task;
using Capstone.Common.Enums;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        private readonly IBoardStatusRepository _boardStatusRepository;
        private readonly PresenceTracker _presenceTracker;

        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly IMapper _mapper;
        public NotificationService(INotificationRepository notificationRepository,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            IProjectMemberRepository projectMemberRepository,
            ITaskRepository taskRepository,
            ITaskCommentRepository taskCommentRepository,
            IBoardStatusRepository boardStatusRepository,
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
            _boardStatusRepository = boardStatusRepository;
            _presenceTracker = presenceTracker;
            _hubContext = hubContext;
            _mapper = mapper;
        }
        public async Task<List<NotificationViewModel>> GetLatestNotifications(Guid userId, int page = 10)
        {
            var results = await _notificationRepository.GetQuery().Where(x => x.RecerverId == userId).OrderByDescending(y => y.CreateAt).Take(page)
                .Select(x=>new NotificationViewModel
                {
                    NotificationId = x.NotificationId,
                    Title = x.Title,
                    Description = x.Description,
                    CreateAt = x.CreateAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    IsRead = x.IsRead,
                    RecerverId = x.RecerverId,
                    TargetUrl = x.TargetUrl
                })
                .ToListAsync();
            return results;
        }
        public async Task<List<NotificationViewModel>> GetAllNotificationsByUser(Guid userId)
        {
            var results = await _notificationRepository.GetQuery().Where(x => x.RecerverId == userId).OrderByDescending(y => y.CreateAt)
                .Select(x => new NotificationViewModel
                {
                    NotificationId = x.NotificationId,
                    Title = x.Title,
                    Description = x.Description,
                    CreateAt = x.CreateAt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    IsRead = x.IsRead,
                    RecerverId = x.RecerverId,
                    TargetUrl = x.TargetUrl
                })
                .ToListAsync();
            return results;
        }
        public async Task<bool> MarkReadNotification(Guid userId, ReadNotificationRequest request)
        {
            try
            {
                var listReadNotification = _notificationRepository.GetQuery().Where(x => x.RecerverId == userId && !x.IsRead);
                if (request.ListNotificationIds != null && request.ListNotificationIds.Count > 0)
                {
                    listReadNotification = listReadNotification.Where(x => request.ListNotificationIds.Contains(x.NotificationId));
                }
                foreach (var notification in listReadNotification)
                {
                    notification.IsRead = true;
                    await _notificationRepository.UpdateAsync(notification);
                }
                return await _notificationRepository.SaveChanges() > 0;
            }catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public async System.Threading.Tasks.Task SendNotificationChangeProjectStatus( Guid projectId, Guid userId)
        {
            var project = await _projectRepository.GetQuery().Include("Status").FirstOrDefaultAsync(x => x.ProjectId == projectId);
            var projectMembers = await _projectMemberRepository.GetQuery().Include("Role").Where(x => x.ProjectId == projectId).ToListAsync();
            if (project == null) return;
            var currentUser = await _userRepository.GetAsync(x => x.UserId == userId, null);
            if (currentUser == null) return;
            var lstReceived = projectMembers.Where(x => x.UserId != userId).Select(x => x.UserId);
            var lstNotification = lstReceived.Select(x => new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = "Project status has been changed",
                Description = $"User <strong>{currentUser.UserName}</strong> has change the status of project <strong>{project.ProjectName}</strong> to <strong>{project.Status.Title}</strong>",
                CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
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
        public async System.Threading.Tasks.Task SendNotificationChangeTaskStatus(Guid taskId, Guid userId)
        {
            var task = await _taskRepository.GetQuery()
                .Include(t=>t.ProjectMember)
                .Include(x => x.Status)
                .Include(t => t.Interation)
                .ThenInclude(it => it.Board)
                .ThenInclude(b => b.Project)
                .ThenInclude(prj=>prj.ProjectMembers).ThenInclude(prjMem => prjMem.Role)
                .Include(tc=>tc.TaskHistories)
                .FirstOrDefaultAsync(x => x.TaskId == taskId);

            if (task == null) return;
            //if (task.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.ToDo
            //    && task.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.Done
            //    && task.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.Deleted) return;

            var lstProjectAdmin = task.Interation.Board.Project.ProjectMembers
                .Where(x => (x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor) && x.UserId!= userId ).Select(y => y.UserId);
            var createdBy = await _projectMemberRepository.GetQuery().FirstOrDefaultAsync(x => x.UserId == task.CreateBy);
            var listReceiver = lstProjectAdmin;

            var title = "";
            var description = "";
            var testLatest = task.TaskHistories.OrderByDescending(x => x.ChangeAt).ToList();
            var latestTaskhistory = task.TaskHistories.OrderByDescending(x=>x.ChangeAt).FirstOrDefault();
            var TargetUrl = $"https://devtasker.azurewebsites.net/project/{task.Interation.BoardId}/tasks?id={task.TaskId}";
            
            var userAccount = _userRepository.GetQuery().FirstOrDefault(x => x.UserId == userId);
            if(task.ProjectMember.UserId != userId)
            {
                listReceiver = listReceiver.Append(task.ProjectMember.UserId).Distinct();
            }
            if (createdBy.UserId != userId)
            {
                listReceiver = listReceiver.Append(createdBy.UserId).Distinct();
            }
            
            if(latestTaskhistory== null)
            {
                title = "Task Is Ready";
                description = $"User <strong>{userAccount?.UserName}</strong> has created task <strong>{task.Title}</strong> in project <strong>{task.Interation.Board.Project.ProjectName}</strong> at <strong>{task.CreateTime}</strong>";
            }
            else
            {
                if (task.IsDelete.HasValue && task.IsDelete.Value)
                {
                    title = "Task Deleted";
                    description = $"User <strong>{userAccount?.UserName}</strong> has deleted task <strong>{task.Title}</strong> in project <strong>{task.Interation.Board.Project.ProjectName}</strong> at <strong>{DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"))}</strong>";
                }
                else
                {
                    title = "Task Updated!";
                    var previousStatus = await _boardStatusRepository.GetQuery().FirstOrDefaultAsync(x => x.BoardStatusId == latestTaskhistory.PreviousStatusId);
                    if (previousStatus != null)
                    {
                        if (task.StatusId == previousStatus.BoardStatusId)
                        {
                            description = $"User <strong>{userAccount?.UserName}</strong> has updated task <strong>{task.Title}</strong> in project <strong>{task.Interation.Board.Project.ProjectName}</strong> at <strong>{task.CreateTime}</strong>";
                        }
                        else
                        {
                            description = $"User <strong>{userAccount?.UserName}</strong> has changed status of task <strong>{task.Title}</strong> in project <strong>{task.Interation.Board.Project.ProjectName}</strong> from <strong>{previousStatus.Title}</strong> to <strong>{task.Status.Title}</strong> at <strong>{task.CreateTime}</strong>";
                        }

                    }
                    else
                    {
                        description = $"User <strong>{userAccount?.UserName}</strong> has change status of task <strong>{task.Title}</strong> in project <strong>{task.Interation.Board.Project.ProjectName}</strong> to <strong>{task.Title}</strong> at <strong>{task.CreateTime}</strong>";
                    }
                }
                
            }
            
            //switch (task.Status.Title)
            //{
            //    case CapstoneNameConstant.TaskStatusNameConstant.ToDo:


            //        if(latestTaskhistory == null)
            //        {
            //            title = "Task Is Ready";
            //            description = $"User <strong>{userAccount?.UserName}</strong> has created task <strong>{task.Title}</strong> in project <strong>{task.Interation.Board.Project}</strong> at <strong>{task.CreateTime}</strong>";
            //        }
            //        else
            //        {
            //            title = "Task status changed"
            //            description = $"The task <strong>{task.Title}</strong> has been set to <strong>{CapstoneNameConstant.TaskStatusNameConstant.ToDo}</strong> by <strong>{userAccount?.UserName}</strong> ";
            //        }
            //        listReceiver = listReceiver.Append(task.ProjectMember.UserId).Distinct();
            //        break;

            //    case CapstoneNameConstant.TaskStatusNameConstant.Done:

            //        title = "[Task Is Done]";
            //        description = $"The task <strong>{task.Title}</strong> has been set to <strong>{CapstoneNameConstant.TaskStatusNameConstant.Done}</strong> by <strong>{userAccount?.UserName}</strong> ";
            //        listReceiver = listReceiver.Append(task.ProjectMember.UserId).Distinct();
            //        listReceiver = listReceiver.Append(createdBy.UserId).Distinct();
            //        break;

            //    case CapstoneNameConstant.TaskStatusNameConstant.Deleted:

            //        title = "[Task Is Deleted]";
            //        description = $"The task <strong>{task.Title}</strong> has been set to <strong>{CapstoneNameConstant.TaskStatusNameConstant.Deleted}</strong> by <strong>{userAccount?.UserName}</strong> ";
            //        listReceiver = listReceiver.Append(task.ProjectMember.UserId).Distinct();
            //        listReceiver = listReceiver.Append(createdBy.UserId).Distinct();
            //        break;

            //}
            var listNotification = listReceiver.Select(id => new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = title,
                Description = description,
                CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                TargetUrl = TargetUrl,
                IsRead = false,
                RecerverId = id
            });
            foreach (var notif in listNotification)
            {
                await _notificationRepository.CreateAsync(notif);
            }

            await _notificationRepository.SaveChanges();

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
        public async System.Threading.Tasks.Task SendNotificationCreateTask(Guid taskId, Guid userId)
        {
            var task = await _taskRepository.GetQuery()
                .Include(t => t.ProjectMember)
                .Include(x => x.Status)
                .Include(t => t.Interation)
                .ThenInclude(it => it.Board)
                .ThenInclude(b => b.Project)
                .ThenInclude(prj => prj.ProjectMembers).ThenInclude(prjMem => prjMem.Role)
                .Include(tc => tc.TaskHistories)
                .FirstOrDefaultAsync(x => x.TaskId == taskId);

            if (task == null) return;
            var lstProjectAdmin = task.Interation.Board.Project.ProjectMembers
                .Where(x => (x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor) && x.UserId != userId).Select(y => y.UserId);
            var createdBy = await _projectMemberRepository.GetQuery().FirstOrDefaultAsync(x => x.UserId == task.CreateBy);
            var listReceiver = lstProjectAdmin;

            var title = "";
            var description = "";
            var testLatest = task.TaskHistories.OrderByDescending(x => x.ChangeAt).ToList();
            var latestTaskhistory = task.TaskHistories.OrderByDescending(x => x.ChangeAt).FirstOrDefault();
            var TargetUrl = $"https://devtasker.azurewebsites.net/project/{task.Interation.BoardId}/tasks?id={task.TaskId}";

            var userAccount = _userRepository.GetQuery().FirstOrDefault(x => x.UserId == userId);
            if (task.ProjectMember.UserId != userId)
            {
                listReceiver = listReceiver.Append(task.ProjectMember.UserId).Distinct();
            }
            title = "New Task Created";
            description = $"User <strong>{userAccount?.UserName}</strong> created task <strong>{task.Title}</strong> in project <strong>{task.Interation.Board.Project.ProjectName}</strong>";
            var descriptionForAssign = $"User <strong>{userAccount?.UserName}</strong> assigned task <strong>{task.Title}</strong> in project <strong>{task.Interation.Board.Project.ProjectName}</strong> to you</strong>";
            


            var listNotification = listReceiver.Select(id => new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = title,
                Description = id == task.ProjectMember.UserId? descriptionForAssign : description,
                CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                TargetUrl = TargetUrl,
                IsRead = false,
                RecerverId = id
            });
            foreach (var notif in listNotification)
            {
                await _notificationRepository.CreateAsync(notif);
            }

            await _notificationRepository.SaveChanges();

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
        public async System.Threading.Tasks.Task SendNotificationUpdateTask(Guid taskId, Guid userId, TaskDetailViewModel oldTask)
        {
            var task = await _taskRepository.GetQuery()
                .Include(t => t.ProjectMember)
                .Include(x => x.Status)
                .Include(t => t.Interation)
                .ThenInclude(it => it.Board)
                .ThenInclude(b => b.Project)
                .ThenInclude(prj => prj.ProjectMembers).ThenInclude(prjMem => prjMem.Role)
                .Include(tc => tc.TaskHistories)
                .FirstOrDefaultAsync(x => x.TaskId == taskId);

            if (task == null) return;
            var lstProjectAdmin = task.Interation.Board.Project.ProjectMembers
                .Where(x => (x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor) && x.UserId != userId).Select(y => y.UserId);
            var createdBy = await _projectMemberRepository.GetQuery().FirstOrDefaultAsync(x => x.UserId == task.CreateBy);
            var listReceiver = lstProjectAdmin;

            var title = "";
            var description = "";
            var testLatest = task.TaskHistories.OrderByDescending(x => x.ChangeAt).ToList();
            var latestTaskhistory = task.TaskHistories.OrderByDescending(x => x.ChangeAt).FirstOrDefault();
            var TargetUrl = $"https://devtasker.azurewebsites.net/project/{task.Interation.BoardId}/tasks?id={task.TaskId}";

            var userAccount = _userRepository.GetQuery().FirstOrDefault(x => x.UserId == userId);
            if (task.ProjectMember.UserId != userId)
            {
                listReceiver = listReceiver.Append(task.ProjectMember.UserId).Distinct();
            }
            if (createdBy.UserId != userId)
            {
                listReceiver = listReceiver.Append(createdBy.UserId).Distinct();
            }
            title = "Task Updated";
            var previousStatus = await _boardStatusRepository.GetQuery().FirstOrDefaultAsync(x => x.BoardStatusId == latestTaskhistory.PreviousStatusId);
            if (previousStatus != null)
            {
                if (task.StatusId == previousStatus.BoardStatusId)
                {
                    description = $"User <strong>{userAccount?.UserName}</strong> has updated task <strong>{task.Title}</strong> in project <strong>{task.Interation.Board.Project.ProjectName}</strong> at <strong>{task.CreateTime}</strong>";
                }
                else
                {
                    description = $"User <strong>{userAccount?.UserName}</strong> has changed status of task <strong>{task.Title}</strong> in project <strong>{task.Interation.Board.Project.ProjectName}</strong> from <strong>{previousStatus.Title}</strong> to <strong>{task.Status.Title}</strong>";
                }

            }
            else
            {
                description = $"User <strong>{userAccount?.UserName}</strong> has change status of task <strong>{task.Title}</strong> in project <strong>{task.Interation.Board.Project.ProjectName}</strong> to <strong>{task.Title}</strong> ";
            }
            

            bool isSendEmail = false;
            if((task.Status.Title == CapstoneNameConstant.TaskStatusNameConstant.ToDo||task.Status.Title == CapstoneNameConstant.TaskStatusNameConstant.Done) 
                && task.StatusId == previousStatus.BoardStatusId)
            {
                isSendEmail = true ;
            } 
            var listNotification = listReceiver.Select(id => new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = title,
                Description = description,
                CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                TargetUrl = TargetUrl,
                IsRead = false,
                RecerverId = id
            });
            foreach (var notif in listNotification)
            {
                await _notificationRepository.CreateAsync(notif);
            }

            await _notificationRepository.SaveChanges();

            foreach (var user in listReceiver)
            {
                if (!await _presenceTracker.IsOnlineUser(user.ToString()))
                {
                    continue;
                }
                await _hubContext.Clients.Group(user.ToString()).SendAsync("EmitNotification");
            }
            //send mail for admins
            if (isSendEmail)
            {
                await SendMailForNotification(lstProjectAdmin.ToList(), listNotification.ToList());
            }
            
        }
        public async System.Threading.Tasks.Task SendNotificationDeleteTask(Guid taskId, Guid userId)
        {
            var task = await _taskRepository.GetQuery()
                .Include(t => t.ProjectMember)
                .Include(x => x.Status)
                .Include(t => t.Interation)
                .ThenInclude(it => it.Board)
                .ThenInclude(b => b.Project)
                .ThenInclude(prj => prj.ProjectMembers).ThenInclude(prjMem => prjMem.Role)
                .Include(tc => tc.TaskHistories)
                .FirstOrDefaultAsync(x => x.TaskId == taskId);

            if (task == null) return;
            var lstProjectAdmin = task.Interation.Board.Project.ProjectMembers
                .Where(x => (x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor) && x.UserId != userId).Select(y => y.UserId);
            var createdBy = await _projectMemberRepository.GetQuery().FirstOrDefaultAsync(x => x.UserId == task.CreateBy);
            var listReceiver = lstProjectAdmin;

            var userAccount = _userRepository.GetQuery().FirstOrDefault(x => x.UserId == userId);
            var title = "Task Deleted";
            var description = $"User <strong>{userAccount?.UserName}</strong> has deleted task <strong>{task.Title}</strong> in project <strong>{task.Interation.Board.Project.ProjectName}</strong> at <strong>{DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"))}</strong>"; 
            var TargetUrl = $"https://devtasker.azurewebsites.net/project/{task.Interation.BoardId}/trash?id={task.TaskId}";

            
            if (task.ProjectMember.UserId != userId)
            {
                listReceiver = listReceiver.Append(task.ProjectMember.UserId).Distinct();
            }
            if (createdBy.UserId != userId)
            {
                listReceiver = listReceiver.Append(createdBy.UserId).Distinct();
            }
            
            
            var listNotification = listReceiver.Select(id => new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = title,
                Description = description,
                CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                TargetUrl = TargetUrl,
                IsRead = false,
                RecerverId = id
            });
            foreach (var notif in listNotification)
            {
                await _notificationRepository.CreateAsync(notif);
            }

            await _notificationRepository.SaveChanges();

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
        public async System.Threading.Tasks.Task SendNotificationRestoreTask(Guid taskId, Guid userId)
        {
            var task = await _taskRepository.GetQuery()
                .Include(t => t.ProjectMember)
                .Include(x => x.Status)
                .Include(t => t.Interation)
                .ThenInclude(it => it.Board)
                .ThenInclude(b => b.Project)
                .ThenInclude(prj => prj.ProjectMembers).ThenInclude(prjMem => prjMem.Role)
                .Include(tc => tc.TaskHistories)
                .FirstOrDefaultAsync(x => x.TaskId == taskId);
            if (task == null) return;
            var lstProjectAdmin = task.Interation.Board.Project.ProjectMembers
                .Where(x => (x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor) && x.UserId != userId).Select(y => y.UserId);
            var createdBy = await _projectMemberRepository.GetQuery().FirstOrDefaultAsync(x => x.UserId == task.CreateBy);
            var listReceiver = lstProjectAdmin;

            var userAccount = _userRepository.GetQuery().FirstOrDefault(x => x.UserId == userId);
            var title = "Task Restored";
            var description = $"User <strong>{userAccount?.UserName}</strong> restored task <strong>{task.Title}</strong> in project <strong>{task.Interation.Board.Project.ProjectName}</strong> at <strong>{DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"))}</strong>";
            var TargetUrl = $"https://devtasker.azurewebsites.net/project/{task.Interation.BoardId}/tasks?id={task.TaskId}";

            if (task.ProjectMember.UserId != userId)
            {
                listReceiver = listReceiver.Append(task.ProjectMember.UserId).Distinct();
            }
            if (createdBy.UserId != userId)
            {
                listReceiver = listReceiver.Append(createdBy.UserId).Distinct();
            }


            var listNotification = listReceiver.Select(id => new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = title,
                Description = description,
                CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                TargetUrl = TargetUrl,
                IsRead = false,
                RecerverId = id
            });
            foreach (var notif in listNotification)
            {
                await _notificationRepository.CreateAsync(notif);
            }

            await _notificationRepository.SaveChanges();

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
        public async System.Threading.Tasks.Task SendNotificationChangeRole(Guid memberId, Guid userId)
        {
            //var tasks = await _taskRepository.GetAllWithOdata(t => t.AssignTo == memberId,x => x.Interation);
            var tasks = _taskRepository.GetQuery()
                .Include(t => t.ProjectMember)
                .Include(x => x.Status)
                .Include(t => t.Interation)
                .ThenInclude(it => it.Board)
                .ThenInclude(b => b.Project)
                .ThenInclude(prj => prj.ProjectMembers).ThenInclude(prjMem => prjMem.Role)
                .Include(tc => tc.TaskHistories)
                .Where(x => x.AssignTo == memberId);
            if (tasks == null) return;
            var lstProjectAdmin = tasks.First().Interation.Board.Project.ProjectMembers
                .Where(x => (x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor) && x.UserId != userId).Select(y => y.UserId);
            var createdBy = tasks.Select(x => x.CreateBy);
            var listReceiver = lstProjectAdmin;
            var project = await _projectRepository.GetAsync(x => x.ProjectId == tasks.First().Interation.BoardId, null);
            var userAccount = _userRepository.GetQuery().FirstOrDefault(x => x.UserId == userId);
            var member = _projectMemberRepository.GetQuery().Include(x => x.Role).Include(u => u.Users).FirstOrDefault(x => x.UserId == userId);
            var title = "Member has been changed role";
            var description = $"User <strong>{userAccount?.UserName}</strong> has assigned Role <strong>{member.Role.RoleName}</strong> for <strong>{member.Users.UserName}</strong>, all <strong>{member.Users.UserName}'s</strong> remaining task in project <strong>{project.ProjectName}</strong> will assigned to <strong>{userAccount?.UserName}</strong> at <strong>{DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"))}</strong>"; 
            var TargetUrl = $"https://devtasker.azurewebsites.net/project/{project.ProjectId}";

            foreach(var task in tasks)
            {
                if (task.ProjectMember.UserId != userId)
                {
                    listReceiver = listReceiver.Append(task.ProjectMember.UserId).Distinct();
                }
                if (task.CreateBy != userId)
                {
                    listReceiver = listReceiver.Append(task.CreateBy).Distinct();
                }
            }
            
            
            
            var listNotification = listReceiver.Select(id => new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = title,
                Description = tasks.Count() == 0 ? $"User <strong>{userAccount?.UserName}</strong> has assigned Role <strong>{member.Role.RoleName}</strong> for <strong>{member.Users.UserName}</strong> at <strong>{DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"))}</strong>" : description,
                CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                TargetUrl = TargetUrl,
                IsRead = false,
                RecerverId = id
            });
            foreach (var notif in listNotification)
            {
                await _notificationRepository.CreateAsync(notif);
            }

            await _notificationRepository.SaveChanges();

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
        public async System.Threading.Tasks.Task SendNotificationCommentTask(Guid commentId, Guid userId, string action)
        {
            var comment = await _taskCommentRepository.GetQuery().FirstOrDefaultAsync(x => x.CommentId == commentId);
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
            var cmtUser = await _userRepository.GetAsync(x => x.UserId == userId, null);
            if (cmtUser == null) return;
            
            
            comment.Task = task;
            var lstReceived = comment.Task.Interation.Board.Project.ProjectMembers
                .Where(x => x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor || x.MemberId == comment.Task.AssignTo)
                .Select(y => new
                {
                    UserId = y.UserId,
                    MemberId = y.MemberId,
                    Name = y.Users.UserName,
                    Role = y.Role.RoleName
                }).Where(dt=>dt.UserId!= userId).ToList();

            var lstNotification = new List<Notification>();
            switch (action)
            {
                case CommentActionCconstant.Create:
                    lstNotification = lstReceived.Select(x => new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "New comment in task",
                        Description = $"<strong>{cmtUser.UserName}</strong> commented on task <strong>{comment.Task.Title}</strong> of project <strong>{comment.Task.Interation.Board.Project.ProjectName}</strong>",
                        CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                        IsRead = false,
                        RecerverId = x.UserId,
                        TargetUrl = $"https://devtasker.azurewebsites.net/project/{comment.Task.Interation.BoardId}/tasks?id={comment.TaskId}"
                    }).ToList();
                    break;
                    
                case CommentActionCconstant.Edit:
                    lstNotification = lstReceived.Select(x => new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Comment edited in task",
                        Description = $"<strong>{cmtUser.UserName}</strong> has edited {(!cmtUser.Gender.HasValue? "their" : (cmtUser.Gender.Value == GenderEnum.Male? "his" : "her")) } comment in task <strong>{comment.Task.Title}</strong> of project <strong>{comment.Task.Interation.Board.Project.ProjectName}</strong>",
                        CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                        IsRead = false,
                        RecerverId = x.UserId,
                        TargetUrl = $"https://devtasker.azurewebsites.net/project/{comment.Task.Interation.BoardId}/tasks?id={comment.TaskId}"
                    }).ToList();
                    break;

                case CommentActionCconstant.Delete:
                    lstNotification = lstReceived.Select(x => new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "Comment status change",
                        Description = x.MemberId == comment.CreateBy? $"Your comment in task <strong>{comment.Task.Title}</strong> has been deleted by <strong>{cmtUser.UserName}</strong>" : $"Comment of <strong>{x.Name}</strong> been deleted in task <strong>{comment.Task.Title}</strong> by <strong>{cmtUser.UserName}</strong>",
                        CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                        IsRead = false,
                        RecerverId = x.UserId,
                        TargetUrl = $"https://devtasker.azurewebsites.net/project/{comment.Task.Interation.BoardId}/tasks?id={comment.TaskId}"
                    }).ToList();
                    break;
                case CommentActionCconstant.Reply:
                    
                    lstNotification = lstReceived.Select(x => new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        Title = "New comment in task",
                        Description = $"<strong>{cmtUser.UserName}</strong> replied to a comment in task <strong>{comment.Task.Title}</strong> of project <strong>{comment.Task.Interation.Board.Project.ProjectName}</strong>",
                        CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                        IsRead = false,
                        RecerverId = x.UserId,
                        TargetUrl = $"https://devtasker.azurewebsites.net/project/{comment.Task.Interation.BoardId}/tasks?id={comment.TaskId}"
                    }).ToList();
                    if (comment.ReplyTo.HasValue)
                    {
                        var replyComment = await _taskCommentRepository.GetQuery().Include(x => x.ProjectMember).FirstOrDefaultAsync(cm => cm.CommentId == comment.ReplyTo);
                        var replyNotification = lstNotification.FirstOrDefault(x => x.RecerverId == replyComment.ProjectMember.UserId);

                        if(replyNotification == null)
                        {
                            lstNotification.Add(new Notification
                            {
                                NotificationId = Guid.NewGuid(),
                                Title = "New comment in task",
                                Description = $"<strong>{cmtUser.UserName}</strong> replied to your comment in task <strong>{comment.Task.Title}</strong> of project <strong>{comment.Task.Interation.Board.Project.ProjectName}</strong>",
                                CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                                IsRead = false,
                                RecerverId =replyComment.ProjectMember.UserId,
                                TargetUrl = $"https://devtasker.azurewebsites.net/project/{comment.Task.Interation.BoardId}/tasks?id={comment.TaskId}"
                            });
                        }
                        else
                        {
                            replyNotification.Description = $"<strong>{cmtUser.UserName}</strong> replied to your comment in task <strong>{comment.Task.Title}</strong> of project <strong>{comment.Task.Interation.Board.Project.ProjectName}</strong>";
                        }

                    }
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
        public async System.Threading.Tasks.Task RemoveMemberNotification(Guid userId, Guid projectId, Guid removedMemberUserId)
        {
            var currentUser = await _userRepository.GetQuery().FirstOrDefaultAsync(x => x.UserId == userId);
            var project = await _projectRepository.GetQuery().FirstOrDefaultAsync(x => x.ProjectId == projectId);
            if (project == null) return;

            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = "Removed from project",
                Description = $"You have been removed from project <strong>{project.ProjectName}</strong> by user <strong>{currentUser.UserName}</strong>",
                CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                TargetUrl = "",
                IsRead = false,
                RecerverId = removedMemberUserId
            };
            await _notificationRepository.CreateAsync(notification);
            await _notificationRepository.SaveChanges();
            if (await _presenceTracker.IsOnlineUser(removedMemberUserId.ToString()))
            {
                await _hubContext.Clients.Group(removedMemberUserId.ToString()).SendAsync("EmitNotification");
            }
            await SendMailForNotification(new List<Guid>() { removedMemberUserId}, new List<Notification>() { notification });

        }
        public async System.Threading.Tasks.Task ExitProjectNotification(Guid userId, Guid projectId)
        {
            var project = await _projectRepository.GetQuery().Include("Status").FirstOrDefaultAsync(x => x.ProjectId == projectId);
            var projectMembers = await _projectMemberRepository.GetQuery().Include("Role").Where(x => x.ProjectId == projectId).ToListAsync();
            if (project == null) return;
            var currentUser = await _userRepository.GetAsync(x => x.UserId == userId, null);
            if (currentUser == null) return;

            var lstReceived = projectMembers.Where(x => x.UserId != userId && x.Role.RoleName == RoleNameConstant.ProductOwner).Select(x => x.UserId);
            var lstNotification = lstReceived.Select(x => new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = "Member exit project",
                Description = $"User <strong>{currentUser.UserName}</strong> exited project <strong>{project.ProjectName}</strong>",
                CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                IsRead = false,
                RecerverId = x,
                TargetUrl = ""
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
            await SendMailForNotification(lstReceived.ToList(), lstNotification.ToList());
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
                .Where(x => (x.DueDate <= DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")).AddDays(3) && x.DueDate >= DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")).AddDays(2)
                || x.DueDate <= DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")).AddDays(1)) && 
                !x.IsDelete.Value && x.Status.Title != CapstoneNameConstant.TaskStatusNameConstant.Done)
                .ToListAsync();

            var t = await _taskRepository.GetQuery()
                .Select(x => new
                {
                    DueDate = x.DueDate,
                    Now = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                    NowAdd3Day = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")).AddDays(3),
                    NowAdd2Day = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")).AddDays(2),
                    NowAdd1Day = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")).AddDays(1)
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
                            CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                            IsRead = false,
                            RecerverId = dt.Users.UserId,
                            TargetUrl = $"https://devtasker.azurewebsites.net/project/{task.Interation.BoardId}/tasks?id={task.TaskId}"
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
        public async System.Threading.Tasks.Task SendNotificationTaskFailedDeadline(Guid taskId, Guid userId)
        {
            var task = await _taskRepository.GetQuery()
                .Include(t => t.ProjectMember)
                .Include(x => x.Status)
                .Include(t => t.Interation)
                .ThenInclude(it => it.Board)
                .ThenInclude(b => b.Project)
                .ThenInclude(prj => prj.ProjectMembers).ThenInclude(prjMem => prjMem.Role)
                .Include(tc => tc.TaskHistories)
                .FirstOrDefaultAsync(x => x.TaskId == taskId);

            if (task == null) return;
            var lstProjectAdmin = task.Interation.Board.Project.ProjectMembers
                .Where(x => (x.Role.RoleName == RoleNameConstant.ProductOwner || x.Role.RoleName == RoleNameConstant.Supervisor) && x.UserId != userId).Select(y => y.UserId);
            var createdBy = await _projectMemberRepository.GetQuery().FirstOrDefaultAsync(x => x.UserId == task.CreateBy);
            var listReceiver = lstProjectAdmin;

            var title = "Overdue Task Failed";
            var description = $"<strong>{task.Title}</strong>  in project project <strong>{task.Interation.Board.Project.ProjectName}</strong> is due at <strong>{DateTime.Parse(task.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"))}</strong> ";
            var TargetUrl = $"https://devtasker.azurewebsites.net/project/{task.Interation.BoardId}/tasks?id={task.TaskId}";
            var userAccount = _userRepository.GetQuery().FirstOrDefault(x => x.UserId == userId);
            if (task.ProjectMember.UserId != userId)
            {
                listReceiver = listReceiver.Append(task.ProjectMember.UserId).Distinct();
            }
            if (createdBy.UserId != userId)
            {
                listReceiver = listReceiver.Append(createdBy.UserId).Distinct();
            }
            
            var listNotification = listReceiver.Select(id => new Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = title,
                Description = description,
                CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                TargetUrl = TargetUrl,
                IsRead = false,
                RecerverId = id
            });
            foreach (var notif in listNotification)
            {
                await _notificationRepository.CreateAsync(notif);
            }

            await _notificationRepository.SaveChanges();

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
