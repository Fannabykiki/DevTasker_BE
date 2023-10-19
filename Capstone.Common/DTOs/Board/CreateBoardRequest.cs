using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Board
{
    public class CreateBoardRequest
    {
        public string Title { get; set; }
        public DateTime CreateAt { get; set; }
        public Guid? InterationId { get; set; }
    }
}
