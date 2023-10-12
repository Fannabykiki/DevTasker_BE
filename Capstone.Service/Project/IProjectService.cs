using Capstone.Common.DTOs.Project;

namespace Capstone.Service.Project;

public interface IProjectService
{
    Task<bool> CreateProject(CreateProjectRequest createProjectRequest);
    Task<IEnumerable<GetAllProjectViewModel>> GetAllProjects();
}