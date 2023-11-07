using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Paging
{
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; }
        public Pagination Paginations { get; set; }
    }
}
