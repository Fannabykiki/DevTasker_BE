using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.Service.StatusService
{
    public class StatusService : IStatusService
    {
        private readonly IStatusRepository _statusRepository;

        public StatusService(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }
        public async Task<Status> GetStatusByIdAsync(Guid id)
        {
            return await _statusRepository.GetAsync(x => x.StatusId == id, null)!;
        }

        public Task<IQueryable<Status>> GetAllStatusAsync()
        {
            return Task.FromResult(_statusRepository.GetAllAsync(x=> true, null));
        }
    }
}
