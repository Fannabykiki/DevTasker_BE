using Capstone.Common.DTOs.Board;
using Capstone.Common.DTOs.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Service.BoardService
{
    public interface IBoardService 
    {
        Task<bool> CreateBoard(CreateBoardRequest createBoardRequest, Guid iterationId);
        Task<bool> UpdateBoard(UpdateBoardRequest updateBoardRequest, Guid iterationId);
    }
}
