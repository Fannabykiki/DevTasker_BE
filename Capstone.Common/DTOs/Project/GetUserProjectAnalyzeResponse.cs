using Capstone.Common.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Project
{
    public class GetUserProjectAnalyzeResponse
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string ProjectStatus { get; set; }
        public UserResponse Manager { get; set; }
        public DateTime StartDate { get; set; }

    }
}
