﻿using Capstone.Common.DTOs.Project;

namespace Capstone.Service.Project;

public interface IProjectService
{
    Task<bool> CreateProject(CreateProjectRequest createProjectRequest);
    Task<bool> CreateProjectRole(CreateRoleRequest createRoleRequest);
    Task<IEnumerable<GetAllProjectViewModel>> GetAllProjects(Guid UserId);

}