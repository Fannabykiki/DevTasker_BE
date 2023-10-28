using Capstone.Common.DTOs.Iteration;


namespace Capstone.Service.IterationService
{
    public interface IIterationService
    {
        
        Task<bool> CreateIteration(CreateIterationRequest createIterationRequest, Guid boardId);
        Task<bool> UpdateIterationRequest(UpdateIterationRequest updateIterationRequest, Guid iterationId);
    }
}
