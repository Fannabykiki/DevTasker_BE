using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.TaskType
{
    public class GetTaskTypeResponse
    {
        public Guid TypeId { get; set; }
        public string Title { get; set; }
    }
}
