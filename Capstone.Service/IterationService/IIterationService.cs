using Capstone.Common.DTOs.Iteration;


namespace Capstone.Service.IterationService
{
    public interface IIterationService
    {
        //Task<IEnumerable<GetAllIterationResponse>> GetAllIteration();
        Task<List<GetAllInterrationByProjectIdResonse>> GetIterationsByProjectId(Guid boardId);
        Task<bool> CreateIteration(CreateIterationRequest createIterationRequest, Guid boardId);
        Task<bool> UpdateIterationRequest(UpdateIterationRequest updateIterationRequest, Guid iterationId);
    }
}
