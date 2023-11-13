using AutoMapper;
using Capstone.Common.DTOs.Task;
using Capstone.DataAccess;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.TaskService;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Task = Capstone.DataAccess.Entities.Task;

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
        private Mock <IPriorityRepository> _priorityRepository;

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

    }
}
