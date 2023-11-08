using Capstone.Common.DTOs.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.PermissionSchema
{
    public class GetAllPermissionSchemaResponse
    {
		public List<GetSchemaResponse> Schemas { get; set; }
		public Pagination Pagination { get; set; }
		
	}
}
