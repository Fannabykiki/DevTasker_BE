﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.PermissionSchema
{
    public class GetAllPermissionSchemaResponse
    {
		public Guid SchemaId { get; set; }
		public string SchemaName { get; set; }
		public string Description { get; set; }
	}
}
