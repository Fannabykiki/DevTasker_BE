using AutoMapper;
using Capstone.Common.DTOs.Project;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.ProjectService;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;

namespace NUnitTest.DevTasker.Service
{
    public class ProjectServiceTest
    {
        private ProjectService _projectService;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IBoardRepository> _boardRepositoryMock;
        private Mock<IRoleRepository> _roleRepositoryMock;
        private Mock<IProjectMemberRepository> _projectMemberRepositoryMock;
        private Mock<IDatabaseTransaction> _databaseTransactionMock;
        private Mock<IPermissionRepository> _permissionRepositoryMock;
        
        private Mock<IInterationRepository> _interationRepositoryMock;
        private Mock<IDatabaseTransaction> _transactionMock;

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

            _projectRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_databaseTransactionMock.Object);

            _projectService = new ProjectService(
                null,
                _projectRepositoryMock.Object,
                _roleRepositoryMock.Object,
                null,
                null,
                _projectMemberRepositoryMock.Object,
                _boardRepositoryMock.Object,
                _permissionRepositoryMock.Object,
                _interationRepositoryMock.Object,
                null
            );
        }

        [Test]
        public async Task TestCreateProject_Success()
        {
            // Arrange
            var createProjectRequest = new CreateProjectRequest
            {
                ProjectName = "Test Project",
                CreateAt = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                StartDate = DateTime.Now,
                PrivacyStatus = true,
                CreateBy = Guid.NewGuid(),
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
            var result = await _projectService.CreateProject(createProjectRequest);

            // Assert
            Assert.IsTrue(result);
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
            var result = await _projectService.CreateProject(createProjectRequest);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestCreateProject_Fail_MissingProjectName()
        {
            // Arrange
            var createProjectRequest = new CreateProjectRequest
            {
                // Omitting ProjectName, which should cause a failure.
                CreateAt = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                StartDate = DateTime.Now,
                PrivacyStatus = true,
                CreateBy = Guid.NewGuid(),
                Description = "Test Project Description",
            };

            // Act
            var result = await _projectService.CreateProject(createProjectRequest);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestCreateProject_Fail_InvalidDate()
        {
            // Arrange
            var createProjectRequest = new CreateProjectRequest
            {
                ProjectName = "Test Project",
                CreateAt = DateTime.Now.AddMonths(1), // Create date is greater than the end date.
                EndDate = DateTime.Now.AddMonths(1),
                StartDate = DateTime.Now,
                PrivacyStatus = true,
                CreateBy = Guid.NewGuid(),
                Description = "Test Project Description",
            };

            // Act
            var result = await _projectService.CreateProject(createProjectRequest);

            // Assert
            Assert.IsFalse(result);
        }

    }
}
