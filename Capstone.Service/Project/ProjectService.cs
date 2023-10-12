using AutoMapper;
using Capstone.Common.DTOs.Project;
using Capstone.Common.Enums;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.Service.Project;

public class ProjectService : IProjectService
{
    private readonly CapstoneContext _context;
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly IRoleRepository _roleRepository;

	public ProjectService(CapstoneContext context, IProjectRepository projectRepository, IRoleRepository roleRepository, IMapper mapper)
	{
		_context = context;
		_projectRepository = projectRepository;
		_roleRepository = roleRepository;
		_mapper = mapper;
	}

	public async Task<bool> CreateProject(CreateProjectRequest createProjectRequest)
    {
        using var transaction = _projectRepository.DatabaseTransaction();
        try
        {
            var newProjectRequest = new DataAccess.Entities.Project
            {
                ProjectId = Guid.NewGuid(),
                ProjectName = createProjectRequest.ProjectName,
                CreateAt = createProjectRequest.CreateAt,
                EndDate = createProjectRequest.EndDate,
                StartDate = createProjectRequest.StartDate,
                PrivacyStatus = true,
                ProjectStatus = StatusEnum.Active,
                CreateBy = createProjectRequest.CreateBy
            };

            await _projectRepository.CreateAsync(newProjectRequest);
            _projectRepository.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<IEnumerable<GetAllProjectViewModel>> GetAllProjects(Guid UserId)
    {
        var projects = await _projectRepository.GetAllWithOdata(x => true, x => x.ProjectMembers.Where(x=> x.UserId == UserId));
        return _mapper.Map<List<GetAllProjectViewModel>>(projects);
    }

	public async Task<bool> CreateProjectRole(CreateRoleRequest createRoleRequest)
	{
		using var transaction = _projectRepository.DatabaseTransaction();
		try
		{
			var newRoleRequest = new Role
			{
				RoleId = Guid.NewGuid(),
                RoleName = createRoleRequest.RoleName,
                Description = createRoleRequest.Descroption
			};

			await _roleRepository.CreateAsync(newRoleRequest);

			_projectRepository.SaveChanges();

			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
}