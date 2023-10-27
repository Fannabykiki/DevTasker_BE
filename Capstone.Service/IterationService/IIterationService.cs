using Capstone.Common.DTOs.Iteration;


namespace Capstone.Service.IterationService
{
    public interface IIterationService
    {
        //Task<IEnumerable<GetAllIterationResponse>> GetAllIteration();
        Task<IEnumerable<GetInterrationByBoardIdResonse>> GetIterationsByBoardId(Guid boardId);
        Task<bool> CreateIteration(CreateIterationRequest createIterationRequest, Guid ProjectId);
        Task<bool> UpdateIterationRequest(UpdateIterationRequest updateIterationRequest, Guid iterationId);
    }
}
