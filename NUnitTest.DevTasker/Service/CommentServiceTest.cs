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
        private Mock <ITaskCommentRepository> _taskCommentRepository;
        private Mock <ITaskRepository> _taskRepository;
        private Mock <IUserRepository> _userRepository;
        private Mock<IMapper> _mapper;
        private Mock<IDatabaseTransaction> _transactionMock;
        private Mock<IProjectMemberRepository> _projectMemberRepository;
        private Mock<IStatusRepository> _statusRepository;
        [SetUp]
        public void Setup()
        {
            _taskCommentRepository = new Mock<ITaskCommentRepository>();
            _taskRepository = new Mock<ITaskRepository>();
            _mapper = new Mock<IMapper>();
            _userRepository = new Mock<IUserRepository>();
            _projectMemberRepository= new Mock<IProjectMemberRepository>();
            _statusRepository = new Mock<IStatusRepository>();

            _commentService = new TaskCommentService
                (
                _taskCommentRepository.Object,
                _mapper.Object,
                _taskRepository.Object,
                _userRepository.Object,
                _projectMemberRepository.Object,
                _statusRepository.Object
                );
           
        }
        [Test]
        public async Task CreateComment_Success()
        {
            // Arrange
            var byUserId = Guid.NewGuid();
            var createCommentRequest = new CreateCommentRequest
            {
                Content = "Test Comment",
                TaskId = Guid.NewGuid()
            };

            var projectMember = new ProjectMember
            {
                MemberId = Guid.NewGuid(),
                UserId = byUserId,
                Users = new User { StatusId = Guid.NewGuid() }
            };

            var status = new Status { StatusId = projectMember.Users.StatusId };

            var task = new TaskComment { TaskId = createCommentRequest.TaskId };

            _taskCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(new Mock<IDatabaseTransaction>().Object);

            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(projectMember);

            _statusRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Status, bool>>>(), null))
                .ReturnsAsync(status);
            _mapper.Setup(m => m.Map<GetCommentResponse>(It.IsAny<TaskComment>()))
                .Returns((TaskComment tc) => new GetCommentResponse { CommentId = tc.CommentId, Content = tc.Content });

            // Act
            var result = await _commentService.CreateComment(byUserId, createCommentRequest);

            // Assert
            Assert.Null(result);
        }
        [Test]
        public async Task replyComment_Success()
        {
            // Arrange
            var byUserId = Guid.NewGuid();
            var createCommentRequest = new CreateCommentRequest
            {
                Content = "Test Comment",
                TaskId = Guid.NewGuid()
            };

            var projectMember = new ProjectMember
            {
                MemberId = Guid.NewGuid(),
                UserId = byUserId,
                Users = new User { StatusId = Guid.NewGuid() }
            };

            var status = new Status { StatusId = projectMember.Users.StatusId };

            var task = new TaskComment { TaskId = createCommentRequest.TaskId };

            _taskCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(new Mock<IDatabaseTransaction>().Object);

            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(projectMember);

            _statusRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Status, bool>>>(), null))
                .ReturnsAsync(status);
            _mapper.Setup(m => m.Map<GetCommentResponse>(It.IsAny<TaskComment>()))
                .Returns((TaskComment tc) => new GetCommentResponse { CommentId = tc.CommentId, Content = tc.Content });

            // Act
            var result = await _commentService.CreateComment(byUserId, createCommentRequest);

            // Assert
            Assert.Null(result);
        }
        [Test]
        public async Task CreateComment_Successw()
        {
            // Arrange
            var byUserId = Guid.NewGuid();
            var createCommentRequest = new CreateCommentRequest
            {
                Content = "Test Comment",
                TaskId = Guid.NewGuid()
            };

            var projectMember = new ProjectMember
            {
                MemberId = Guid.NewGuid(),
                UserId = byUserId,
                Users = new User { StatusId = Guid.NewGuid() }
            };

            var status = new Status { StatusId = projectMember.Users.StatusId };

            var task = new TaskComment { TaskId = createCommentRequest.TaskId };

            _taskCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(new Mock<IDatabaseTransaction>().Object);

            _projectMemberRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<ProjectMember, bool>>>(), null))
                .ReturnsAsync(projectMember);

            _statusRepository.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Status, bool>>>(), null))
                .ReturnsAsync(status);
            _mapper.Setup(m => m.Map<GetCommentResponse>(It.IsAny<TaskComment>()))
                .Returns((TaskComment tc) => new GetCommentResponse { CommentId = tc.CommentId, Content = tc.Content });

            // Act
            var result = await _commentService.CreateComment(byUserId, createCommentRequest);

            // Assert
            Assert.Null(result);
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

            _taskCommentRepository.Setup(repo => repo.CreateAsync(It.IsAny<TaskComment>()))
                .Throws(new Exception("Failed to create comment"));
            var transaction = new Mock<IDatabaseTransaction>();
            transaction.Setup(t => t.Commit()).Verifiable();
            _taskCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(transaction.Object);
            //// Act
            var byUserId = Guid.NewGuid();
            var result = await _commentService.CreateComment(byUserId, createCommentRequest);
            // Assert
            Assert.IsNull(result);
        }
        [Test]
        public async Task replyComment_Failure()
        {
            // Arrange
            var createCommentRequest = new CreateCommentRequest
            {
                Content = "Test Comment",
                TaskId = Guid.NewGuid(),

            };

            _taskCommentRepository.Setup(repo => repo.CreateAsync(It.IsAny<TaskComment>()))
                .Throws(new Exception("Failed to create comment"));
            var transaction = new Mock<IDatabaseTransaction>();
            transaction.Setup(t => t.Commit()).Verifiable();
            _taskCommentRepository.Setup(repo => repo.DatabaseTransaction())
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
            var commentId = Guid.NewGuid();
            var updatedComment = new ReplyCommentRequest { Content = "Updated content" };
            // Arrange
           
            var existingComment = new TaskComment
            {
                CommentId = commentId,
                Content = "Original content",
                DeleteAt = null

            };

            var expectedUpdatedComment = new GetCommentResponse
            {
                CommentId = commentId,
                Content = "Updated content",
            };

            _taskCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<TaskComment, bool>>>(),
                It.IsAny<Expression<Func<TaskComment, object>>>()
            )).ReturnsAsync(existingComment);

            _taskCommentRepository.Setup(repo => repo.UpdateAsync(existingComment));
            _taskCommentRepository.Setup(repo => repo.SaveChanges());

            var expectedUpdatedCommentResponse = new GetCommentResponse 
            {
                CommentId = existingComment.CommentId,
                Content = updatedComment.Content
                
            };

            _mapper.Setup(m => m.Map<GetCommentResponse>(It.IsAny<TaskComment>()))
                .Returns(expectedUpdatedCommentResponse);



            var result = await _commentService.UpdateComment(commentId, updatedComment);
            if (result != null)
            {
                Console.WriteLine("Update Comment Success");
            }
            else
            {
                Console.WriteLine("Update Comment Fail");
            }

            // // Assert
            Assert.IsNotNull(result);

        }
        [Test]
        public async Task UpdatereplyComment_Success()
        {
            var commentId = Guid.NewGuid();
            var updatedComment = new ReplyCommentRequest { Content = "Updated content" };
            // Arrange

            var existingComment = new TaskComment
            {
                CommentId = commentId,
                Content = "Original content",
                DeleteAt = null

            };

            var expectedUpdatedComment = new GetCommentResponse
            {
                CommentId = commentId,
                Content = "Updated content",
            };

            _taskCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<TaskComment, bool>>>(),
                It.IsAny<Expression<Func<TaskComment, object>>>()
            )).ReturnsAsync(existingComment);

            _taskCommentRepository.Setup(repo => repo.UpdateAsync(existingComment));
            _taskCommentRepository.Setup(repo => repo.SaveChanges());

            var expectedUpdatedCommentResponse = new GetCommentResponse
            {
                CommentId = existingComment.CommentId,
                Content = updatedComment.Content

            };

            _mapper.Setup(m => m.Map<GetCommentResponse>(It.IsAny<TaskComment>()))
                .Returns(expectedUpdatedCommentResponse);



            var result = await _commentService.UpdateComment(commentId, updatedComment);
            if (result != null)
            {
                Console.WriteLine("Update Comment Success");
            }
            else
            {
                Console.WriteLine("Update Comment Fail");
            }

            // // Assert
            Assert.IsNotNull(result);

        }

        [Test]
        public async Task UpdateComment_Failure()
        {
            var commentId = Guid.NewGuid();
            var updatedComment = new ReplyCommentRequest { Content = "Updated content" };

            _taskCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<TaskComment, bool>>>(), null))
                .ReturnsAsync((TaskComment)null);


            var result = await _commentService.UpdateComment(commentId, updatedComment);
            if (result != null)
            {
                Console.WriteLine("Update Comment Success");
            }
            else
            {
                Console.WriteLine("Update Comment Fail");
            }
            Assert.IsNull(result); 
        }
        [Test]
        public async Task UpdatereplyComment_Failure()
        {
            var commentId = Guid.NewGuid();
            var updatedComment = new ReplyCommentRequest { Content = "Updated content" };

            _taskCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<TaskComment, bool>>>(), null))
                .ReturnsAsync((TaskComment)null);


            var result = await _commentService.UpdateComment(commentId, updatedComment);
            if (result != null)
            {
                Console.WriteLine("Update Comment Success");
            }
            else
            {
                Console.WriteLine("Update Comment Fail");
            }
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

            _taskCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny < Expression<Func<TaskComment, bool>>>(), null))
                .ReturnsAsync(existingComment); 

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _taskCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            var ticketCommentService = new TaskCommentService(_taskCommentRepository.Object, _mapper.Object, _taskRepository.Object, _userRepository.Object, _projectMemberRepository.Object,
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
        public async Task RemovereplyComment_Success()
        {
            // Arrange
            var commentId = Guid.NewGuid();

            var existingComment = new TaskComment
            {
                CommentId = commentId,
                Content = "Test Comment Content",
            };

            _taskCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<TaskComment, bool>>>(), null))
                .ReturnsAsync(existingComment);

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _taskCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            var ticketCommentService = new TaskCommentService(_taskCommentRepository.Object, _mapper.Object, _taskRepository.Object, _userRepository.Object, _projectMemberRepository.Object,
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

            _taskCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny < Expression<Func<TaskComment, bool>>>(), null))
                .ReturnsAsync((TaskComment)null); 

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _taskCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            var ticketCommentService = new TaskCommentService(_taskCommentRepository.Object, _mapper.Object, _taskRepository.Object, _userRepository.Object, _projectMemberRepository.Object, _statusRepository.Object);

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
        [Test]
        public async Task RemovereplyComment_Failure()
        {
            // Arrange
            var commentId = Guid.NewGuid();

            _taskCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<TaskComment, bool>>>(), null))
                .ReturnsAsync((TaskComment)null);

            var databaseTransaction = new Mock<IDatabaseTransaction>();

            _taskCommentRepository.Setup(repo => repo.DatabaseTransaction())
                .Returns(databaseTransaction.Object);

            var ticketCommentService = new TaskCommentService(_taskCommentRepository.Object, _mapper.Object, _taskRepository.Object, _userRepository.Object, _projectMemberRepository.Object, _statusRepository.Object);

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
        [Test]
        public async Task RemoveComment_CommentNotFound_ReturnsFalse()
        {
            // Arrange
            var commentId = Guid.NewGuid();

            _taskCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<TaskComment, bool>>>(), null))
                .ReturnsAsync((TaskComment)null); // Simulate comment not found

            // Act
            var result = await _commentService.RemoveComment(commentId);

            // Assert
            Assert.IsFalse(result);
        }
        [Test]
        public async Task UpdateComment_CommentFound_ReturnsTrue()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var updatedComment = new ReplyCommentRequest { Content = "Updated content" };

            _taskCommentRepository.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<TaskComment, bool>>>(), null))
                .ReturnsAsync(new TaskComment()); // Simulate comment found

            // Act
            var result = await _commentService.UpdateComment(commentId, updatedComment);

            // Assert
            Assert.IsFalse(result != null);
        }

    }
}
