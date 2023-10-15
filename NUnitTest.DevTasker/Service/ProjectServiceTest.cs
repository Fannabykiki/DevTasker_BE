using AutoMapper;
using Capstone.Common.DTOs.Project;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.Project;
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

        [SetUp]
        public void Setup()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _projectMemberRepositoryMock = new Mock<IProjectMemberRepository>();
            _databaseTransactionMock = new Mock<IDatabaseTransaction>();

            _projectService = new ProjectService(
                null,
                _projectRepositoryMock.Object,
                _roleRepositoryMock.Object,
                null,
                null,
                _projectMemberRepositoryMock.Object,
                _boardRepositoryMock.Object
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
                Description = "Test Project Description"
            };

            // Thiết lập cho phương thức DatabaseTransaction() trả về transaction mock
            _projectRepositoryMock.Setup(repo => repo.DatabaseTransaction())
                .Returns(_databaseTransactionMock.Object);

            // Thiết lập cho phương thức CreateAsync trả về một Project mock
            _projectRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Project>()))
                .ReturnsAsync(new Project
                {
                    ProjectId = Guid.NewGuid(),
                    ProjectName = createProjectRequest.ProjectName,
                    // Thêm các thuộc tính khác nếu cần
                });

            // Thiết lập cho phương thức CreateAsync trả về một Board mock
            _boardRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Board>()))
                .ReturnsAsync(new Board
                {
                    BoardId = Guid.NewGuid(),
                    CreateAt = DateTime.UtcNow,
                    ProjectId = Guid.NewGuid(), // Sử dụng một Guid hợp lệ
                    Title = "",
                });

            // Thiết lập cho phương thức GetAsync trả về một Role mock
            _roleRepositoryMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Role, bool>>>(),
               It.IsAny<Expression<Func<Role, object>>>()

                ))
                .ReturnsAsync(new Role
                {
                    // Thiết lập Role theo nhu cầu
                });

            // Thiết lập cho phương thức CreateAsync trả về một ProjectMember mock
            _projectMemberRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<ProjectMember>()))
                .ReturnsAsync(new ProjectMember
                {
                    // Thiết lập ProjectMember theo nhu cầu
                });

            // Thiết lập cho phương thức Commit trên transaction mock
            _databaseTransactionMock.Setup(transaction => transaction.Commit());

            // Act
            var result = await _projectService.CreateProject(createProjectRequest);
            if (result)
            {
                Console.WriteLine("Success"); // Kết quả là thành công
            }
            else
            {
                Console.WriteLine("Fail"); // Kết quả là thất bại
            }
            // Assert
            Assert.IsTrue(result, "create success");
            // Kiểm tra xem các phương thức đã được gọi một lần
            _projectRepositoryMock.Verify(repo => repo.DatabaseTransaction(), Times.Once);
            _databaseTransactionMock.Verify(transaction => transaction.Commit(), Times.Once);
        }


        [Test]
        public async Task TestCreateProject_Fail()
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
                Description = "Test Project Description"
            };

            // Thiết lập cho phương thức DatabaseTransaction() trả về transaction mock
            _projectRepositoryMock.Setup(repo => repo.DatabaseTransaction())
                .Returns(_databaseTransactionMock.Object);

            // Thiết lập cho phương thức CreateAsync trả về null hoặc giá trị không hợp lệ
            _projectRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Project>()))
                .ReturnsAsync((Project)null); // Trả về null

            // Thiết lập cho phương thức Commit trên transaction mock
            _databaseTransactionMock.Setup(transaction => transaction.Commit());

            // Act
            var result = await _projectService.CreateProject(createProjectRequest);
            if (result)
            {
                Console.WriteLine("Success"); // Kết quả là thành công
            }
            else
            {
                Console.WriteLine("Fail"); // Kết quả là thất bại
            }
            // Assert

            Assert.IsFalse(result); 
            _projectRepositoryMock.Verify(repo => repo.DatabaseTransaction(), Times.Once);
            _databaseTransactionMock.Verify(transaction => transaction.Commit(), Times.Never); // Không commit khi thất bại.
        }

    }
}
