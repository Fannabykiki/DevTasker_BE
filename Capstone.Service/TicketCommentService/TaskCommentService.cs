using AutoMapper;
using Capstone.Common.DTOs.Comments;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Google.Apis.Drive.v3.Data;

namespace Capstone.Service.TicketCommentService
{
    public class TaskCommentService : ITaskCommentService
    {
        private readonly ITaskCommentRepository _taskCommentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public TaskCommentService(ITaskCommentRepository taskCommentRepository, IMapper mapper, ITaskRepository taskRepository, IUserRepository userRepository)
        {
            _taskCommentRepository = taskCommentRepository;
            _mapper = mapper;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async Task<GetCommentResponse> CreateComment(CreateCommentRequest comment)
        {
            using var transaction = _taskCommentRepository.DatabaseTransaction();
            try
            {
                var newComment = new TaskComment 
                {
                    CommentId = Guid.NewGuid(),
                    Content= comment.Content,
                    CreateAt= DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                    UpdateAt= DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                    TaskId = comment.TaskId,
                    Task = await _taskRepository.GetAsync(x =>x.TaskId == comment.TaskId,null),
                    CreateBy = comment.ByUser,
                    User = await _userRepository.GetAsync(x => x.UserId == comment.ByUser,x => x.Status),
                };

                await _taskCommentRepository.CreateAsync(newComment);
                await _taskCommentRepository.SaveChanges();

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
            using var transaction = _taskCommentRepository.DatabaseTransaction();
            try
            {
                var commentUpdate = await _taskCommentRepository.GetAsync(x => x.CommentId == id, null);
                if (commentUpdate == null) return false;
                commentUpdate.UpdateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
                commentUpdate.DeleteAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));

                await _taskCommentRepository.UpdateAsync(commentUpdate);
                await _taskCommentRepository.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
                transaction.RollBack();
                return false;
            }
        }

        public async Task<IEnumerable<GetCommentResponse>> GetAllCommentByTaskID(Guid ticketId)
        {
            var listComment = await _taskCommentRepository.GetAllWithOdata(x => x.TaskId == ticketId && x.DeleteAt == null, null);
            return _mapper.Map<List<GetCommentResponse>>(listComment);
        }

        public async Task<GetCommentResponse> UpdateComment(Guid id, CreateCommentRequest updatedComment)
        {
            var commentUpdate = await _taskCommentRepository.GetAsync(x => x.CommentId == id && x.DeleteAt == null,null);
            if (commentUpdate == null) return null;
            commentUpdate.UpdateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            commentUpdate.Content= updatedComment.Content;

            await _taskCommentRepository.UpdateAsync(commentUpdate);
            await _taskCommentRepository.SaveChanges();

            return _mapper.Map<GetCommentResponse>(commentUpdate);
        }
    }
}
