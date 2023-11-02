using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.Service.Priority
{
    public class PriorityService : IPriorityService
    {
        private readonly IPriorityRepository _priorityRepository;

        public PriorityService(IPriorityRepository priorityRepository)
        {
            _priorityRepository = priorityRepository;
        }
    
        public Task<IQueryable<PriorityLevel>> GetAllPriorityAsync()
        {
            var result = _priorityRepository.GetAllAsync(x => true, null);
            return Task.FromResult(result);
        }
    }
}
