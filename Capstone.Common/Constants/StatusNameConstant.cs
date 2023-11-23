using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.Constants
{
    public static class StatusNameConstant
    {
        public static class Task
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
    }
}
