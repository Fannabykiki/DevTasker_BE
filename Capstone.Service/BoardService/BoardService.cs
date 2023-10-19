using AutoMapper;
using Capstone.Common.DTOs.Board;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.Service.BoardService
{
    public class BoardService : IBoardService
    {
        private readonly CapstoneContext _context;
        private readonly IBoardRepository _boardRepository;
        private readonly IMapper _mapper;
        private readonly IInterationRepository _iterationRepository;

        public BoardService(CapstoneContext context, IBoardRepository boardRepository, IMapper mapper, IInterationRepository iterationRepository)
        {
            _context = context;
            _boardRepository = boardRepository;
            _mapper = mapper;
            _iterationRepository = iterationRepository;
        }

        public async Task<bool> CreateBoard(CreateBoardRequest createBoardRequest, Guid interationId)
        {
            using var transaction = _boardRepository.DatabaseTransaction();

            try
            {
                var newBoard = new Board
                {
                    Title = createBoardRequest.Title,
                    CreateAt = DateTime.UtcNow,
                    InterationId = interationId
                };

               
                await _boardRepository.CreateAsync(newBoard);
                var iteration = await _iterationRepository.GetAsync(x => x.InterationId == interationId, null);
                iteration.Boards.Add(newBoard);
                await _iterationRepository.UpdateAsync(iteration);
               
                _boardRepository.SaveChanges();
                _iterationRepository.SaveChanges();

                transaction.Commit();

                return true;
            }
            catch (Exception)
            {
                transaction.RollBack();
                return false;
            }
        }

        public Task<bool> UpdateBoard(UpdateBoardRequest updateBoardRequest, Guid iterationId)
        {
            throw new NotImplementedException();
        }
    }
}
