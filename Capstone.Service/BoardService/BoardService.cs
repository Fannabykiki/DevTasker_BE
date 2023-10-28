using AutoMapper;
using Capstone.Common.DTOs.Board;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Common.Enums;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Math.EC.Rfc7748;
using Capstone.Common.DTOs.User;

namespace Capstone.Service.BoardService
{
    public class BoardService : IBoardService
    {
        private readonly CapstoneContext _context;
        private readonly IBoardRepository _boardRepository;
        private readonly IMapper _mapper;
        private readonly IInterationRepository _iterationRepository;
        private readonly IProjectRepository _projectRepository;

        public BoardService(CapstoneContext context, IBoardRepository boardRepository, IMapper mapper, IInterationRepository iterationRepository, IProjectRepository projectRepository)
        {
            _context = context;
            _boardRepository = boardRepository;
            _mapper = mapper;
            _iterationRepository = iterationRepository;
            _projectRepository = projectRepository;
        }

        public async Task<bool> CreateBoard(CreateBoardRequest createBoardRequest, Guid projectId)
        {
            using var transaction = _boardRepository.DatabaseTransaction();

            /*try
            {
                var statusValue = (int)createBoardRequest.Status;
                var newBoard = new Board
                {
                    Title = createBoardRequest.Title,
                    CreateAt = DateTime.UtcNow,
                    Status = (StatusEnum?)(BoardStatusEnum)statusValue,
                    ProjectId = projectId
                };


                await _boardRepository.CreateAsync(newBoard);
                var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null);
                project.Boards.Add(newBoard);
                await _iterationRepository.UpdateAsync(project);

                _boardRepository.SaveChanges();
                _iterationRepository.SaveChanges();

                transaction.Commit();

                return true;
            }
            catch (Exception)
            {
                transaction.RollBack();
                return false;
            }*/
            return false;
        }

        public async Task<bool> UpdateBoard(UpdateBoardRequest updateBoardRequest, Guid boardId)
        {/*
            using (var transaction = _boardRepository.DatabaseTransaction())
            {
                try
                {
                   
                    var board = await _boardRepository.GetAsync(x => x.BoardId == boardId, null);
                    board.Title = updateBoardRequest.Title;
                    board.Status = updateBoardRequest.Status;
                    board.UpdateAt = DateTime.UtcNow;

                    if (updateBoardRequest.InterationId != null && updateBoardRequest.InterationId != board.InterationId)
                    {
                        board.InterationId = updateBoardRequest.InterationId;
                    }

                    await _boardRepository.UpdateAsync(board);
                     _boardRepository.SaveChanges();

                  
                    transaction.Commit();

                    return true;

                }
                catch (Exception)
                {
                    transaction.RollBack();
                    return false;
                }
            }*/
            return false;
        }
    }
}
