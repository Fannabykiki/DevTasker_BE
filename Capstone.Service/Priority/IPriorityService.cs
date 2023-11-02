using Capstone.DataAccess.Entities;

namespace Capstone.Service.Priority;

public interface IPriorityService
{
    Task<IQueryable<PriorityLevel>> GetAllPriorityAsync();

}