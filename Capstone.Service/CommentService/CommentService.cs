using Capstone.Common.DTOs.Comments;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Service.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly ITicketCommentRepository _ticketCommentRepository;

        public CommentService(ITicketCommentRepository ticketCommentRepository)
        {
            _ticketCommentRepository = ticketCommentRepository;
        }

        public Task<GetCommentResponse> CreateComment(CreateCommentRequest comment)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteComment(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GetCommentResponse>> GetAllCommentByTaskID(Guid ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable> UpdateComment(Guid id, TicketComment updatedComment)
        {
            throw new NotImplementedException();
        }
    }
}
