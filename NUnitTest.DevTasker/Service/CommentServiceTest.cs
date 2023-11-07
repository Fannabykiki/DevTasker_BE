using AutoMapper;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.TicketCommentService;
using Moq;
using NUnit.Framework;

namespace NUnitTest.DevTasker.Service
{
    [TestFixture]
    public class CommentServiceTest
    {
        private ITicketCommentService _iticketService; 
        private Mock <ITicketCommentRepository> _ticketCommentRepository;
        private Mock <ITicketRepository> _ticketRepository;
        private Mock <IUserRepository> _userRepository;
        private Mock<IMapper> _mapper;

        [SetUp]
        public void Setup()
        {
            _ticketCommentRepository = new Mock<ITicketCommentRepository>();
            _ticketRepository = new Mock<ITicketRepository>();
            _mapper = new Mock<IMapper>();
            _userRepository = new Mock<IUserRepository>();

            _iticketService = new TicketCommentService
                (
                _ticketCommentRepository.Object,
                _mapper.Object,
                _ticketRepository.Object,
                _userRepository.Object
                );
        }

      


    }
}
