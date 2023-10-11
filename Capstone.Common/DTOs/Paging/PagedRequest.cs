using Capstone.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Paging
{
    public class PagedRequest
    {
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
        public StatusEnum? status { get; set; }
        public string? search { get; set; }
    }
}
