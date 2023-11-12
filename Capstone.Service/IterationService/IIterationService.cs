using Capstone.Common.DTOs.Iteration;


namespace Capstone.Service.IterationService
{
    public interface IIterationService
    {
        Task<IEnumerable<GetInterrationByBoardIdResonse>> GetIterationTasksByProjectId(Guid projectId);
        Task<IEnumerable<GetInterrationByBoardIdResonse>> GetIterationsById(Guid iterationId);
        Task<bool> CreateInteration(CreateIterationRequest createIterationRequest, Guid boardId);
        Task<bool> UpdateIterationRequest(UpdateIterationRequest updateIterationRequest, Guid iterationId);
    }
}
