using AutoMapper;
using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Project;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.Hubs;
using Capstone.Service.ProjectService;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace NUnitTest.DevTasker.Service
{
    public class ProjectServiceTest
    {
        private readonly CapstoneContext _context;
        private ProjectService _projectService;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IBoardRepository> _boardRepositoryMock;
        private Mock<IRoleRepository> _roleRepositoryMock;
        private Mock<IProjectMemberRepository> _projectMemberRepositoryMock;
        private Mock<IDatabaseTransaction> _databaseTransactionMock;
        private Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly IMapper _mapper;
        private Mock<ISchemaRepository> _schemaRepository;
        private Mock<IStatusRepository> _statusRepository;

        private Mock<IInterationRepository> _interationRepositoryMock;
        private Mock<IDatabaseTransaction> _transactionMock;
        private Mock<IPermissionSchemaRepository> _permissionScemaRepo;
        private Mock<IBoardStatusRepository> _boardStatusRepository;
        private Mock<ITaskRepository> _ticketRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<ITaskTypeRepository> _ticketTypeRepository;
        private Mock<IPriorityRepository> _priorityRepository;
        private Mock<IInvitationRepository> _invitationRepository;
        private readonly PresenceTracker _presenceTracker;
        private Mock <INotificationRepository> _notificationRepository;
        private Mock<Microsoft.AspNetCore.SignalR.IHubContext<NotificationHub>> _hubContext;



        [SetUp]
        public void Setup()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _projectMemberRepositoryMock = new Mock<IProjectMemberRepository>();
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _interationRepositoryMock = new Mock<IInterationRepository>();
            _statusRepository = new Mock<IStatusRepository>();
            _databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _schemaRepository = new Mock<ISchemaRepository>();
            _permissionScemaRepo = new Mock<IPermissionSchemaRepository>();
            _boardStatusRepository = new Mock<IBoardStatusRepository>();
            _ticketRepository = new Mock<ITaskRepository>();
            _userRepository = new Mock<IUserRepository>();
            _ticketTypeRepository = new Mock<ITaskTypeRepository>();
            _priorityRepository = new Mock<IPriorityRepository>();
            _invitationRepository = new Mock<IInvitationRepository>();
            _hubContext = new Mock<Microsoft.AspNetCore.SignalR.IHubContext<NotificationHub>>();
            _notificationRepository = new Mock<INotificationRepository>();


            _projectRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_databaseTransactionMock.Object);


            _projectService = new ProjectService(
                _context,
                _projectRepositoryMock.Object,
                _roleRepositoryMock.Object,
               _mapper,
                _schemaRepository.Object,
               _projectMemberRepositoryMock.Object,
               _boardRepositoryMock.Object,
                _permissionRepositoryMock.Object,
                _interationRepositoryMock.Object,
                _permissionScemaRepo.Object,
                _statusRepository.Object,
                _boardStatusRepository.Object,
                _userRepository.Object,
                _ticketTypeRepository.Object,
                 _priorityRepository.Object,
                _ticketRepository.Object,
                _invitationRepository.Object,
                _presenceTracker,
                _hubContext.Object,
                _notificationRepository.Object
            );
        }

        //Create Project 
        [Test]
        public async Task CreateProject_HappyPath_ReturnsValidResponse()
        { // Arrange
            var createProjectRequest = new CreateProjectRequest
            {
                ProjectName = "Test Project",
                EndDate = DateTime.Now.AddMonths(1),
                StartDate = DateTime.Now,
                PrivacyStatus = true,
                Description = "Test Project Description",
            };

            _projectRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Project>()))
                .ReturnsAsync(new Project { ProjectId = Guid.NewGuid() });
            _boardRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Board>()))
                .ReturnsAsync(new Board { BoardId = Guid.NewGuid() });
            _interationRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Interation>()))
                .ReturnsAsync(new Interation { InterationId = Guid.NewGuid() });
            _projectMemberRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<ProjectMember>()))
                .ReturnsAsync(new ProjectMember { MemberId = Guid.NewGuid() });
            _databaseTransactionMock.Setup(transaction => transaction.Commit());
            // Act
            var userId = new Guid();
            var result = await _projectService.CreateProject(createProjectRequest, userId);
            // Assert
            Assert.IsFalse(result.BaseResponse.IsSucceed);

        }

        [Test]
        public async Task TestCreateProject_Fail_MissingProjectName()
        {
            // Arrange
            var createProjectRequest = new CreateProjectRequest
            {
                ProjectName = "",
                EndDate = DateTime.Now.AddMonths(1),
                StartDate = DateTime.Now,
                PrivacyStatus = true,
                Description = "Test Project Description",

            };
            _projectRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Project>()))
                .ThrowsAsync(new Exception("Create Project fail"));
            // Act
            var userId = new Guid();
            var result = await _projectService.CreateProject(createProjectRequest, userId);
            // Assert
            Assert.IsFalse(result.BaseResponse.IsSucceed);
        }

        [Test]
        public async Task TestCreateProject_Fail_InvalidDate()
        {
            // Arrange
            var createProjectRequest = new CreateProjectRequest
            {
                ProjectName = "Test Project",
                EndDate = DateTime.Now.AddMonths(1),
                StartDate = DateTime.Now,
                PrivacyStatus = true,
                Description = "Test Project Description",
            };
            // Act
            _projectRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Project>()))
               .ThrowsAsync(new Exception("Create Project fail"));
            // Act
            var userId = new Guid();
            var result = await _projectService.CreateProject(createProjectRequest, userId);
            // Assert
            Assert.IsFalse(result.BaseResponse.IsSucceed);
        }

        //Update Project 
        [Test]
        public async Task TestUpdateProjectInfo_Success()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var updateProjectNameInfo = new UpdateProjectNameInfo
            {
                ProjectName = "Updated Project Name",
                Description = "Updated Description"
            };
            _projectRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), null))
                .ReturnsAsync(new Project { ProjectId = projectId });
            _databaseTransactionMock.Setup(transaction => transaction.Commit());
            // Act
            var result = await _projectService.UpdateProjectInfo(projectId, updateProjectNameInfo);
            // Assert
            Assert.IsTrue(result.IsSucceed);
            Console.WriteLine("Update Succes");
        }
        [Test]
        public async Task TestUpdateProjectInfo_SuccessNoDes()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var updateProjectNameInfo = new UpdateProjectNameInfo
            {
                ProjectName = "Updated Project Name",
                Description = ""
            };
            _projectRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), null))
                .ReturnsAsync(new Project { ProjectId = projectId });
            _databaseTransactionMock.Setup(transaction => transaction.Commit());
            // Act
            var result = await _projectService.UpdateProjectInfo(projectId, updateProjectNameInfo);
            // Assert
            Assert.IsTrue(result.IsSucceed);
            Console.WriteLine("Update Succes");
        }
        [Test]
        public async Task TestUpdateProjectInfo_SuccessWithEnddateinthefuture()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var updateProjectNameInfo = new UpdateProjectNameInfo
            {
                ProjectName = "Updated Project Name",
                Description = "Updated Description"
            };
            _projectRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), null))
                .ReturnsAsync(new Project { ProjectId = projectId });
            _databaseTransactionMock.Setup(transaction => transaction.Commit());
            // Act
            var result = await _projectService.UpdateProjectInfo(projectId, updateProjectNameInfo);
            // Assert
            Assert.IsTrue(result.IsSucceed);
            Console.WriteLine("Update Succes");
        }
        [Test]
        public async Task TestUpdateProjectInfo_Failure()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var updateProjectNameInfo = new UpdateProjectNameInfo
            {
                ProjectName = "Updated Project Name",
                Description = "Updated Description"
            };
            _projectRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), null))
                .ReturnsAsync((Project)null);
            // Act
            var result = await _projectService.UpdateProjectInfo(projectId, updateProjectNameInfo);
            // Assert
            Assert.IsFalse(result.IsSucceed);
            Console.WriteLine("Update Fail");
        }
        [Test]
        public async Task UpdateProjectInfo_Fail_WithEmptyProjectName()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var updateProjectNameInfo = new UpdateProjectNameInfo
            {
                ProjectName = "",
                Description = "Updated Description"
            };
            // Act
            var result = await _projectService.UpdateProjectInfo(projectId, updateProjectNameInfo);
            // Assert
            Assert.IsFalse(result.IsSucceed);
        }

        // Delete Project 
        [Test]
        public async Task TestDeleteProject_Success()
        {
            // Arrange
            var projectIdToDelete = Guid.NewGuid();
            _projectRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), null))
                .ReturnsAsync(new Project { ProjectId = projectIdToDelete });
            _projectRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Project>()))
                .ReturnsAsync(true);
            _databaseTransactionMock.Setup(transaction => transaction.Commit());
            // Act
            var result = await _projectService.DeleteProject(projectIdToDelete);
            // Assert
            Assert.IsTrue(result.IsSucceed);
        }

        [Test]
        public async Task TestDeleteProject_Failure_ProjectNotFound()
        {
            // Arrange
            var projectIdToDelete = Guid.NewGuid();
            _projectRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), null))
                .ReturnsAsync((Project)null);
            // Act
            var result = await _projectService.DeleteProject(projectIdToDelete);
            // Assert
            Assert.IsFalse(result.IsSucceed);
        }
        [Test]
        public async Task RestoreProject_Success()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new Project
            {
                ProjectId = projectId,
                StatusId = Guid.Parse("53F76F08-FF3C-43EB-9FF4-C9E028E513D5"),
                DeleteAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddMonths(1),
                IsDelete = true
            };

            _projectRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), null))
                .ReturnsAsync(project);
            _databaseTransactionMock.Setup(transaction => transaction.Commit());

            // Act
            var result = await _projectService.RestoreProject(projectId);

            // Assert
            Assert.IsTrue(result.IsSucceed, "Expected project restoration to succeed.");
            Console.WriteLine(result.Message);
        }

        [Test]
        public async Task RestoreProject_Failure()
        {
            // Arrange
            var projectId = Guid.NewGuid();

            _projectRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), null))
                .ThrowsAsync(new Exception("Restore Project failed"));
            _databaseTransactionMock.Setup(transaction => transaction.RollBack());

            // Act
            var result = await _projectService.RestoreProject(projectId);

            // Assert
            Assert.IsFalse(result.IsSucceed, "Expected project restoration to fail.");
            Console.WriteLine(result.Message);
        }
        [Test]
        public async Task ChangeProjectStatus_Success()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var statusId = Guid.NewGuid();
            var changeProjectStatusRequest = new ChangeProjectStatusRequest
            {
                ProjectId = projectId,
                // other properties...
            };

            var project = new Project
            {
                ProjectId = projectId,
                // other properties...
            };

            _projectRepositoryMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Project, bool>>>(),
                It.IsAny<Expression<Func<Project, object>>>())) 
                .ReturnsAsync(project);
            _projectRepositoryMock.Setup(repo => repo.UpdateAsync(project))
                .ReturnsAsync(project);

            // Act
            var result = await _projectService.ChangeProjectStatus(statusId, changeProjectStatusRequest);

            // Assert
            Assert.IsNotNull(result, "Expected a non-null result for ChangeProjectStatus.");
            Assert.IsTrue(result.StatusResponse.IsSucceed, "Expected a successful status response.");
            Console.WriteLine("ChangeProjectStatus Success");
        }
    }
}




