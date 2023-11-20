using AutoMapper;
using Capstone.Common.DTOs.Comments;
using Capstone.Common.DTOs.TicketComment;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
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
        private ITaskCommentService _commentService; 
        private Mock <ITaskCommentRepository> _ticketCommentRepository;
        private Mock <ITaskRepository> _ticketRepository;
        private Mock <IUserRepository> _userRepository;
        private Mock<IMapper> _mapper;
        private Mock<IDatabaseTransaction> _transactionMock;
        private Mock<IProjectMemberRepository> _projectMemberRepository;
        private Mock<IStatusRepository> _statusRepository;
        [SetUp]
        public void Setup()
        {
            _ticketCommentRepository = new Mock<ITaskCommentRepository>();
            _ticketRepository = new Mock<ITaskRepository>();
            _mapper = new Mock<IMapper>();
            _userRepository = new Mock<IUserRepository>();
            _projectMemberRepository= new Mock<IProjectMemberRepository>();
            _statusRepository = new Mock<IStatusRepository>();

            _commentService = new TaskCommentService
                (
                _ticketCommentRepository.Object,
                _mapper.Object,
                _ticketRepository.Object,
                _userRepository.Object,
                _projectMemberRepository.Object,
                _statusRepository.Object
                );
           
        }
        [Test]
        public async Task CreateComment_Success()
        {
            //Arrange
           var Comment = new CreateCommentRequest
           {
               Content = "Test Comment",
               TaskId = Guid.NewGuid()
           };

            var expectedCommentResponse = new GetCommentResponse
            {
                CommentId = It.IsAny<Guid>(),
                Content = Comment.Content,
                CreateAt = Comment.ToString(),
                TaskId = Comment.TaskId,
            };

            _mapper.Setup(m => m.Map<GetCommentResponse>(It.IsAny<TaskComment>()))
                .Returns(expectedCommentResponse);

            _ticketCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(new Mock<IDatabaseTransaction>().Object);

           // Act
           var byUserId = Guid.NewGuid();
           var result = await _commentService.CreateComment(byUserId, Comment);

            // Assert
            Assert.IsNull(result);
           
        }

        [Test]
        public async Task CreateComment_Failure()
        {
            // Arrange
            var createCommentRequest = new CreateCommentRequest
            {
                Content = "Test Comment",
                TaskId = Guid.NewGuid(),
               
            };

            _ticketCommentRepository.Setup(repo => repo.CreateAsync(It.IsAny<TaskComment>()))
                .Throws(new Exception("Failed to create comment"));
            var transaction = new Mock<IDatabaseTransaction>();
            transaction.Setup(t => t.Commit()).Verifiable();
            _ticketCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(transaction.Object);
            //// Act
            var byUserId = Guid.NewGuid();
            var result = await _commentService.CreateComment(byUserId, createCommentRequest);
            // Assert
            Assert.IsNull(result);
        }
        [Test]
        public async Task UpdateComment_Success()
        {
            // Arrange
            var Id = Guid.NewGuid();
            var updatedComment = new UpdateCommentRequest
            {
                Content = "Updated Test Comment"
            };

            var existingComment = new TaskComment
            {
                CommentId = Id,
                Content = "Test Comment",
                
            };

            _ticketCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<TaskComment, bool>>>(),
                It.IsAny<Expression<Func<TaskComment, object>>>()
            )).ReturnsAsync(existingComment);

            _ticketCommentRepository.Setup(repo => repo.UpdateAsync(existingComment));
            _ticketCommentRepository.Setup(repo => repo.SaveChanges());

            var expectedUpdatedCommentResponse = new GetCommentResponse 
            {
                CommentId = existingComment.CommentId,
                Content = updatedComment.Content
                
            };

            _mapper.Setup(m => m.Map<GetCommentResponse>(It.IsAny<TaskComment>()))
                .Returns(expectedUpdatedCommentResponse);

            // Act
           var result = await _commentService.UpdateComment(Id, updatedComment);

            // Assert
            Assert.IsNotNull(result);
           Assert.AreEqual(expectedUpdatedCommentResponse.Content, result.Content);
           
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
            //var result = await _commentService.UpdateComment(commentId, updatedCommentRequest);
            //if (result != null)
            //{
            //    Console.WriteLine("Comment Success");
            //}
            //else
            //{
            //    Console.WriteLine("Comment Fail");
            //}
            //// Assert
            //Assert.IsNull(result); 
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

            var ticketCommentService = new TaskCommentService(_ticketCommentRepository.Object, _mapper.Object, _ticketRepository.Object, _userRepository.Object, _projectMemberRepository.Object,
                _statusRepository.Object);

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

            var ticketCommentService = new TaskCommentService(_ticketCommentRepository.Object, _mapper.Object, _ticketRepository.Object, _userRepository.Object, _projectMemberRepository.Object, _statusRepository.Object);

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
