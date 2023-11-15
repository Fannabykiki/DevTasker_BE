using Capstone.Common.DTOs.Comments;
using Capstone.Common.DTOs.TicketComment;
using Capstone.DataAccess.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Service.TicketCommentService
{
    public interface ITaskCommentService
    {
        Task<IEnumerable<GetCommentResponse>> GetAllCommentByTaskID(Guid ticketId);
        Task<GetCommentResponse> CreateComment(CreateCommentRequest comment);
        Task<GetCommentResponse> UpdateComment(Guid id, UpdateCommentRequest updatedComment);
        Task<bool> RemoveComment(Guid id);
    }
}
