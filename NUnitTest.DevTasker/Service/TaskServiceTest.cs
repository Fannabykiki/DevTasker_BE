using AutoMapper;
using Capstone.Common.DTOs.Task;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Capstone.Service.TaskService;
using Task = Capstone.DataAccess.Entities.Task;
using Capstone.DataAccess.Repository.Implements;

namespace NUnitTest.DevTasker.Service
{
    public class TicketServiceTest
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
        private Mock <ITaskTypeRepository> _taskType;
        private Mock <ISubTaskRepository> _subTaskRepository;

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
            _subTaskRepository = new Mock<ISubTaskRepository>();
            _taskType = new Mock<ITaskTypeRepository>();

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
                _subTaskRepository.Object
                );

        }
        // Create Ticket

        [Test]
        public async System.Threading.Tasks.Task TestCreateTicket_Success()
        {
            // Arrange
            var request = new CreateTaskRequest
            {
                Title = "Test Ticket",
                Decription = "Test Description",
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                AssignTo = Guid.NewGuid(),
                PriorityId = Guid.NewGuid()
            };

            var iterationId = Guid.NewGuid();

            // Act
            var userId = Guid.NewGuid();
            using var transaction = _transactionMock.Object;
            //var result = await _taskService.CreateTask(request, iterationId, userId);

            // Assert
            transaction.Commit();
            //Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task TestCreateTicket_FailEmptyTitle()
        {
            // Arrange
            var request = new CreateTaskRequest
            {
                Title = "",
                Decription = "Test Description",
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                AssignTo = Guid.NewGuid(),
                PriorityId = Guid.NewGuid()
            };
            var userId = Guid.NewGuid();
            var iterationId = Guid.NewGuid();
            // Act
            //var result = await _taskService.CreateTask(request, iterationId, userId);

            // Assert

            //Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task TestCreateTicket_FailDueDateBeforeStartDate()
        {
            /* // Arrange
             var request = new CreateTicketRequest
             {
                 Title = "Test Ticket",
                 Decription = "Test Description",
                 StartDate = DateTime.Now,
                 DueDate = DateTime.Now.AddMinutes(-30),  
                 CreateTime = DateTime.Now,
                 AssignTo = Guid.NewGuid(),
                 ByUser = Guid.NewGuid(),
                 PriorityId = Guid.NewGuid()
             };

             var iterationId = Guid.NewGuid();

             // Act
             using var transaction = _transactionMock.Object;
             var result = await _ticketService.CreateTicket(request, iterationId);

             // Assert
             transaction.Commit();
             Assert.IsTrue(result);*/

        }

        [Test]
        public async System.Threading.Tasks.Task TestCreateTicket_FailLongTitle()
        {/*
            // Arrange
            var request = new CreateTicketRequest
            {
                Title = new string('A', 256), 
                Decription = "Test Description",
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddMinutes(30),
                CreateTime = DateTime.Now,
                AssignTo = Guid.NewGuid(),
                ByUser = Guid.NewGuid(),
                PriorityId = Guid.NewGuid()
            };

            var iterationId = Guid.NewGuid();

            // Act
            using var transaction = _transactionMock.Object;
            var result = await _ticketService.CreateTicket(request, iterationId);

            // Assert
            transaction.Commit();
            Assert.IsFalse(result);*/
        }

        //Update  ticket
        [Test]
        public async System.Threading.Tasks.Task TestUpdateTicket_Success()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var updateTicketRequest = new UpdateTaskRequest
            {
                Title = "Updated Ticket Title",
                Decription = "Updated Ticket Description",
                DueDate = DateTime.Now.AddDays(14),
                AssignTo = Guid.NewGuid(),
                TypeId = Guid.NewGuid(),
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid()
            };

            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Task, bool>>>(), null))
                .ReturnsAsync(new Task { TaskId = ticketId });

            // Act
            using var transaction = _transactionMock.Object;
            var result = await _taskService.UpdateTask(updateTicketRequest, ticketId);



            // Assert
            Assert.IsTrue(result);

        }
        [Test]
        public async System.Threading.Tasks.Task TestUpdateTicket_Fail()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var updateTicketRequest = new UpdateTaskRequest
            {
                Title = "Updated Ticket Title",
                Decription = "Updated Ticket Description",
                DueDate = DateTime.Now.AddDays(14),
                AssignTo = Guid.NewGuid(),
                TypeId = Guid.NewGuid(),
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid()
            };

            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Task, bool>>>(), null))
                .ReturnsAsync((Task)null);

            // Act
            using var transaction = _transactionMock.Object;
            var result = await _taskService.UpdateTask(updateTicketRequest, ticketId);

            // Assert
            Assert.IsTrue(result);

        }

        [Test]
        public async System.Threading.Tasks.Task TestUpdateTicket_FailEmptyTitle()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var updateTicketRequest = new UpdateTaskRequest
            {
                Title = "",
                Decription = "Updated Ticket Description",
                DueDate = DateTime.Now.AddDays(14),
                AssignTo = Guid.NewGuid(),
                TypeId = Guid.NewGuid(),
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid()
            };

            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Task, bool>>>(), null))
                .ReturnsAsync(new Task { TaskId = ticketId });

            // Act
            using var transaction = _transactionMock.Object;
            var result = await _taskService.UpdateTask(updateTicketRequest, ticketId);


            // Assert
            Assert.IsTrue(result);

        }

        [Test]
        public async System.Threading.Tasks.Task TestDeleteTicket_Success()
        {
            // Arrange
            var ticketIdToDelete = Guid.NewGuid();

            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Task, bool>>>(), null))
                .ReturnsAsync(new Task { TaskId = ticketIdToDelete });

            _ticketRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Task>()))
                .ReturnsAsync(true);

            _databaseTransactionMock.Setup(transaction => transaction.Commit());

            // Act
            var result = await _taskService.DeleteTask(ticketIdToDelete);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async System.Threading.Tasks.Task TestDeleteTicket_Failure_TicketNotFound()
        {
            // Arrange
            var ticketIdToDelete = Guid.NewGuid();

            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Task, bool>>>(), null))
                .ReturnsAsync((Task)null);

            // Act
            var result = await _taskService.DeleteTask(ticketIdToDelete);

            // Assert
            Assert.IsFalse(result);
        }

    }
}
