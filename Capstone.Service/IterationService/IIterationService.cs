using Capstone.Common.DTOs.Iteration;


namespace Capstone.Service.IterationService
{
    public interface IIterationService
    {
        Task<IEnumerable<GetInterrationByBoardIdResonse>> GetIterationTasksByProjectId(Guid projectId);
        Task<IEnumerable<GetInterrationByBoardIdResonse>> GetIterationsById(Guid iterationId);
        Task<GetIntergrationResponse> CreateInteration(CreateIterationRequest createIterationRequest);
        Task<bool> UpdateIterationRequest(UpdateIterationRequest updateIterationRequest, Guid iterationId);
    }
}
