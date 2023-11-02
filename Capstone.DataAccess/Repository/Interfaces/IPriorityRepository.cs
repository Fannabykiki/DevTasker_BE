﻿using Capstone.Common.DTOs.Project;
using Capstone.DataAccess.Entities;
using static System.Reflection.Metadata.BlobBuilder;

namespace Capstone.DataAccess.Repository.Interfaces
{
    public interface IPriorityRepository : IBaseRepository<PriorityLevel>
    {
      //  Task<List<ViewProjectInfoRequest>> GetInfoProjectByProjectId(Guid projectId);
    }
}
