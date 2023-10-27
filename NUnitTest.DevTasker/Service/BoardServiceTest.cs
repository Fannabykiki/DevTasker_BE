/*using AutoMapper;
using Capstone.Common.DTOs.Board;
using Capstone.Common.Enums;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.BoardService;
using Capstone.Service.IterationService;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTest.DevTasker.Service
{
    [TestFixture]
    public class BoardServiceTest
    {
        private BoardService _boardService;
        private Mock<IBoardRepository> _boardRepositoryMock;
        private Mock<IInterationRepository> _iterationRepositoryMock;
        private Mock<IDatabaseTransaction> _transactionMock;
        private Mock<IMapper> _mapperMock;
        private Mock <IProjectRepository> _projectRepository;


        [SetUp]
        public void Setup()
        {
            _boardRepositoryMock = new Mock<IBoardRepository>();
            _iterationRepositoryMock = new Mock<IInterationRepository>();
            _transactionMock = new Mock<IDatabaseTransaction>();
            _mapperMock = new Mock<IMapper>();

            _boardService = new BoardService(
                null,
                _boardRepositoryMock.Object,
                _mapperMock.Object,
                _iterationRepositoryMock.Object,
                _projectRepository.Object

            );

           
        }

        [Test]
        public async Task CreateBoard_Success()
        {
            *//*// Arrange
            var createBoardRequest = new CreateBoardRequest
            {
                Title = "Test Board",
                Status = (StatusEnum?)BoardStatusEnum.InProgress,
            };
            var interationId = Guid.NewGuid();

            var fakeInteration = new Interation
            {
                InterationId = interationId,
                //Boards = new List<Board>()
            };

            _boardRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_transactionMock.Object);
            _iterationRepositoryMock.Setup(repo => repo.GetAsync( It.IsAny<Expression<Func<Interation, bool>>>(),null)).ReturnsAsync(fakeInteration);
            _boardRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Board>()))
                .ReturnsAsync((Board newBoard) =>
                {
                    newBoard.BoardId = Guid.NewGuid();
                    return newBoard;
                });

            _iterationRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Interation>()))
                .Callback<Interation>(async (updatedInteration) =>
                {
                    //fakeInteration.Boards.Add(updatedInteration.Boards.Single());
                });

            // Act
            var result = await _boardService.CreateBoard(createBoardRequest, interationId);

            // Assert
            Console.WriteLine(result ? "Create Board success" : "Create Board failed");
            Assert.IsTrue(result);*//*
            
        }

        [Test]
        public async Task CreateBoard_Fail_MissingTitle()
        {
            *//* // Arrange
             var createBoardRequest = new CreateBoardRequest
             {
                 Title = null,
                 Status = (StatusEnum?)BoardStatusEnum.InProgress,
             };
             var interationId = Guid.NewGuid();
             _boardRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_transactionMock.Object);
             // Act
             var result = await _boardService.CreateBoard(createBoardRequest, interationId);

             // Assert
             Console.WriteLine(result ? "Create Board success" : "Create Board failed");
             *//*
            Assert.IsFalse(result);

        }

        [Test]
        public async Task UpdateBoard_Success()
        {*//*
            // Arrange
            var boardId = Guid.NewGuid();
            var updateBoardRequest = new UpdateBoardRequest
            {
                Title = "Updated Board Title",
                Status = (StatusEnum?)BoardStatusEnum.Closed, 
                InterationId = Guid.NewGuid(), 
            };
            var fakeBoard = new Board
            {
                BoardId = boardId,
                Title = "Initial Board Title",
                Status = (StatusEnum?)BoardStatusEnum.InProgress, 
                //InterationId = Guid.NewGuid(),
            };
            _boardRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_transactionMock.Object);
            _boardRepositoryMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Board, bool>>>(),null)).ReturnsAsync(fakeBoard);

            // Act
            var result = await _boardService.UpdateBoard(updateBoardRequest, boardId);

            // Assert
            Console.WriteLine(result ? "Update Board success" : "Update Board failed");
            Assert.IsTrue(result); *//*

        }
        [Test]
        public async Task UpdateBoard_Fail_MissingTitle()
        {
           *//* // Arrange
            var updateBoardRequest = new UpdateBoardRequest
            {
                Status = (StatusEnum?)BoardStatusEnum.InProgress,
                InterationId = Guid.NewGuid(),
            };
            var boardId = Guid.NewGuid();

            // Configure the mock objects
            _boardRepositoryMock.Setup(repo => repo.DatabaseTransaction()).Returns(_transactionMock.Object);
            _boardRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<Board, bool>>>(),null)).ReturnsAsync((Board)null);

            // Act
            var result = await _boardService.UpdateBoard(updateBoardRequest, boardId);

            // Assert
            Console.WriteLine(result ? "Update Board success" : "Update Board failed");
            Assert.IsFalse(result);*//*
           
        }


    }
}
*/