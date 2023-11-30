using Capstone.Common.DTOs.Paging;
using Capstone.Common.DTOs.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Role
{
    public class GetAllRoleReponse
    {
        public List<GetRoleRecord> roleRecords { get; set; }
        public Pagination pagination { get; set; }
    }
    public class GetRoleRecord
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string? Description { get; set; }
        public List<GetProjectUsedResponse>? ProjectsUsed { get; set; }

    }
}
