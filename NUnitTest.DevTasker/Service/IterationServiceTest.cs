using AutoMapper;
using Capstone.Common.DTOs.Iteration;
using Capstone.Common.Enums;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.IterationService;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Capstone.UnitTests.Service
{
    [TestFixture]
    public class IterationServiceTests
    {
        private IterationService _iterationService;
        private Mock<IInterationRepository> _iterationRepositoryMock;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IDatabaseTransaction> _transactionMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IBoardRepository> _boardRepositoryMock;
        private readonly ITicketRepository _ticketRepository;
        [SetUp]
        public void Setup()
        {
            _iterationRepositoryMock = new Mock<IInterationRepository>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _transactionMock = new Mock<IDatabaseTransaction>();
            _mapperMock = new Mock<IMapper>();
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _iterationRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_transactionMock.Object);

            _iterationService = new IterationService(
                null,
                _projectRepositoryMock.Object, 
                _mapperMock.Object,
                 _iterationRepositoryMock.Object,
                 _boardRepositoryMock.Object,
                null
               );
        }
        [Test]
        public async Task CreateIteration_Success()
        {
            // Arrange
            var createIterationRequest = new CreateIterationRequest
            {
                InterationName = "New Iteration",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Status = InterationStatusEnum.Pass
            };

            var projectId = Guid.NewGuid();
            var newIteration = new Interation
            {
                InterationId = Guid.NewGuid(),
                InterationName = createIterationRequest.InterationName,
                StartDate = createIterationRequest.StartDate,
                EndDate = createIterationRequest.EndDate,
                ProjectId = projectId,
                Status = createIterationRequest.Status
            };

            _iterationRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Interation>()))
                .ReturnsAsync(newIteration);

            var project = new Project
            {
                ProjectId = projectId,
                Interations = new List<Interation>()
            };

            _projectRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Project, bool>>>(), null))
                .ReturnsAsync(project);

            var transaction = new Mock<IDatabaseTransaction>();
            transaction.Setup(t => t.Commit()).Verifiable();
            _iterationRepositoryMock.Setup(repo => repo.DatabaseTransaction())
                .Returns(transaction.Object);

            // Act
            var result = await _iterationService.CreateIteration(createIterationRequest, projectId);

            // Assert
            Assert.True(result);
            transaction.Verify(t => t.Commit(), Times.Once);
        }


        [Test]
        public async Task TestCreateIteration_Fail_InterationNameNull()
        {
            // Arrange
            var createRequest = new CreateIterationRequest
            {
                InterationName = null, // Invalid request
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Status = InterationStatusEnum.Pass
            };

            var projectId = Guid.NewGuid();
            _iterationRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Interation>())).ThrowsAsync(new Exception("Database commit failed"));
            var transaction = new Mock<IDatabaseTransaction>();
            _iterationRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(transaction.Object);

            // Act
            var result = await _iterationService.CreateIteration(createRequest, projectId);
            Assert.False(result);

            if (result)
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("Fail");
            }
            // Assert
            Assert.False(result);
            transaction.Verify(t => t.Commit(), Times.Never);
        }

        [Test]
        public async Task TestCreateIteration_Fail_EndDateBeforeStartDate()
        {
            // Arrange
            var createIterationRequest = new CreateIterationRequest
            {
                InterationName = "New Iteration",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(-7),  
                Status = InterationStatusEnum.Pass
            };

            var projectId = Guid.NewGuid();
            var transaction = new Mock<IDatabaseTransaction>();
            _iterationRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(transaction.Object);

            // Act
            var result = await _iterationService.CreateIteration(createIterationRequest, projectId);
            // Assert
            Assert.False(result);

            if (result)
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("Fail");
            }
            transaction.Verify(t => t.Commit(), Times.Never);
        }

        [Test]
        public async Task TestUpdateIterationRequest_Success()
        {
            // Arrange
            var updateRequest = new UpdateIterationRequest
            {
                InterationName = "Updated Iteration",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                Status = InterationStatusEnum.Current
            };

            var iterationId = Guid.NewGuid();

            var existingIteration = new Interation
            {
                InterationId = iterationId,
                InterationName = "Old Iteration",
                StartDate = DateTime.Now.AddDays(-7),
                EndDate = DateTime.Now,
                Status = InterationStatusEnum.Future
            };

            _iterationRepositoryMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Interation, bool>>>(), null))
                .ReturnsAsync(existingIteration);

            var transaction = new Mock<IDatabaseTransaction>();
            _iterationRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(transaction.Object);

            // Act
            var result = await _iterationService.UpdateIterationRequest(updateRequest, iterationId);

            // Assert
            Assert.True(result);

            if (result)
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("Fail");
            }
            Assert.AreEqual(updateRequest.InterationName, existingIteration.InterationName);
            Assert.AreEqual(updateRequest.StartDate, existingIteration.StartDate);
            Assert.AreEqual(updateRequest.EndDate, existingIteration.EndDate);
            Assert.AreEqual(updateRequest.Status, existingIteration.Status);
            transaction.Verify(t => t.Commit(), Times.Once);
        }

       /* [Test]
        public async Task TestUpdateIterationRequest_Fail_StartDateGreaterThanEndDate()
        {
            // Arrange
            var updateRequest = new UpdateIterationRequest
            {
                InterationName = "Updated Iteration",
                StartDate = DateTime.Now.AddDays(14), 
                EndDate = DateTime.Now,
                Status = InterationStatusEnum.Current
            };

            var iterationId = Guid.NewGuid();

            var existingIteration = new Interation
            {
                InterationId = iterationId,
                InterationName = "Old Iteration",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Status = InterationStatusEnum.Future
            };

            _iterationRepositoryMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Interation, bool>>>(), null))
                .ReturnsAsync(existingIteration);

            var transaction = new Mock<IDatabaseTransaction>();
            _iterationRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(transaction.Object);

            // Act
            var result = await _iterationService.UpdateIterationRequest(updateRequest, iterationId);

            // Assert
            Assert.False(result);
            Console.WriteLine(result ? "Success" : "Fail");
            transaction.Verify(t => t.Commit(), Times.Never);
        }*/

    }
}
