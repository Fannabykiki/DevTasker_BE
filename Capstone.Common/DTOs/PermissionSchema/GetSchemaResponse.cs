using Capstone.Common.DTOs.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.PermissionSchema
{
    public class GetSchemaResponse
    {
        public Guid SchemaId { get; set; }
        public string SchemaName { get; set; }
        public string Description { get; set; }
        public List<GetProjectUsedResponse>? ProjectsUsed { get; set; }
    }
}
