using AutoMapper;
using Capstone.Common.DTOs.ProjectMember;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Capstone.Common.DTOs.User;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Entities;
using Capstone.Common.DTOs.Project;

namespace Capstone.Service.ProjectMemberService
{
	public class ProjectMemberService : IProjectMemberService
	{
		private readonly CapstoneContext _context;
		private readonly IProjectMemberRepository _projectMemberRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ProjectMemberService(IProjectRepository projectRepository, IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor, IMapper mapper, IProjectMemberRepository projectMemberRepository, IUserRepository userRepository)
		{
			_projectRepository = projectRepository;
			_serviceScopeFactory = serviceScopeFactory;
			_httpContextAccessor = httpContextAccessor;
			_mapper = mapper;
			_projectMemberRepository = projectMemberRepository;
			_userRepository = userRepository;
		}

		public async Task<AddNewProjectMemberResponse> AddNewProjectMember(InviteUserRequest inviteUserRequest)
		{
			using (var transaction = _projectMemberRepository.DatabaseTransaction())
			{
				try
				{
					foreach (var email in inviteUserRequest.Email)
					{

						var user = await _userRepository.GetAsync(x => x.Email == email, null);
						if (user == null)
						{
							return new AddNewProjectMemberResponse
							{
								IsSucced = false,
								Message = "User not exist in system"
							};
						}
						var projectMember = new ProjectMember
						{
							IsOwner = false,
							MemberId = Guid.NewGuid(),
							ProjectId = inviteUserRequest.ProjectId,
							UserId = user.UserId,
							StatusId = Guid.Parse("2d79988f-49c8-4bf4-b5ab-623559b30746"),
							RoleId = Guid.Parse("0A0994FC-CBAE-482F-B5E8-160BB8DDCD56")
						};

						var member = await _projectMemberRepository.CreateAsync(projectMember);
						await _projectMemberRepository.SaveChanges();
					}
					transaction.Commit();
					return new AddNewProjectMemberResponse
					{
						IsSucced = true
					};
				}
				catch (Exception ex)
				{
					transaction.RollBack();
					throw ex;
				}
			}
		}
	}
}
