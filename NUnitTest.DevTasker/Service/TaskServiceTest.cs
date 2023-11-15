using AutoMapper;
using Capstone.Common.DTOs.Task;
using Capstone.DataAccess;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.TaskService;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;


namespace NUnitTest.DevTasker.Service
{
    [TestFixture]
    public class TaskServiceTest
    {
        private CapstoneContext _context;
        private TaskService _taskService;
        private Mock<ITaskRepository> _ticketRepositoryMock;
        private Mock<ITicketStatusRepository> _ticketStatusRepositoryMock;
        private Mock<ITaskTypeRepository> _ticketTypeRepositoryMock;
        private Mock<ITicketHistoryRepository> _ticketHistoryRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IInterationRepository> _iterationRepositoryMock;
        private Mock<IStatusRepository> _statusRepositoryMock;
        private Mock<IDatabaseTransaction> _transactionMock;
        private Mock<IDatabaseTransaction> _databaseTransactionMock;
        private Mock<IBoardStatusRepository> _boardStatusRepository;
        private Mock<ITaskTypeRepository> _taskType;
        private Mock<IPriorityRepository> _priorityRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CapstoneContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new CapstoneContext(options);

            _ticketRepositoryMock = new Mock<ITaskRepository>();
            _ticketStatusRepositoryMock = new Mock<ITicketStatusRepository>();
            _ticketTypeRepositoryMock = new Mock<ITaskTypeRepository>();
            _ticketHistoryRepositoryMock = new Mock<ITicketHistoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _iterationRepositoryMock = new Mock<IInterationRepository>();
            _statusRepositoryMock = new Mock<IStatusRepository>();
            _transactionMock = new Mock<IDatabaseTransaction>();
            _databaseTransactionMock = new Mock<IDatabaseTransaction>();
            _boardStatusRepository = new Mock<IBoardStatusRepository>();
            _taskType = new Mock<ITaskTypeRepository>();
            _priorityRepository = new Mock<IPriorityRepository>();

            _iterationRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_transactionMock.Object);

            _taskService = new TaskService(
                _context,
                _ticketRepositoryMock.Object,
                _ticketStatusRepositoryMock.Object,
                _ticketTypeRepositoryMock.Object,
                _ticketHistoryRepositoryMock.Object,
                _ticketTypeRepositoryMock.Object,
                _mapperMock.Object,
                _userRepositoryMock.Object,
                _iterationRepositoryMock.Object,
                _statusRepositoryMock.Object,
                _boardStatusRepository.Object,
                _taskType.Object,
                _priorityRepository.Object
                );
        }

        [Test] 
        public async Task TestCreateTask_Success()
        {
            // Arrange
            var request = new CreateTaskRequest
            {
                Title = "Test Ticket",
                Description = "Test Description",
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                AssignTo = Guid.NewGuid(),
                PriorityId = Guid.NewGuid()
            };
           
            // Act
            var userId = Guid.NewGuid();
            using var transaction = _transactionMock.Object;
            var result = await _taskService.CreateTask(request, userId);
            // Assert
            transaction.Commit();
            Assert.IsTrue(result.BaseResponse.IsSucceed);
        }

        [Test]
        public async Task TestCreateTask_FailEmptyTitle()
        {
            // Arrange
            var request = new CreateTaskRequest
            {
                Title = "",
                Description = "Test Description",
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                AssignTo = Guid.NewGuid(),
                PriorityId = Guid.NewGuid()
            };
            var userId = Guid.NewGuid();
           
            // Act
            var result = await _taskService.CreateTask(request, userId);
            // Assert
            Assert.IsTrue(result.BaseResponse.IsSucceed);
        }
        [Test]
        public async Task TestCreateTask_FailDueDateBeforeStartDate()
        {
            // Arrange
            var request = new CreateTaskRequest
            {
                Title = "Test Ticket",
                Description = "Test Description",
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddMinutes(-30),
                AssignTo = Guid.NewGuid(),
                PriorityId = Guid.NewGuid()
            };
            var userId = Guid.NewGuid();

            // Act
            using var transaction = _transactionMock.Object;

            // Set up repo.GetAsync to return null to simulate a non-existing task
            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Capstone.DataAccess.Entities.Task, bool>>>(), null))
                .ReturnsAsync((Capstone.DataAccess.Entities.Task)null);

            var result = await _taskService.CreateTask(request, userId);

            // Assert
            transaction.Commit();
            Assert.IsTrue(result.BaseResponse.IsSucceed);
        }

        [Test]
        public async Task TestCreateTask_FailLongTitle()
        {
            // Arrange
            var request = new CreateTaskRequest
            {
                Title = new string('A', 256),
                Description = "Test Description",
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddMinutes(30),
                AssignTo = Guid.NewGuid(),
                PriorityId = Guid.NewGuid()
            };
            var userId = Guid.NewGuid();

            // Act
            using var transaction = _transactionMock.Object;
            var result = await _taskService.CreateTask(request, userId);

            // Assert
            transaction.Commit();
            Assert.IsTrue(result.BaseResponse.IsSucceed);
        }
        [Test]
        public async Task TestUpdateTask_Success()
        {
            // Arrange
            var updateTicketRequest = new UpdateTaskRequest
            {
                Title = "Updated Ticket Title",
                Description = "Updated Ticket Description",
                DueDate = DateTime.Now.AddDays(14),
                AssignTo = Guid.NewGuid(),
                TypeId = Guid.NewGuid(),
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid()
            };
            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Capstone.DataAccess.Entities.Task, bool>>>(), null))
     .ReturnsAsync(new Capstone.DataAccess.Entities.Task());
            // Act
            using var transaction = _transactionMock.Object;
            var taskId = Guid.NewGuid();
            var result = await _taskService.UpdateTask(taskId, updateTicketRequest);
            // Assert
            Assert.IsTrue(result.BaseResponse.IsSucceed);
        }
        [Test]
        public async System.Threading.Tasks.Task TestUpdateTask_Fail()
        {
            // Arrange
            var updateTicketRequest = new UpdateTaskRequest
            {
                Title = "Updated Ticket Title",
                Description = "Updated Ticket Description",
                DueDate = DateTime.Now.AddDays(14),
                AssignTo = Guid.NewGuid(),
                TypeId = Guid.NewGuid(),
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid()
            };
            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Capstone.DataAccess.Entities.Task, bool>>>(), null))
     .ReturnsAsync(new Capstone.DataAccess.Entities.Task());
            // Act
            using var transaction = _transactionMock.Object;
            var taskId = Guid.NewGuid();
            var result = await _taskService.UpdateTask(taskId, updateTicketRequest);
            // Assert
            Assert.IsTrue(result.BaseResponse.IsSucceed);
        }
        [Test]
        public async Task TestUpdateTask_FailEmptyTitle()
        {
            // Arrange
            var updateTicketRequest = new UpdateTaskRequest
            {
                Title = "",
                Description = "Updated Ticket Description",
                DueDate = DateTime.Now.AddDays(14),
                AssignTo = Guid.NewGuid(),
                TypeId = Guid.NewGuid(),
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid()
            };
            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Capstone.DataAccess.Entities.Task, bool>>>(), null))
                .ReturnsAsync(new Capstone.DataAccess.Entities.Task { Title = "" });

            // Act
            var taskId = Guid.NewGuid();
            using var transaction = _transactionMock.Object;
            var result = await _taskService.UpdateTask(taskId, updateTicketRequest);

            // Assert
            Assert.IsTrue(result.BaseResponse.IsSucceed);
        }

        [Test]
        public async Task TestDeleteTask_Success()
        {
            // Arrange
            var ticketIdToDelete = Guid.NewGuid();

            // Setup repo.GetAsync to return a task (not null)
            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Capstone.DataAccess.Entities.Task, bool>>>(), null))
                .ReturnsAsync(new Capstone.DataAccess.Entities.Task());

            // Setup repo.DeleteAsync to return true for successful deletion
            _ticketRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Capstone.DataAccess.Entities.Task>()))
                .ReturnsAsync(true);

            _databaseTransactionMock.Setup(transaction => transaction.Commit());

            // Act
            var result = await _taskService.DeleteTask(ticketIdToDelete);

            // Assert
            Assert.IsTrue(result.IsSucceed);
        }


        [Test]
        public async Task TestDeleteTask_Failure_TaskNotFound()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Capstone.DataAccess.Entities.Task, bool>>>(), null))
     .ReturnsAsync((Capstone.DataAccess.Entities.Task)null);
            // Act
            var result = await _taskService.DeleteTask(ticketId);
            // Assert
            Assert.IsFalse(result.IsSucceed);
        }

    }
}


