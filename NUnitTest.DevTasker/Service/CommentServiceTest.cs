using AutoMapper;
using Capstone.Common.DTOs.Comments;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.TicketCommentService;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

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
        private Mock<IDatabaseTransaction> _transactionMock;
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

        [Test]
        public async Task CreateComment_Success()
        {
            // Arrange
            var createCommentRequest = new CreateCommentRequest
            {
                Content = "Test Comment",
                TaskId = Guid.NewGuid(),
                ByUser = Guid.NewGuid()
            };

            var expectedComment = new TaskComment
            {
                CommentId = Guid.NewGuid(),
                Content = createCommentRequest.Content,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                TaskId = createCommentRequest.TaskId,
                CreateBy = createCommentRequest.ByUser,
            };
            _ticketCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<TaskComment, bool>>>(), null))
                .ReturnsAsync(expectedComment);

            var transaction = new Mock<IDatabaseTransaction>();
            transaction.Setup(t => t.Commit()).Verifiable();

            _ticketCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(transaction.Object);

            // Act
            var result = await _iticketService.CreateComment(createCommentRequest);
            if (result != null)
            {
                Console.WriteLine("Comment Success");
            }
            else
            {
                Console.WriteLine("Comment Fail");
            }
            // Assert
            //Assert.IsNotNull(result);
        }

        [Test]
        public async Task CreateComment_Failure()
        {
            // Arrange
            var createCommentRequest = new CreateCommentRequest
            {
                Content = "Test Comment",
                TaskId = Guid.NewGuid(),
                ByUser = Guid.NewGuid()
            };

            _ticketCommentRepository.Setup(repo => repo.CreateAsync(It.IsAny<TaskComment>()))
                .Throws(new Exception("Failed to create comment"));
            var transaction = new Mock<IDatabaseTransaction>();
            transaction.Setup(t => t.Commit()).Verifiable();
            _ticketCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(transaction.Object);
            // Act
            var result = await _iticketService.CreateComment(createCommentRequest);
            // Assert
            Assert.IsNull(result); 
        }

        [Test]
        public async Task UpdateComment_Success()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var updatedCommentRequest = new CreateCommentRequest
            {
                Content = "Updated Comment Content"
            };

            var existingComment = new TaskComment
            {
                CommentId = commentId,
                Content = "Original Comment Content",
            };

            _ticketCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny < Expression<Func<TaskComment, bool>>>(), null))
                .ReturnsAsync(existingComment);

            // Act
            var result = await _iticketService.UpdateComment(commentId, updatedCommentRequest);
            var transaction = new Mock<IDatabaseTransaction>();
            transaction.Setup(t => t.Commit()).Verifiable();
            _ticketCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(transaction.Object);
            // Assert
          //  Assert.IsNotNull(result);
            
           
        }


        [Test]
        public async Task UpdateComment_Failure()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var updatedCommentRequest = new CreateCommentRequest
            {
                Content = "Updated Comment Content"
            };

            _ticketCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<TaskComment, bool>>>(), null))
                .ReturnsAsync((TaskComment)null); 

            // Act
            var result = await _iticketService.UpdateComment(commentId, updatedCommentRequest);
            if (result != null)
            {
                Console.WriteLine("Comment Success");
            }
            else
            {
                Console.WriteLine("Comment Fail");
            }
            // Assert
            Assert.IsNull(result); 
        }

        [Test]
        public async Task RemoveComment_Success()
        {
            // Arrange
            var commentId = Guid.NewGuid();

            var existingComment = new TaskComment
            {
                CommentId = commentId,
                Content = "Test Comment Content",
            };

            _ticketCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny < Expression<Func<TaskComment, bool>>>(), null))
                .ReturnsAsync(existingComment); 

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _ticketCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            var ticketCommentService = new TicketCommentService(_ticketCommentRepository.Object, _mapper.Object, _ticketRepository.Object, _userRepository.Object);

            // Act
            var result = await ticketCommentService.RemoveComment(commentId);
            if (result != null)
            {
                Console.WriteLine("Delete comment Success");
            }
            else
            {
                Console.WriteLine("Delete comment Fail");
            }
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task RemoveComment_Failure()
        {
            // Arrange
            var commentId = Guid.NewGuid();

            _ticketCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny < Expression<Func<TaskComment, bool>>>(), null))
                .ReturnsAsync((TaskComment)null); 

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _ticketCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            var ticketCommentService = new TicketCommentService(_ticketCommentRepository.Object, _mapper.Object, _ticketRepository.Object, _userRepository.Object);

            // Act
            var result = await ticketCommentService.RemoveComment(commentId);
            if (result == null)
            {
                Console.WriteLine("Delete comment Success");
            }
            else
            {
                Console.WriteLine("Delete comment Fail");
            }
            // Assert
            Assert.IsFalse(result);
        }
    }
}
