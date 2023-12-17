using AutoMapper;
using Capstone.Common.DTOs.Notification;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.Hubs;
using Capstone.Service.NotificationService;
using Moq;
using NUnit.Framework;
using Task = System.Threading.Tasks.Task;


namespace NUnitTest.DevTasker.Service
{
    [TestFixture]
    public class NotificationServiceTest
    {
        private readonly CapstoneContext _context;
        private NotificationService NotificationService;
        private INotificationRepository _notificationRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<IProjectRepository> _projectRepository;
        private Mock<IProjectMemberRepository> _projectMemberRepository;
        private Mock<ITaskRepository> _taskRepository;
        private Mock<ITaskCommentRepository> _taskCommentRepository;
        private Mock<IBoardStatusRepository> _boardStatusRepository;
        private readonly PresenceTracker _presenceTracker;
        private Mock<Microsoft.AspNetCore.SignalR.IHubContext<NotificationHub>> _hubContext;
        private Mock<IMapper> _mapper;

        [SetUp]
        public void Setup()
        {

            _notificationRepository = new NotificationRepository(_context);
            _userRepository = new Mock<IUserRepository>();
            _projectRepository = new Mock<IProjectRepository>();
            _projectMemberRepository = new Mock<IProjectMemberRepository>();
            _taskRepository = new Mock<ITaskRepository>();
            _taskCommentRepository = new Mock<ITaskCommentRepository>();
            _boardStatusRepository = new Mock<IBoardStatusRepository>();
            _hubContext = new Mock<Microsoft.AspNetCore.SignalR.IHubContext<NotificationHub>>();
            _mapper = new Mock<IMapper>();

            NotificationService = new NotificationService(
               _notificationRepository,
                _userRepository.Object,
                _projectRepository.Object,
                _projectMemberRepository.Object,
                _taskRepository.Object,
                _taskCommentRepository.Object,
                _boardStatusRepository.Object,
                _presenceTracker,
                _hubContext.Object,
                _mapper.Object
                );
        }
    }
}
