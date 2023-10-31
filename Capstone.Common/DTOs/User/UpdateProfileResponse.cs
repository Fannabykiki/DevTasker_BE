using Capstone.Common.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.User
{
	public class UpdateProfileResponse : BaseResponse
	{
        public string? VerifyToken { get; set; }
    }
}
