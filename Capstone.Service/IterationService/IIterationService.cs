using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Iteration;


namespace Capstone.Service.IterationService
{
    public interface IIterationService
    {
        Task<IEnumerable<GetInterrationByIdResonse>> GetIterationTasksByProjectId(Guid projectId);
        Task<GetInterrationByIdResonse> GetIterationsById(Guid iterationId);
        Task<GetIntergrationResponse> CreateInteration(CreateIterationRequest createIterationRequest);
        Task<BaseResponse> UpdateIterationRequest(UpdateIterationRequest updateIterationRequest, Guid iterationId);
		Task<GetIntergrationResponse> GetCurrentInterationId(Guid projectId);
	}
}
