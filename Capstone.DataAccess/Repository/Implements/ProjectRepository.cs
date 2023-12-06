using Capstone.Common.DTOs.Permission;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Role;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Capstone.DataAccess.Repository.Implements
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        private new readonly CapstoneContext _context;
        public ProjectRepository(CapstoneContext context) : base(context)
        {
            _context = context;
        }

        public async Task<GetProjectReportRequest> GetProjectReport(Guid projectId)
        {
            var project = await LoadProjectWithTasks(projectId);
            var tasks = GetTasksFromProject(project);
            var listStatus = _context.BoardStatus.Where(x => x.BoardId == projectId).ToList();
            var listMember = _context.ProjectMembers.Include(u => u.Users).ThenInclude(s => s.Status).Where(x => x.ProjectId == projectId && x.StatusId == Guid.Parse("BA888147-C90A-4578-8BA6-63BA1756FAC1")).ToList();
            var reportRecord = GenerateReportRecord(listStatus,tasks);
            var reportByWeek = GenerateReportByWeek(listStatus,tasks);
            var reportMembers = GenerateMemberReport(listMember, listStatus, tasks);
            

            var result = new GetProjectReportRequest
            {
                reportProject = reportRecord,
                reportRecordByWeek = reportByWeek,
                memberTaks = reportMembers
            };

            return result;
        }
        public async Task<List<GetProjectCalendarResponse>> GetProjectCalender(Guid projectId)
        {
            var result = new List<GetProjectCalendarResponse>();
            var curentMonth =new List<DateTime>();
            var project = await LoadProjectWithTasks(projectId);
            var tasks = GetTasksFromProject(project);

            DateTime currentDate = DateTime.Today;

            // Get the first day of the current month
            DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

            // Get the last day of the current month
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Loop through from the first day to the last day of the month
            for (DateTime date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
            {
                curentMonth.Add(date);
            }

            foreach (var day in curentMonth)
            {
                var taskResult = new GetProjectCalendarResponse();
                
                var taskByDay = tasks.Where(x => x.CreateTime <= day && x.DueDate >= day).ToList();
                taskResult.TotalTask = taskByDay.Count;
                taskResult.DateTime = day;
                taskResult.TasksByDay = new List<GetProjectTasksResponse>();
                foreach (var task in taskByDay)
                {
                    var assignTo = _context.ProjectMembers.Include(x => x.Users).ThenInclude(s => s.Status).FirstOrDefault(x => x.MemberId == task.AssignTo);

                    var createBy =  _context.Users.Include(s => s.Status).FirstOrDefault(x => x.UserId == task.CreateBy);

                    var taskType =  _context.TaskTypes.FirstOrDefault(x => x.TypeId == task.TypeId);
                    var priority = _context.PriorityLevels.FirstOrDefault(x => x.LevelId == task.PriorityId);

                    var newTask = new GetProjectTasksResponse();
                    newTask.TaskId = task.TaskId;
                    newTask.Title = task.Title;
                    newTask.Description = task.Description;
                    newTask.StartDate = task.StartDate == null? null : task.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                    newTask.DueDate = task.DueDate == null ? null : task.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                    newTask.CreateTime = task.CreateTime == null ? null : task.CreateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                    newTask.DeleteAt = task.DeleteAt == null ? null : task.DeleteAt.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
                    newTask.AssignTo = new UserResponse {
                        UserId = assignTo.UserId,
                        Address = assignTo.Users.Address,
                        Dob = assignTo.Users.Dob,
                        Email = assignTo.Users.Email,
                        IsAdmin = assignTo.Users.IsAdmin,
                        PhoneNumber = assignTo.Users.PhoneNumber,
                        StatusName = assignTo.Users.Status.Title,
                        UserName = assignTo.Users.UserName,
                    };
                    newTask.CreateBy = createBy == null ? null : new UserResponse
                    {
                        UserId = createBy.UserId,
                        Address = createBy.Address,
                        Dob = createBy.Dob,
                        Email = createBy.Email,
                        IsAdmin = createBy.IsAdmin,
                        PhoneNumber = createBy.PhoneNumber,
                        StatusName = createBy.Status.Title,
                        UserName = createBy.UserName,
                    }; 
                    newTask.TaskType = taskType.Title;
                    newTask.PrevId = task.PrevId;
                    newTask.StatusId = task.StatusId;
                    newTask.TaskStatus = _context.BoardStatus.FirstOrDefault(x => x.BoardStatusId == task.StatusId).Title;
                    newTask.Priority = priority.Title;
                    newTask.Interation = task.Interation.InterationName;
                    taskResult.TasksByDay.Add(newTask);
                }
                result.Add(taskResult);
            }

            return result;
        }
        private async Task<Project> LoadProjectWithTasks(Guid projectId)
        {
            return await _context.Projects
                                .Include(p => p.Board)
                                    .ThenInclude(b => b.Interations)
                                        .ThenInclude(i => i.Tasks)
                                .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        }

        private List<Entities.Task> GetTasksFromProject(Project project)
        {
            var tasks = new List<Entities.Task>();
            foreach (var interaction in project.Board.Interations)
            {
                tasks.AddRange(interaction.Tasks);
            }
            return tasks;
        }

        private ReportRecord GenerateReportRecord(List<BoardStatus> listStatus, List<Entities.Task> tasks)////////////////////
        {
            var reportRecord = new ReportRecord();
            var reports = GenerateReportStatusList(listStatus, tasks);

            reportRecord.TotalTask = tasks.Count();
            reportRecord.reportStatuses = reports;

            return reportRecord;
        }

        private List<ReportStatus> GenerateReportStatusList(List<BoardStatus> listStatus, List<Entities.Task> tasks)
        {
            var reports = new List<ReportStatus>();
            foreach (var status in listStatus)
            {
                var numberTask = tasks.Count(x => x.StatusId == status.BoardStatusId && x.IsDelete != true);
                reports.Add(new ReportStatus
                {
                    BoardStatusId = status.BoardStatusId,
                    Title = status.Title,
                    Order = status.Order,
                    NumberTask = numberTask,
                    Percent = tasks.Count() == 0 ? 0 : (int)Math.Round((double)(100 * numberTask) / tasks.Count)
                });
            }
            reports.Add(new ReportStatus
            {
                BoardStatusId = Guid.Parse("C59F200A-C557-4492-8D0A-5556A3BA7D31"),
                Title = "Deleted",
                NumberTask = tasks.Count(x => x.IsDelete == true),
                Percent = tasks.Count() == 0 ? 0 : (int)Math.Round((double)(100 * tasks.Count(x => x.IsDelete == true)) / tasks.Count)
            }) ;
            return reports;
        }

        private List<ReportRecord> GenerateReportByWeek(List<BoardStatus> listStatus, List<Entities.Task> tasks)
        {
            var reportByWeek = new List<ReportRecord>();
            var currentWeek = GetCurrentWeek();

            foreach (var day in currentWeek)
            {
                var taskByDay = tasks.Where(x => x.CreateTime.Date == day).ToList();
                var reportByWeeks = GenerateReportStatusList(listStatus, taskByDay);

                reportByWeek.Add(new ReportRecord
                {
                    TotalTask = taskByDay.Count(),
                    DateTime = day.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    reportStatuses = reportByWeeks
                });
            }
            return reportByWeek;
        }
        private List<MemberTasks> GenerateMemberReport(List<ProjectMember> projectMembers, List<BoardStatus> listStatus, List<Entities.Task> tasks)
        {
            var reportMemberTasks = new List<MemberTasks>();

            foreach (var member in projectMembers)
            {
                var memberTasks = tasks.Where(x => x.AssignTo == member.MemberId).ToList();
                var reportByWeeks = GenerateReportStatusList(listStatus, memberTasks);

                reportMemberTasks.Add(new MemberTasks
                {
                    MemberId= member.MemberId,
                    UserId= member.UserId,
                    Fullname = member.Users.Fullname,
                    Email= member.Users.Email,
                    RoleId= member.RoleId,
                    RoleName = _context.Roles.FirstOrDefault(r => r.RoleId == member.RoleId).RoleName,
                    IsOwner= member.IsOwner,
                    TotalTasks = memberTasks.Count(),
                    reportStatuses = reportByWeeks
                });
            }
            return reportMemberTasks;
        }

        private List<DateTime> GetCurrentWeek()
        {
            DateTime today = DateTime.Today;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);

            List<DateTime> datesOfWeek = Enumerable.Range(0, 7)
                                                    .Select(i => startOfWeek.AddDays(i))
                                                    .ToList();
            return datesOfWeek;
        }

        
    }

}
