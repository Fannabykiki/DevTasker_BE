using Capstone.Common.DTOs.Iteration;


namespace Capstone.Service.IterationService
{
    public interface IIterationService
    {
        Task<IEnumerable<GetInterrationByBoardIdResonse>> GetIterationsByBoardId(Guid boardId);
        Task<IEnumerable<GetInterrationByIdResonse>> GetIterationsById(Guid iterationId);
        Task<bool> CreateIteration(CreateIterationRequest createIterationRequest, Guid boardId);
        Task<bool> UpdateIterationRequest(UpdateIterationRequest updateIterationRequest, Guid iterationId);
    }
}
