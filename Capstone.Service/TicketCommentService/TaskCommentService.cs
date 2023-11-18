using AutoMapper;
using Capstone.Common.DTOs.Comments;
using Capstone.Common.DTOs.TicketComment;
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
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IStatusRepository _statusRepository;

        private readonly IMapper _mapper;

        public TaskCommentService(ITaskCommentRepository taskCommentRepository, IMapper mapper, ITaskRepository taskRepository, IUserRepository userRepository, IProjectMemberRepository projectMemberRepository,IStatusRepository statusRepository)
        {
            _taskCommentRepository = taskCommentRepository;
            _mapper = mapper;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _projectMemberRepository = projectMemberRepository;
            _statusRepository = statusRepository;
        }

        public async Task<GetCommentResponse> CreateComment(Guid byUserId, CreateCommentRequest comment)
        {
            using var transaction = _taskCommentRepository.DatabaseTransaction();
            try
            {
                var member = await _projectMemberRepository.GetAsync(x => x.UserId == byUserId, x => x.Users);
                member.Users.Status = await _statusRepository.GetAsync(x => x.StatusId == member.Users.StatusId, null);
                var newComment = new TaskComment 
                {
                    CommentId = Guid.NewGuid(),
                    Content= comment.Content,
                    CreateAt= DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                    UpdateAt= DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                    TaskId = comment.TaskId,
                    Task = await _taskRepository.GetAsync(x => x.TaskId == comment.TaskId, null),
                    CreateBy = member.MemberId,
                    ProjectMember = member,
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
            finally
            {
                transaction.Dispose();
            }
        }

        public async Task<GetCommentResponse> ReplyComment(Guid commentId, Guid byUserId, ReplyCommentRequest request)
        {
            using var transaction = _taskCommentRepository.DatabaseTransaction();
            try
            {
                var comment = await _taskCommentRepository.GetAsync(x => x.CommentId == commentId, null);
                if (comment == null) return null;
                var member = await _projectMemberRepository.GetAsync(x => x.UserId == byUserId, x => x.Users);
                member.Users.Status = await _statusRepository.GetAsync(x => x.StatusId == member.Users.StatusId, null);
                var newComment = new TaskComment
                {
                    CommentId = Guid.NewGuid(),
                    Content = request.Content,
                    CreateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                    UpdateAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                    TaskId = comment.TaskId,
                    Task = await _taskRepository.GetAsync(x => x.TaskId == comment.TaskId, null),
                    CreateBy = member.MemberId,
                    ProjectMember = member,
                };
                

                if (comment.ReplyTo == null)
                {
                    newComment.ReplyTo = commentId;
                }
                else
                {
                    newComment.ReplyTo = comment.ReplyTo;
                }

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

        public async Task<IEnumerable<GetCommentResponse>> GetAllCommentByTaskID(Guid taskId)
        {
            var listComment = await _taskCommentRepository.GetAllTaskComment(taskId);
            return listComment;
        }

        public async Task<GetCommentResponse> UpdateComment(Guid id, ReplyCommentRequest updatedComment)
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
