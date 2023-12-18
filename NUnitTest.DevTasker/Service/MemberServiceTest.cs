using AutoMapper;
using Capstone.Common.DTOs.Project;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.ProjectMemberService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;


namespace NUnitTest.DevTasker.Service
{
    [TestFixture]
    public class MemberServiceTest
    {
        private readonly CapstoneContext _context;
        private ProjectMemberService _projectMemberService;
        private Mock <IProjectMemberRepository> _projectMemberRepository;
        private Mock <IProjectRepository> _projectRepository;
        private Mock <IUserRepository> _userRepository;
        private Mock <IMapper> _mapper;
        private Mock<IDatabaseTransaction> _transactionMock;
        private Mock<IServiceScopeFactory> _serviceScopeFactory;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<IInvitationRepository> _invitationRepository;


        [SetUp]
        public void Setup()
        {
            
            _projectMemberRepository = new Mock<IProjectMemberRepository>();
            _projectRepository = new Mock<IProjectRepository>();
            _userRepository = new Mock<IUserRepository>();
            _mapper = new Mock<IMapper>(); 
            _transactionMock = new Mock<IDatabaseTransaction>();  
            _serviceScopeFactory = new Mock<IServiceScopeFactory>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _invitationRepository = new Mock<IInvitationRepository>();

           
            _projectMemberRepository.Setup(repo => repo.DatabaseTransaction()).Returns(_transactionMock.Object);

            _projectMemberService = new ProjectMemberService(
            _projectRepository.Object,
            _serviceScopeFactory.Object,
            _httpContextAccessor.Object,
            _mapper.Object,
            _projectMemberRepository.Object,
            _userRepository.Object,
            _invitationRepository.Object
            );
        }

        [Test]
        public async Task AcceptInvitation_SuccessfulAcceptance_ReturnsSuccessResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var acceptInviteRequest = new AcceptInviteRequest
            {
                ProjectId = Guid.NewGuid(),
                InvitationId = Guid.NewGuid(),
            };
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(new ProjectMember { UserId = userId, ProjectId = acceptInviteRequest.ProjectId });
            _invitationRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Invitation, bool>>>(), null))
                .ReturnsAsync(new Invitation { InvitationId = acceptInviteRequest.InvitationId });

            // Act
            var result = await _projectMemberService.AcceptInvitation(userId, acceptInviteRequest);

            // Assert
            Assert.IsTrue(result.IsSucceed);
            Assert.AreEqual("Accept invitation successfully", result.Message);
        }

        [Test]
        public async Task AcceptInvitation_InvalidUser_ReturnsErrorResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var acceptInviteRequest = new AcceptInviteRequest
            {
                ProjectId = Guid.NewGuid(),
                InvitationId = Guid.NewGuid(),
            };
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync((ProjectMember)null); // Simulate user not found

            // Act
            var result = await _projectMemberService.AcceptInvitation(userId, acceptInviteRequest);

            // Assert
            Assert.IsFalse(result.IsSucceed);
            Assert.AreEqual("Accept invitation fail", result.Message);
        }

        [Test]
        public async Task AcceptInvitation_InvalidInvitation_ReturnsErrorResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var acceptInviteRequest = new AcceptInviteRequest
            {
                ProjectId = Guid.NewGuid(),
                InvitationId = Guid.NewGuid(),
            };
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(new ProjectMember { UserId = userId, ProjectId = acceptInviteRequest.ProjectId });
            _invitationRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Invitation, bool>>>(), null))
                .ReturnsAsync((Invitation)null); // Simulate invitation not found

            // Act
            var result = await _projectMemberService.AcceptInvitation(userId, acceptInviteRequest);

            // Assert
            Assert.IsFalse(result.IsSucceed);
            Assert.AreEqual("Accept invitation fail", result.Message);
        }

        [Test]
        public async Task AcceptInvitation_UpdateProjectMemberFails_ReturnsErrorResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var acceptInviteRequest = new AcceptInviteRequest
            {
                ProjectId = Guid.NewGuid(),
                InvitationId = Guid.NewGuid(),
            };
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(new ProjectMember { UserId = userId, ProjectId = acceptInviteRequest.ProjectId });
            _invitationRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Invitation, bool>>>(), null))
                .ReturnsAsync(new Invitation { InvitationId = acceptInviteRequest.InvitationId });
            _projectMemberRepository.Setup(repo => repo.UpdateAsync(It.IsAny<ProjectMember>()))
                .ThrowsAsync(new Exception("Simulated update failure"));

            // Act
            var result = await _projectMemberService.AcceptInvitation(userId, acceptInviteRequest);

            // Assert
            Assert.IsFalse(result.IsSucceed);
            Assert.AreEqual("Accept invitation fail", result.Message);
           
        }

        [Test]
        public async Task AcceptInvitation_UpdateInvitationFails_ReturnsErrorResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var acceptInviteRequest = new AcceptInviteRequest
            {
                ProjectId = Guid.NewGuid(),
                InvitationId = Guid.NewGuid(),
            };
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(new ProjectMember { UserId = userId, ProjectId = acceptInviteRequest.ProjectId });
            _invitationRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Invitation, bool>>>(), null))
                .ReturnsAsync(new Invitation { InvitationId = acceptInviteRequest.InvitationId });
            _projectMemberRepository.Setup(repo => repo.UpdateAsync(It.IsAny<ProjectMember>()));
            _invitationRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Invitation>()))
                .ThrowsAsync(new Exception("Simulated update failure"));

            // Act
            var result = await _projectMemberService.AcceptInvitation(userId, acceptInviteRequest);

            // Assert
            Assert.IsFalse(result.IsSucceed);
            Assert.AreEqual("Accept invitation fail", result.Message);
           
        }

        [Test]
        public async Task AcceptInvitation_TransactionRollbackOnFailure_ReturnsErrorResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var acceptInviteRequest = new AcceptInviteRequest
            {
                ProjectId = Guid.NewGuid(),
                InvitationId = Guid.NewGuid(),
            };
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(new ProjectMember { UserId = userId, ProjectId = acceptInviteRequest.ProjectId });
            _invitationRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Invitation, bool>>>(), null))
                .ReturnsAsync(new Invitation { InvitationId = acceptInviteRequest.InvitationId });
            _projectMemberRepository.Setup(repo => repo.UpdateAsync(It.IsAny<ProjectMember>()))
                .ThrowsAsync(new Exception("Simulated update failure"));
            _transactionMock.Setup(t => t.RollBack());

            // Act
            var result = await _projectMemberService.AcceptInvitation(userId, acceptInviteRequest);

            // Assert
            Assert.IsFalse(result.IsSucceed);
            Assert.AreEqual("Accept invitation fail", result.Message);
           
        }

        [Test]
        public async Task AcceptInvitation_SuccessfulAcceptance_CommitsTransaction()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var acceptInviteRequest = new AcceptInviteRequest
            {
                ProjectId = Guid.NewGuid(),
                InvitationId = Guid.NewGuid(),
            };
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(new ProjectMember { UserId = userId, ProjectId = acceptInviteRequest.ProjectId });
            _invitationRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Invitation, bool>>>(), null))
                .ReturnsAsync(new Invitation { InvitationId = acceptInviteRequest.InvitationId });

            // Act
            await _projectMemberService.AcceptInvitation(userId, acceptInviteRequest);

            // Assert
            _transactionMock.Verify(t => t.Commit(), Times.Once);
        }

        [Test]
        public async Task AcceptInvitation_ExceptionDuringCommit_RollsBackTransaction()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var acceptInviteRequest = new AcceptInviteRequest
            {
                ProjectId = Guid.NewGuid(),
                InvitationId = Guid.NewGuid(),
            };
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(new ProjectMember { UserId = userId, ProjectId = acceptInviteRequest.ProjectId });
            _invitationRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Invitation, bool>>>(), null))
                .ReturnsAsync(new Invitation { InvitationId = acceptInviteRequest.InvitationId });
            _transactionMock.Setup(t => t.Commit()).Throws(new Exception("Simulated commit failure"));

            // Act
            var result = await _projectMemberService.AcceptInvitation(userId, acceptInviteRequest);

            // Assert
            Assert.IsFalse(result.IsSucceed);
            Assert.AreEqual("Accept invitation fail", result.Message);
            _transactionMock.Verify(t => t.RollBack(), Times.Once);
        }

        [Test]
        public async Task AddNewProjectMember_SuccessfulInvitation_ReturnsSuccessResponse()
        {
            // Arrange
            var inviteUserRequest = new InviteUserRequest
            {
                ProjectId = Guid.NewGuid(),
                Email = new List<string> { "user@example.com" }
            };
            var user = new User { UserId = Guid.NewGuid(), Email = "user@example.com" };
            _userRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(user);
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync((ProjectMember)null); // Simulate member not found

            // Act
            var result = await _projectMemberService.AddNewProjectMember(inviteUserRequest);

            // Assert
            Assert.IsTrue(result.IsSucceed);
            Assert.AreEqual("Invite user to project successfully", result.Message);
            _projectMemberRepository.Verify(repo => repo.CreateAsync(It.IsAny<ProjectMember>()), Times.Once);
            _projectMemberRepository.Verify(repo => repo.SaveChanges(), Times.Once);
            _transactionMock.Verify(t => t.Commit(), Times.Once);
        }

        [Test]
        public async Task AddNewProjectMember_UserIsAlreadyMember_ReturnsSuccessResponse()
        {
            // Arrange
            var inviteUserRequest = new InviteUserRequest
            {
                ProjectId = Guid.NewGuid(),
                Email = new List<string> { "existinguser@example.com" }
            };
            var user = new User { UserId = Guid.NewGuid(), Email = "existinguser@example.com" };
            var existingMember = new ProjectMember { UserId = user.UserId, ProjectId = inviteUserRequest.ProjectId };
            _userRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), null))
                .ReturnsAsync(user);
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(existingMember);

            // Act
            var result = await _projectMemberService.AddNewProjectMember(inviteUserRequest);

            // Assert
            Assert.IsTrue(result.IsSucceed);
            Assert.AreEqual("Invite user to project successfully", result.Message);
            _projectMemberRepository.Verify(repo => repo.UpdateAsync(existingMember), Times.Once);
            _projectMemberRepository.Verify(repo => repo.SaveChanges(), Times.Once);
            _transactionMock.Verify(t => t.Commit(), Times.Once);
        }
       

        [Test]
        public async Task DeclineInvitation_UserNotMember_ReturnsErrorResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var acceptInviteRequest = new AcceptInviteRequest
            {
                ProjectId = Guid.NewGuid(),
                InvitationId = Guid.NewGuid(),
            };
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync((ProjectMember)null); // Simulate member not found

            // Act
            var result = await _projectMemberService.DeclineInvitation(userId, acceptInviteRequest);

            // Assert
            Assert.IsFalse(result.IsSucceed);
            Assert.AreEqual("You decline invitation fail", result.Message);
            _transactionMock.Verify(t => t.RollBack(), Times.Once);
        }

        [Test]
        public async Task DeclineInvitation_InvitationNotFound_ReturnsErrorResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var acceptInviteRequest = new AcceptInviteRequest
            {
                ProjectId = Guid.NewGuid(),
                InvitationId = Guid.NewGuid(),
            };
            var projectMember = new ProjectMember { UserId = userId, ProjectId = acceptInviteRequest.ProjectId };
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(projectMember);
            _invitationRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Invitation, bool>>>(), null))
                .ReturnsAsync((Invitation)null); // Simulate invitation not found

            // Act
            var result = await _projectMemberService.DeclineInvitation(userId, acceptInviteRequest);

            // Assert
            Assert.IsFalse(result.IsSucceed);
            Assert.AreEqual("You decline invitation fail", result.Message);
            _transactionMock.Verify(t => t.RollBack(), Times.Once);
        }

        [Test]
        public async Task DeclineInvitation_UpdateMemberFails_RollsBackTransaction()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var acceptInviteRequest = new AcceptInviteRequest
            {
                ProjectId = Guid.NewGuid(),
                InvitationId = Guid.NewGuid(),
            };
            var projectMember = new ProjectMember { UserId = userId, ProjectId = acceptInviteRequest.ProjectId };
            var invitation = new Invitation { InvitationId = acceptInviteRequest.InvitationId };
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(projectMember);
            _invitationRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Invitation, bool>>>(), null))
                .ReturnsAsync(invitation);
            _projectMemberRepository.Setup(repo => repo.UpdateAsync(It.IsAny<ProjectMember>()))
                .ThrowsAsync(new Exception("Simulated update failure"));

            // Act
            var result = await _projectMemberService.DeclineInvitation(userId, acceptInviteRequest);

            // Assert
            Assert.IsFalse(result.IsSucceed);
            Assert.AreEqual("You decline invitation fail", result.Message);
            _transactionMock.Verify(t => t.RollBack(), Times.Once);
        }

        [Test]
        public async Task DeclineInvitation_SaveChangesFails_RollsBackTransaction()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var acceptInviteRequest = new AcceptInviteRequest
            {
                ProjectId = Guid.NewGuid(),
                InvitationId = Guid.NewGuid(),
            };
            var projectMember = new ProjectMember { UserId = userId, ProjectId = acceptInviteRequest.ProjectId };
            var invitation = new Invitation { InvitationId = acceptInviteRequest.InvitationId };
            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(projectMember);
            _invitationRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Invitation, bool>>>(), null))
                .ReturnsAsync(invitation);
            _projectMemberRepository.Setup(repo => repo.SaveChanges())
                .ThrowsAsync(new Exception("Simulated save changes failure"));

            // Act
            var result = await _projectMemberService.DeclineInvitation(userId, acceptInviteRequest);

            // Assert
            Assert.IsFalse(result.IsSucceed);
            Assert.AreEqual("You decline invitation fail", result.Message);
            _transactionMock.Verify(t => t.RollBack(), Times.Once);
        }

       
    }
}
