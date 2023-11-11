using Capstone.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Common.DTOs.User
{
    public class ChangeUserStatusRequest
    {
        public string? reason { get; set; }
        public bool StatusIdChangeTo { get; set; }
    }
}
