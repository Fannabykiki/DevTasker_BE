using AutoMapper;
using Capstone.Common.DTOs.Comments;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.Service.TicketCommentService
{
    public class TicketCommentService : ITicketCommentService
    {
        private readonly ITicketCommentRepository _ticketCommentRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public TicketCommentService(ITicketCommentRepository ticketCommentRepository, IMapper mapper, ITicketRepository ticketRepository, IUserRepository userRepository)
        {
            _ticketCommentRepository = ticketCommentRepository;
            _mapper = mapper;
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
        }

        public async Task<GetCommentResponse> CreateComment(CreateCommentRequest comment)
        {
            using var transaction = _ticketCommentRepository.DatabaseTransaction();
            try
            {
                var newComment = new TaskComment 
                {
                    CommentId = Guid.NewGuid(),
                    Content= comment.Content,
                    CreateAt= DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                    UpdateAt= DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                    TaskId = comment.TaskId,
                    Task = await _ticketRepository.GetAsync(x =>x.TaskId == comment.TaskId,null),
                    CreateBy = comment.ByUser,
                    User = await _userRepository.GetAsync(x => x.UserId == comment.ByUser,null),
                };

                await _ticketCommentRepository.CreateAsync(newComment);
                await _ticketCommentRepository.SaveChanges();

                transaction.Commit();
                return _mapper.Map<GetCommentResponse>(newComment);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
                transaction.RollBack();
                return null;
            }
        }

        public async Task<bool> RemoveComment(Guid id)
        {
            var commentUpdate = await _ticketCommentRepository.GetAsync(x => x.CommentId == id, null);
            if (commentUpdate == null) return false;
            commentUpdate.UpdateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            commentUpdate.DeleteAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));

            _ticketCommentRepository.UpdateAsync(commentUpdate);
            _ticketCommentRepository.SaveChanges();

            return true;
        }

        public async Task<IEnumerable<GetCommentResponse>> GetAllCommentByTaskID(Guid ticketId)
        {
            var listComment = await _ticketCommentRepository.GetAllWithOdata(x => x.TaskId == ticketId && x.DeleteAt == null, null);
            return _mapper.Map<List<GetCommentResponse>>(listComment);
        }

        public async Task<GetCommentResponse> UpdateComment(Guid id, CreateCommentRequest updatedComment)
        {
            var commentUpdate = await _ticketCommentRepository.GetAsync(x => x.CommentId == id && x.DeleteAt == null,null);
            if (commentUpdate == null) return null;
            commentUpdate.UpdateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            commentUpdate.Content= updatedComment.Content;

            _ticketCommentRepository.UpdateAsync(commentUpdate);
            _ticketCommentRepository.SaveChanges();

            return _mapper.Map<GetCommentResponse>(commentUpdate);
        }
    }
}
