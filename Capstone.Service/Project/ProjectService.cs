using System.Security.Cryptography;
using AutoMapper;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.User;
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

    public ProjectService(CapstoneContext context, IProjectRepository projectRepository, IMapper mapper)
    {
        _context = context;
        _projectRepository = projectRepository;
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

    public async Task<IEnumerable<GetAllProjectViewModel>> GetAllProjects()
    {
        var projects = await _projectRepository.GetAllWithOdata(x => true, null);
        return _mapper.Map<List<GetAllProjectViewModel>>(projects); 
    }
}