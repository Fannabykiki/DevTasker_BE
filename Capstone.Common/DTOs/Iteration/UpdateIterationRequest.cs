using Capstone.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Iteration
{
    public class UpdateIterationRequest
    {
		public Guid InterationId { get; set; }

		public string InterationName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid StatusId { get; set; }
    }
}
