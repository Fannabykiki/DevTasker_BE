using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.Constants
{
    public static class CapstoneNameConstant
    {
        public static class TaskStatusNameConstant
        {
            public const string New = "New";
            public const string ToDo = "To do";
            public const string InProgress = "In Progress";
            public const string Done = "Done";
            public const string Deleted = "Deleted";

        }
    }
    public static class RoleNameConstant
    {
        public const string ProductOwner = "PO";
        public const string Supervisor = "Supervisor";

    }
    public static class CommentActionCconstant
    {
        public const string Create = "Create";
        public const string Edit = "Edit";
        public const string Delete = "Delete";
        public const string Reply = "Reply";
    }
    public static class ErrorMessage
    {
        public const string InvalidPermission = "Invalid permission!";
    }
    public static class PermissionNameConstant
    {
        public const string AdministerProjects = "Administer Projects";
        public const string BrowseProjects = "Browse Projects";
        public const string CreateTasks = "Create Tasks";
        public const string ResolveTasks = "Resolve Tasks";
        public const string ScheduleTasks = "Schedule Tasks";
        public const string AssignableTasks = "Assignable Tasks";
        public const string EditTasks = "Edit Tasks";
        public const string AssignTasks = "Assign tasks";
        public const string CloseTasks = "Close Tasks";
        public const string DeleteTasks = "Delete Tasks";
        public const string DeleteOwnComments = "Delete Own Comments";
        public const string DeleteAllComments = "Delete All Comments";
        public const string EditOwnComments = "Edit Own Comments";
        public const string EditAllComments = "Edit All Comments";
        public const string CreateAttachments = "Create Attachments";
        public const string DeleteAllAttachments = "Delete All Attachments";

    }
    public static class AuthorizationRequirementNameConstant
    {
        public const string RolePermission = "RolePermission";
    }
    public static class StatusGuidConstant
    {
        public const string StatusInTeamGuid = "BA888147-C90A-4578-8BA6-63BA1756FAC1";
        public const string StatusUnavailableGuid = "A29BF1E9-2DE2-4E5F-A6DA-32D88FCCD274";
    }
}
