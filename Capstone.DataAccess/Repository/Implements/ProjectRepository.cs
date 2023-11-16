using Capstone.Common.DTOs.Permission;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Role;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            var listMember = _context.ProjectMembers.Include(u => u.Users).ThenInclude(s => s.Status).Where(x => x.ProjectId == projectId).ToList();
            var reportRecord = GenerateReportRecord(listStatus,tasks);
            var reportByWeek = GenerateReportByWeek(listStatus,tasks);
            var reportMembers = GenerateMemberReport(listMember, listStatus, tasks);

            var result = new GetProjectReportRequest
            {
                reportProject = reportRecord,
                reportRecordByWeerk = reportByWeek,
                memberTaks = reportMembers
            };

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

        private ReportRecord GenerateReportRecord(List<BoardStatus> listStatus, List<Entities.Task> tasks)
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
                var numberTask = tasks.Count(x => x.StatusId == status.BoardStatusId);
                reports.Add(new ReportStatus
                {
                    BoardStatusId = status.BoardStatusId,
                    Title = status.Title,
                    NumberTask = numberTask,
                    Percent = tasks.Count() == 0 ? 0 : (int)Math.Round((double)(100 * numberTask) / tasks.Count)
                });
            }
            return reports;
        }

        private List<ReportRecord> GenerateReportByWeek(List<BoardStatus> listStatus, List<Entities.Task> tasks)
        {
            var reportByWeek = new List<ReportRecord>();
            var currentWeek = GetCurrentWeek();

            foreach (var day in currentWeek)
            {
                var taskByDay = tasks.Where(x => x.CreateTime <= day && x.DueDate >= day).ToList();
                var reportByWeeks = GenerateReportStatusList(listStatus, taskByDay);

                reportByWeek.Add(new ReportRecord
                {
                    TotalTask = taskByDay.Count(),
                    DateTime = day,
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
