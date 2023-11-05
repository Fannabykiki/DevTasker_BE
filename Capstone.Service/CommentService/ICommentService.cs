using Capstone.Common.DTOs.Comments;
using Capstone.DataAccess.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Service.CommentService
{
    public interface ICommentService
    {
        Task<IEnumerable<GetCommentResponse>> GetAllCommentByTaskID(Guid ticketId);
        Task<GetCommentResponse> CreateComment(CreateCommentRequest comment);
        Task<IEnumerable> UpdateComment(Guid id, TicketComment updatedComment);
        Task<bool> DeleteComment(Guid id);
    }
}
