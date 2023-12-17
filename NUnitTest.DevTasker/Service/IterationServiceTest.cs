using AutoMapper;
using Capstone.Common.DTOs.Iteration;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.IterationService;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace Capstone.UnitTests.Service
{
    [TestFixture]
    public class IterationServiceTests
    {
        private readonly CapstoneContext _context;
        private IterationService _iterationService;
        private Mock<IInterationRepository> _iterationRepositoryMock;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IDatabaseTransaction> _transactionMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IBoardRepository> _boardRepositoryMock;
        private Mock<ITaskRepository> _TaskRepository;
        private Mock <IStatusRepository> _statusRepositoryMock;
        private Mock <ITaskTypeRepository> _TaskTypeRepository;
        private Mock<IUserRepository> _userRepository;
        [SetUp]
        public void Setup()
        {
            _iterationRepositoryMock = new Mock<IInterationRepository>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _transactionMock = new Mock<IDatabaseTransaction>();
            _mapperMock = new Mock<IMapper>();
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _TaskRepository = new Mock<ITaskRepository>();
            _statusRepositoryMock = new Mock<IStatusRepository>();
            _TaskTypeRepository = new Mock<ITaskTypeRepository>();
            _userRepository = new Mock<IUserRepository>();

            _iterationRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_transactionMock.Object);

            _iterationService = new IterationService(
                _context,
                _projectRepositoryMock.Object,
                _mapperMock.Object,
                _iterationRepositoryMock.Object,
                _boardRepositoryMock.Object,
               _TaskRepository.Object,
               _statusRepositoryMock.Object,
               _TaskTypeRepository.Object,
               _userRepository.Object
            );
        }
        [Test]
        public async Task CreateInteration_Successful()
        {
            // Arrange
            var createIterationRequest = new CreateIterationRequest
            {
                InterationName = "Test Iteration",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7),
                ProjectId = Guid.NewGuid()
            };

            var newIterationId = Guid.NewGuid();
            var newIteration = new Interation
            {
                InterationId = newIterationId,
                InterationName = createIterationRequest.InterationName,
                StartDate = DateTime.Parse(createIterationRequest.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                EndDate = DateTime.Parse(createIterationRequest.EndDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                BoardId = createIterationRequest.ProjectId,
                StatusId = Guid.Parse("093416CB-1A26-43A4-9E11-DBDF5166DF4A"),
            };

            _iterationRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Interation>())).ReturnsAsync(newIteration);

            // Act
            var result = await _iterationService.CreateInteration(createIterationRequest);

            // Assert
            //Assert.IsTrue(result.Response.IsSucceed, "Expected IsSucceed to be true.");
            //Assert.AreEqual("Create successfully", result.Response.Message, "Expected Message to be 'Create successfully'.");
            //Assert.AreEqual(newIterationId, result.InterationId, "Expected InterationId to match.");
        }
        [Test]
        public async Task CreateInteration_FailIterationNameEmpty()
        {
            // Arrange
            var createIterationRequest = new CreateIterationRequest
            {
                InterationName = "",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7),
                ProjectId = Guid.NewGuid()
            };

            _iterationRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Interation>()))
                .ThrowsAsync(new Exception("Create iteration failed."));

            // Act
            var result = await _iterationService.CreateInteration(createIterationRequest);

            // Assert
            Assert.IsFalse(result.Response.IsSucceed, "Expected IsSucceed to be false.");
        }
        [Test]
        public async Task CreateInteration_FailStartDateBeforEndDate()
        {
            // Arrange
            var createIterationRequest = new CreateIterationRequest
            {
                InterationName = "hihi",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(-7),
                ProjectId = Guid.NewGuid()
            };

            _iterationRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Interation>()))
                .ThrowsAsync(new Exception("Create iteration failed."));

            // Act
            var result = await _iterationService.CreateInteration(createIterationRequest);

            // Assert
            Assert.IsFalse(result.Response.IsSucceed, "Expected IsSucceed to be false.");
        }
        [Test]
        public async Task CreateInteration_Fail()
        {
            // Arrange
            var createIterationRequest = new CreateIterationRequest
            {
                InterationName = "Test Iteration",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7),
                ProjectId = Guid.NewGuid()
            };

            _iterationRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Interation>()))
                .ThrowsAsync(new Exception("Create iteration failed."));

            // Act
            var result = await _iterationService.CreateInteration(createIterationRequest);

            // Assert
            Assert.IsFalse(result.Response.IsSucceed, "Expected IsSucceed to be false.");
        }
        [Test]
        public async Task UpdateIteration_Fail()
        {
            // Arrange
            var updateIterationRequest = new UpdateIterationRequest
            {
              
                InterationName = "Updated Iteration",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(14)
            };

            _iterationRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Interation, bool>>>(), null))
                                    .ReturnsAsync((Interation)null); 

            // Act
            var iterationId = Guid.NewGuid();
            var result = await _iterationService.UpdateIterationRequest(updateIterationRequest, iterationId);

            // Assert
            Assert.IsFalse(result.IsSucceed);

        }
        [Test]
        public async Task UpdateIteration_Fail_InvalidIterationName()
        {
            // Arrange
            var updateIterationRequest = new UpdateIterationRequest
            {

                InterationName = "",  // IterationName is empty
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(14)
            };

            // Act
            var iterationId = Guid.NewGuid();
            var result = await _iterationService.UpdateIterationRequest(updateIterationRequest, iterationId);

            // Assert
            Assert.IsFalse(result.IsSucceed);
            Console.WriteLine("Update iteration Fail");

        }
        [Test]
        public async Task UpdateIteration_Fail_InvalidIterationNameduplicate()
        {
            // Arrange
            var updateIterationRequest = new UpdateIterationRequest
            {

                InterationName = "duplicate",  // IterationName is duplicate
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(14)
            };

            // Act
            var iterationId = Guid.NewGuid();
            var result = await _iterationService.UpdateIterationRequest(updateIterationRequest, iterationId);

            // Assert
            Assert.IsFalse(result.IsSucceed);
            Console.WriteLine("Update iteration Fail");

        }

        [Test]
        public async Task UpdateIteration_Fail_InvalidDateRange()
        {
            // Arrange
            var updateIterationRequest = new UpdateIterationRequest
            {
               
                InterationName = "Updated Iteration",
                StartDate = DateTime.UtcNow.AddDays(14), 
                EndDate = DateTime.UtcNow
            };

            // Act
            var iterationId = Guid.NewGuid();
            var result = await _iterationService.UpdateIterationRequest(updateIterationRequest, iterationId);

            // Assert
            Assert.IsFalse(result.IsSucceed);
            Console.WriteLine("Update iteration Fail");

        }
    }
}
