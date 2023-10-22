using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Schema
{
    public class UpdateSchemaRequest
    {
        public string? SchemaName { get; set; }
        public string? Description { get; set; }
    }
}
