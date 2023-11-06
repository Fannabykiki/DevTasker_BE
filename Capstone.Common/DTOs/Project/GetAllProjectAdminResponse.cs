using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Project
{
    public class GetAllProjectAdminResponse
    {
        public int TotalProject { get; set; }
        public int ActiveProject { get; set; }
        public int PercentActive { get; set; }
        public int CloseProject { get; set; }
        public int PercentClose { get; set; }
        public int OtherProject { get; set; }
        public int PercentOther { get; set; }
        public List<GetAllProjectResponse> getAllProjectViewModels { get; set; }
    }
}
