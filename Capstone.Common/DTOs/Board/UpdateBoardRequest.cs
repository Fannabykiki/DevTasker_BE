using Capstone.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Board
{
    public class UpdateBoardRequest
    {
        public string Title { get; set; }
        public Guid? InterationId { get; set; }
        public StatusEnum? Status { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}

