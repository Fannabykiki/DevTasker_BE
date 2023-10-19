using Capstone.Common.DTOs.Board;
using Capstone.Common.DTOs.Iteration;
using Capstone.Service.BoardService;
using Capstone.Service.LoggerService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.API.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IActionResult> CreateBoard(CreateBoardRequest createBoardRequest, Guid iterationId)
        {
            var result = await _boardService.CreateBoard(createBoardRequest, iterationId);

            return Ok(result);
        }
    }
}
