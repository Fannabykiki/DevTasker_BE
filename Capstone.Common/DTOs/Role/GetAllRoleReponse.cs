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
    }
    public class GetRoleRecord
    {
        public GetRoleResponse Role { get; set; }
        public List<GetProjectUseRoleResponse> ProjectsUsed { get; set; }

    }
}
