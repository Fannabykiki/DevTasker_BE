﻿using Capstone.Common.DTOs.Iteration;


namespace Capstone.Service.IterationService
{
    public interface IInterationService
    {
        Task<List<GetAllInterrationByProjectIdResonse>> GetIterationsByProjectId(Guid projectId);
        Task<bool> CreateIteration(CreateIterationRequest createIterationRequest, Guid ProjectId);
        Task<bool> UpdateIterationRequest(UpdateIterationRequest updateIterationRequest, Guid iterationId);
    }
}
