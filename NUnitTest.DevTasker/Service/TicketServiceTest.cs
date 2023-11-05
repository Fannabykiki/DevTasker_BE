using AutoMapper;
using Capstone.Common.DTOs.Ticket;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.TicketService;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;

namespace NUnitTest.DevTasker.Service
{
    public class TicketServiceTest
    {
        private CapstoneContext _context;
        private TicketService _ticketService;
        private Mock<ITicketRepository> _ticketRepositoryMock;
        private Mock<ITicketStatusRepository> _ticketStatusRepositoryMock;
        private Mock<ITicketTypeRepository> _ticketTypeRepositoryMock;
        private Mock<ITicketHistoryRepository> _ticketHistoryRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IInterationRepository> _iterationRepositoryMock;
        private Mock<IStatusRepository> _statusRepositoryMock;
        private Mock<IDatabaseTransaction> _transactionMock;
        private Mock<IDatabaseTransaction> _databaseTransactionMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CapstoneContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new CapstoneContext(options);

            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _ticketStatusRepositoryMock = new Mock<ITicketStatusRepository>();
            _ticketTypeRepositoryMock = new Mock<ITicketTypeRepository>();
            _ticketHistoryRepositoryMock = new Mock<ITicketHistoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _iterationRepositoryMock = new Mock<IInterationRepository>();
            _statusRepositoryMock = new Mock<IStatusRepository>();
            _transactionMock = new Mock<IDatabaseTransaction>();
            _databaseTransactionMock = new Mock<IDatabaseTransaction>();

            _iterationRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_transactionMock.Object);

            _ticketService = new TicketService(
                _context,
                _ticketRepositoryMock.Object,
                _ticketStatusRepositoryMock.Object,
                _ticketTypeRepositoryMock.Object,
                _ticketHistoryRepositoryMock.Object,
                _ticketTypeRepositoryMock.Object,
                _mapperMock.Object,
                _userRepositoryMock.Object,
                _iterationRepositoryMock.Object,
                _statusRepositoryMock.Object);

        }

        // Create Ticket

        [Test]
        public async Task TestCreateTicket_Success()
        {
            // Arrange
            var request = new CreateTicketRequest
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
            var result = await _ticketService.CreateTicket(request, iterationId, userId);

            // Assert
            transaction.Commit();
            //Assert.IsTrue(result);
        }

        [Test]
        public async Task TestCreateTicket_FailEmptyTitle()
        {
            // Arrange
            var request = new CreateTicketRequest
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
            var result = await _ticketService.CreateTicket(request, iterationId, userId);

            // Assert

            Assert.IsTrue(result);
        }

        [Test]
        public async Task TestCreateTicket_FailDueDateBeforeStartDate()
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
                 CreateBy = Guid.NewGuid(),
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
        public async Task TestCreateTicket_FailLongTitle()
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
                CreateBy = Guid.NewGuid(),
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
        public async Task TestUpdateTicket_Success()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var updateTicketRequest = new UpdateTicketRequest
            {
                Title = "Updated Ticket Title",
                Decription = "Updated Ticket Description",
                DueDate = DateTime.Now.AddDays(14),
                AssignTo = Guid.NewGuid(),
                TypeId = Guid.NewGuid(),
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid()
            };

            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Ticket, bool>>>(), null))
                .ReturnsAsync(new Ticket { TicketId = ticketId });

            // Act
            using var transaction = _transactionMock.Object;
            var result = await _ticketService.UpdateTicket(updateTicketRequest, ticketId);



            // Assert
            Assert.IsTrue(result);

        }
        [Test]
        public async Task TestUpdateTicket_Fail()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var updateTicketRequest = new UpdateTicketRequest
            {
                Title = "Updated Ticket Title",
                Decription = "Updated Ticket Description",
                DueDate = DateTime.Now.AddDays(14),
                AssignTo = Guid.NewGuid(),
                TypeId = Guid.NewGuid(),
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid()
            };

            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Ticket, bool>>>(), null))
                .ReturnsAsync((Ticket)null);

            // Act
            using var transaction = _transactionMock.Object;
            var result = await _ticketService.UpdateTicket(updateTicketRequest, ticketId);

            // Assert
            Assert.IsTrue(result);

        }

        [Test]
        public async Task TestUpdateTicket_FailEmptyTitle()
        {
            // Arrange
            var ticketId = Guid.NewGuid();
            var updateTicketRequest = new UpdateTicketRequest
            {
                Title = "",
                Decription = "Updated Ticket Description",
                DueDate = DateTime.Now.AddDays(14),
                AssignTo = Guid.NewGuid(),
                TypeId = Guid.NewGuid(),
                PriorityId = Guid.NewGuid(),
                StatusId = Guid.NewGuid()
            };

            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Ticket, bool>>>(), null))
                .ReturnsAsync(new Ticket { TicketId = ticketId });

            // Act
            using var transaction = _transactionMock.Object;
            var result = await _ticketService.UpdateTicket(updateTicketRequest, ticketId);


            // Assert
            Assert.IsTrue(result);

        }

        [Test]
        public async Task TestDeleteTicket_Success()
        {
            // Arrange
            var ticketIdToDelete = Guid.NewGuid();

            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Ticket, bool>>>(), null))
                .ReturnsAsync(new Ticket { TicketId = ticketIdToDelete });

            _ticketRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Ticket>()))
                .ReturnsAsync(true);

            _databaseTransactionMock.Setup(transaction => transaction.Commit());

            // Act
            var result = await _ticketService.DeleteTicket(ticketIdToDelete);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task TestDeleteTicket_Failure_TicketNotFound()
        {
            // Arrange
            var ticketIdToDelete = Guid.NewGuid();

            _ticketRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Ticket, bool>>>(), null))
                .ReturnsAsync((Ticket)null);

            // Act
            var result = await _ticketService.DeleteTicket(ticketIdToDelete);

            // Assert
            Assert.IsFalse(result);
        }

    }
}
