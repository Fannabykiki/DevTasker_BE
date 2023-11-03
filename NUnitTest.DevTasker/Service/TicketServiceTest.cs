using AutoMapper;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.DataAccess;
using Capstone.Service.TicketService;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capstone.Common.DTOs.Ticket;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace NUnitTest.DevTasker.Service
{
    public class TicketServiceTest
    {
        private  CapstoneContext _context;
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

        [SetUp]

        public void Setup()
        {
            _context = new CapstoneContext(new DbContextOptions<CapstoneContext>());
            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _ticketStatusRepositoryMock = new Mock<ITicketStatusRepository>();
            _ticketTypeRepositoryMock = new Mock<ITicketTypeRepository>();
            _ticketHistoryRepositoryMock = new Mock<ITicketHistoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _iterationRepositoryMock = new Mock<IInterationRepository>();
            _statusRepositoryMock = new Mock<IStatusRepository>();
            _transactionMock = new Mock<IDatabaseTransaction>();

            
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
            Assert.IsTrue(result);
        }

        [Test]
        public async Task TestCreateTicket_FailEmptyTitle()
        {
            // Arrange
            var request = new CreateTicketRequest
            {
                Title = "", // Trường Title rỗng
                Decription = "Test Description",
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
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
            Assert.IsFalse(result); 
        }

        [Test]
        public async Task TestCreateTicket_FailDueDateBeforeStartDate()
        {
            // Arrange
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
            Assert.IsFalse(result); 
        }

    }
}
