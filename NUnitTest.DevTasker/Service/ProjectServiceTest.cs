using AutoMapper;
using Capstone.Common.DTOs.Project;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.ProjectService;
using Google.Apis.Drive.v3.Data;
using Moq;
using NUnit.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;
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
        private Mock <ISchemaRepository> _schemaRepository;

        private Mock<IInterationRepository> _interationRepositoryMock;
        private Mock<IDatabaseTransaction> _transactionMock;
        private Mock<IPermissionSchemaRepository> _permissionScemaRepo;

        [SetUp]
        public void Setup()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _projectMemberRepositoryMock = new Mock<IProjectMemberRepository>();
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _interationRepositoryMock = new Mock<IInterationRepository>();

            _databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _schemaRepository = new Mock<ISchemaRepository>();
            _permissionScemaRepo = new Mock<IPermissionSchemaRepository>();

            _projectRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_databaseTransactionMock.Object);

            //_projectService = new ProjectService(
            //    _context,
            //    _projectRepositoryMock.Object,
            //    _roleRepositoryMock.Object,
            //    _mapper,
            //    _schemaRepository.Object,
            //    _projectMemberRepositoryMock.Object,
            //    _boardRepositoryMock.Object,
            //    _permissionRepositoryMock.Object,
            //    _interationRepositoryMock.Object,
            //    _permissionScemaRepo.Object
            //);
            
          
        }

        [Test]
        public async Task TestCreateProject_Success()
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
            //Assert.IsTrue(result);
        }

        [Test]
        public async Task TestCreateProject_Failure()
        {
            // Arrange
            var createProjectRequest = new CreateProjectRequest
            {
               
            };

            _projectRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Project>()))
                .ThrowsAsync(new Exception("Simulated failure"));

            // Act
            var userId = new Guid();
            var result = await _projectService.CreateProject(createProjectRequest, userId);

            // Assert
            //Assert.IsFalse(result);
        }

        [Test]
        public async Task TestCreateProject_Fail_MissingProjectName()
        {
            // Arrange
            var createProjectRequest = new CreateProjectRequest
            {
                // Omitting ProjectName, which should cause a failure.
                ProjectName = "",
                EndDate = DateTime.Now.AddMonths(1),
                StartDate = DateTime.Now,
                PrivacyStatus = true,
                Description = "Test Project Description",
            };

            // Act
            var userId = new Guid();
            var result = await _projectService.CreateProject(createProjectRequest, userId);

            // Assert
            //Assert.IsFalse(result);
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
            var userId = new Guid();
            var result = await _projectService.CreateProject(createProjectRequest, userId);

            // Assert
            //Assert.IsFalse(result);
        }

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
            Assert.IsTrue(result);
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
            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateProjectInfo_Fail_WithEmptyProjectName()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var updateProjectNameInfo = new UpdateProjectNameInfo
            {
                ProjectName = "", // Trống ProjectName, một trường hợp thất bại.
                Description = "Updated Description"
            };

            // Act
            var result = await _projectService.UpdateProjectInfo(projectId, updateProjectNameInfo);

            // Assert
            Assert.IsFalse(result);
        }

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
            Assert.IsTrue(result);
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
            Assert.IsFalse(result);
        }
    }
}
