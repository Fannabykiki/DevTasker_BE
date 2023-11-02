using Capstone.Common.DTOs.Board;
using Capstone.Common.DTOs.Iteration;
using Capstone.Service.BoardService;
using Capstone.Service.LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.API.Controllers
{
    [Route("api/board-management")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IBoardService _boardService;

        public BoardController(ILoggerManager logger, IBoardService boardService)
        {
            _logger = logger;
            _boardService = boardService;
        }


        [HttpPost("Board")]
        public async Task<IActionResult> CreateBoard( Guid iterationId, CreateBoardRequest createBoardRequest)
        {
            var result = await _boardService.CreateBoard(createBoardRequest, iterationId);

            return Ok(result);
        }

        [HttpPut("Board/{boardId}")]
        public async Task<IActionResult> UpdateBoard( Guid boardId, UpdateBoardRequest updateBoardRequest)
        {
            var result = await _boardService.UpdateBoard(updateBoardRequest, boardId);
            if (result == null)
            {
                return StatusCode(500);
            }

            return Ok(result);
        }
    }
}
