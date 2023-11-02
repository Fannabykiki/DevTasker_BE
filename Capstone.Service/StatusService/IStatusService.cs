using Capstone.DataAccess.Entities;

namespace Capstone.Service.StatusService
{
    public interface IStatusService
    {
        Task<Status> GetStatusByIdAsync(Guid id);
       Task<IQueryable<Status>>  GetAllStatusAsync();
    }
}
