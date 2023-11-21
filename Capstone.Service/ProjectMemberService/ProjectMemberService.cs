using AutoMapper;
using Capstone.Common.DTOs.ProjectMember;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Capstone.DataAccess.Entities;
using Capstone.Common.DTOs.Project;
using Capstone.Common.DTOs.Base;

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
		private readonly IInvitationRepository _invitationRepository;

		public ProjectMemberService(IProjectRepository projectRepository, IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor, IMapper mapper, IProjectMemberRepository projectMemberRepository, IUserRepository userRepository, IInvitationRepository invitationRepository)
		{
			_projectRepository = projectRepository;
			_serviceScopeFactory = serviceScopeFactory;
			_httpContextAccessor = httpContextAccessor;
			_mapper = mapper;
			_projectMemberRepository = projectMemberRepository;
			_userRepository = userRepository;
			_invitationRepository = invitationRepository;
		}

		public async Task<BaseResponse> AcceptInvitation(Guid userId, AcceptInviteRequest acceptInviteRequest)
		{
			using (var transaction = _projectMemberRepository.DatabaseTransaction())
			{
				try
				{
					var projectMember = await _projectMemberRepository.GetAsync(x => x.UserId == userId && x.ProjectId == acceptInviteRequest.ProjectId, null);
					projectMember.StatusId = Guid.Parse("BA888147-C90A-4578-8BA6-63BA1756FAC1");

					await _projectMemberRepository.UpdateAsync(projectMember);
					await _projectMemberRepository.SaveChanges();

					var invitation = await _invitationRepository.GetAsync(x => x.InvitationId == acceptInviteRequest.InvitationId, null);
					invitation.StatusId = Guid.Parse("ea91f463-44e8-4209-bad9-eb5b96732844");

					await _invitationRepository.UpdateAsync(invitation);
					await _invitationRepository.SaveChanges();
					transaction.Commit();

					return new BaseResponse
					{
						IsSucceed = true,
						Message = "Accept invitation successfully",
					};
				}
				catch (Exception ex)
				{
					transaction.RollBack();
					return new BaseResponse
					{
						IsSucceed = false,
						Message = "Accept invitation fail",
					};
				}
			}
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
						transaction.Commit();

					}
					return new AddNewProjectMemberResponse
					{
						IsSucceed = true
					};
				}
				catch (Exception ex)
				{
					transaction.RollBack();
					throw ex;
				}
			}
		}

		public async Task<bool> CheckMemberExist(string email, Guid projectId)
		{
			var statusId = Guid.Parse("BA888147-C90A-4578-8BA6-63BA1756FAC1");
			var projects = await _projectMemberRepository.CheckStatus(projectId, statusId);
			var list = _mapper.Map<List<ViewMemberProject>>(projects);
			foreach (var member in list)
			{
				if (email.Equals(member.Email))
				{
					return false;
				}
			}
			return true;
		}

		public async Task<bool> CheckMemberStatus(string email, Guid projectId)
		{
			var statusId = Guid.Parse("A29BF1E9-2DE2-4E5F-A6DA-32D88FCCD274");
			var projects = await _projectMemberRepository.CheckStatus(projectId, statusId);
			var list = _mapper.Map<List<ViewMemberProject>>(projects);
			foreach (var member in list)
			{
				if (email.Equals(member.Email))
				{
					return false;
				}
			}
			return true;
		}

		public async Task<bool> CheckSendMail(string email, Guid projectId)
		{
			var statusId = Guid.Parse("2D79988F-49C8-4BF4-B5AB-623559B30746");
			var projects = await _projectMemberRepository.CheckStatus(projectId, statusId);
			var list = _mapper.Map<List<ViewMemberProject>>(projects);
			foreach (var member in list)
			{
				if (email.Equals(member.Email))
				{
					return false;
				}
			}
			return true;
		}

		public async Task<BaseResponse> DeclineInvitation(Guid userId, AcceptInviteRequest acceptInviteRequest)
		{
			using (var transaction = _projectMemberRepository.DatabaseTransaction())
			{
				try
				{
					var projectMember = await _projectMemberRepository.GetAsync(x => x.UserId == userId && x.ProjectId == acceptInviteRequest.ProjectId, null);
					projectMember.StatusId = Guid.Parse("4ba5ff61-5397-4526-a4d6-5d220081689b");

					await _projectMemberRepository.UpdateAsync(projectMember);
					await _projectMemberRepository.SaveChanges();

					var invitation = await _invitationRepository.GetAsync(x => x.InvitationId == acceptInviteRequest.InvitationId, null);
					invitation.StatusId = Guid.Parse("4BA5FF61-5397-4526-A4D6-5D220081689B");

					await _invitationRepository.UpdateAsync(invitation);
					await _invitationRepository.SaveChanges();

					transaction.Commit();

					return new BaseResponse
					{
						IsSucceed = true,
						Message = "You decline invitation successfully",
					};
				}
				catch (Exception ex)
				{
					transaction.RollBack();
					return new BaseResponse
					{
						IsSucceed = false,
						Message = "You decline invitation fail",
					};
				}
			}
		}
	}
}
