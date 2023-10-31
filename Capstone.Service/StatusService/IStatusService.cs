using Capstone.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Service.StatusService
{
    public interface IStatusService
    {
        Task<Status> GetStatusByIdAsync(Guid id);
    }
}
