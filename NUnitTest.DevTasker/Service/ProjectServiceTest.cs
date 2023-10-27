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
            _databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            

            
            _projectRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_databaseTransactionMock.Object);

            //_projectService = new ProjectService(
            //    null,
            //    _projectRepositoryMock.Object,
            //    _roleRepositoryMock.Object,
            //    null,
            //    null,
            //    _projectMemberRepositoryMock.Object,
            //    _boardRepositoryMock.Object,
            //    _permissionRepositoryMock.Object,
            //    //_permissionRepositoryMock.Object,
            //    null
            //);
        }
        [Test]
        public async Task TestCreateProject_Success()
        {
           /* // Arrange
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
                .ReturnsAsync(new Project
                {
                    ProjectId = Guid.NewGuid(),
                    ProjectName = createProjectRequest.ProjectName,
                });
            // Act
            var result = await _projectService.CreateProject(createProjectRequest);
            if (result)
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("Fail");
            }
            // Assert

            Assert.IsTrue(result);
            _projectRepositoryMock.Verify(repo => repo.DatabaseTransaction(), Times.Once);
            _databaseTransactionMock.Verify(transaction => transaction.Commit(), Times.Never);*/
        }


        [Test]
        public async Task TestCreateProject_Fail()
        {
            // Arrange
            var createProjectRequest = new CreateProjectRequest
            {
                ProjectName = null,
                CreateAt = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                StartDate = DateTime.Now,
                PrivacyStatus = true,
                CreateBy = Guid.NewGuid(),
                Description = null,
            };

            _projectRepositoryMock.Setup(repo => repo.DatabaseTransaction())
                .Returns(_databaseTransactionMock.Object);


            _projectRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Project>()))
                .ReturnsAsync((Project)null);


            _databaseTransactionMock.Setup(transaction => transaction.Commit());

            // Act
            var result = await _projectService.CreateProject(createProjectRequest);

            // Assert
            if (result)
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("Fail");
            }
            Assert.IsFalse(result);
            
        }

        [Test]
        public async Task UpdateProjectInfo_Success()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var updateProjectNameInfo = new UpdateProjectNameInfo
            {
                ProjectName = "Valid Project Name",
                Description = "Valid Description"
            };

            _projectRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), null)).ReturnsAsync(new Project { ProjectId = projectId });
            _databaseTransactionMock.Setup(tx => tx.Commit());

            // Act
            var result = await _projectService.UpdateProjectInfo(projectId, updateProjectNameInfo);

            // Assert
            if (result)
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("Fail");
            }
            Assert.IsTrue(result);
           
        }

        [Test]
        public async Task UpdateProjectInfo_Fail()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var updateProjectNameInfo = new UpdateProjectNameInfo
            {
                ProjectName = "",  // ProjectName rỗng
                Description = "Valid Description"
            };
            _projectRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), null))
                .ReturnsAsync(new Project { ProjectId = projectId });
           
            _projectRepositoryMock.Setup(repo => repo.DatabaseTransaction())
                .Returns(_databaseTransactionMock.Object);

            _projectRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Project>()))
                .Throws<ArgumentException>();

            // Act
            var result = await _projectService.UpdateProjectInfo(projectId, updateProjectNameInfo);

            // Assert
            if (result)
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("Fail");
            }
            Assert.IsFalse(result);
           
        }

    }
}
