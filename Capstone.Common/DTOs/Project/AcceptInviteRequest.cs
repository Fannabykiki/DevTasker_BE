using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.Project
{
	public class AcceptInviteRequest
	{
        public Guid ProjectId { get; set; }
        public Guid InvitationId { get; set; }
        public string Email { get; set; }
    }
}
