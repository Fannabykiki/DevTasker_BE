using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Board
{
    public class DeleteBoardRequest
    {
        public Guid BoardId { get; set; }
        public DateTime? DeleteAt { get; set; }
        public Guid? InterationId { get; set; }
    }
}
