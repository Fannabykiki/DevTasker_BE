using AutoMapper;
using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.User;
using Capstone.Common.Enums;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Capstone.Service.IterationService
{

    public class IterationService : IIterationService
    {

        private readonly CapstoneContext _context;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;
        private readonly IInterationRepository _iterationRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly ITaskRepository _TaskRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly ITaskTypeRepository _TaskTypeRepository;


        public IterationService(CapstoneContext context, IProjectRepository projectRepository, IMapper mapper, IInterationRepository iterationRepository, IBoardRepository boardRepository, ITaskRepository TaskRepository, IStatusRepository statusRepository, ITaskTypeRepository TaskTypeRepository, IUserRepository userRepository)
        {
            _context = context;
            _projectRepository = projectRepository;
            _mapper = mapper;
            _iterationRepository = iterationRepository;
            _boardRepository = boardRepository;
            _TaskRepository = TaskRepository;
            _statusRepository = statusRepository;
            _TaskTypeRepository = TaskTypeRepository;
        }
        public async Task<IEnumerable<GetInterrationByBoardIdResonse>> GetIterationsById(Guid iterationId)
        {
            var iteration = await _iterationRepository.GetAsync(x => x.InterationId == iterationId, x => x.Board)!;

            iteration.Status = await _statusRepository.GetAsync(x => x.StatusId == iteration.StatusId, null);
            var response = new GetInterrationByBoardIdResonse
            {
                InterationId = iteration.InterationId,
                InterationName = iteration.InterationName,
                StartDate = iteration.StartDate,
                EndDate = iteration.EndDate,
                BoardId = iteration.BoardId,
                Status = iteration.Status.Title
            };
            response.Tasks = await _TaskRepository.GetAllTask(iteration.BoardId);

            return new List<GetInterrationByBoardIdResonse> { response };
        }

        public async Task<IEnumerable<GetInterrationByBoardIdResonse>> GetIterationTasksByProjectId(Guid projectId)
        {
            var iterations = await _iterationRepository.GetAllWithOdata(x => x.BoardId == projectId, null);

            var result = new List<GetInterrationByBoardIdResonse>();

            foreach (var iteration in iterations)
            {
                iteration.Status = await _statusRepository.GetAsync(x => x.StatusId == iteration.StatusId, null);
                var response = new GetInterrationByBoardIdResonse
                {
                    InterationId = iteration.InterationId,
                    InterationName = iteration.InterationName,
                    Status = iteration.Status.Title,
                    StartDate = iteration.StartDate,
                    EndDate = iteration.EndDate,
                    BoardId = iteration.BoardId,
                };

                response.Tasks = await _TaskRepository.GetAllTask(projectId);
                result.Add(response);
            }

            return result;
        }

        //private async Task<List<Tasks>> GetWorkItemsForIterationAsync(Interation iteration)
        //{
        //    var workItems = new List<Tasks>();
        //    var Tasks = await _TaskRepository.GetAllWithOdata(x => x.InterationId == iteration.InterationId, x => x.Status);

        //    if (Tasks == null) return null;

        //    foreach (var Task in Tasks)
        //    {
        //        if (Task.PrevId == null)
        //        {
        //            var item = new Tasks
        //            {
        //                TaskId = Task.TaskId,
        //                Title = Task.Title,
        //                Description = Task.Description,
        //                StartDate = Task.StartDate,
        //                DueDate = Task.DueDate,
        //                CreateTime = Task.CreateTime,
        //                DeleteAt = Task.DeleteAt,
        //                IsDelete = Task.IsDelete,
        //                AssignTo = _mapper.Map<UserResponse>(await _userRepository.GetAsync(x =>x.UserId == Task.AssignTo,null)),

        //                TaskType = "Work Item",
        //                StatusId= Task.StatusId,
        //                TaskStatus = Task.Status.Title
        //            };

        //            item.SubTasks = await GetChildTasksAsync(Task.TaskId, Tasks);

        //            workItems.Add(item);
        //        }
        //    }

        //    return workItems;
        //}

        //private async Task<List<TaskResponse>> GetChildTasksAsync(Guid parentId, IEnumerable<DataAccess.Entities.Task> allTasks)
        //{

        //    foreach (var Task in allTasks)
        //    {
        //        Task.TicketType = await _TaskTypeRepository.GetAsync(x => x.TypeId == Task.TypeId, null);
        //    }
        //    return allTasks
        //      .Where(x => x.PrevId == parentId)
        //      .Select(x => new TaskResponse
        //      {
        //          TaskId = x.TaskId,
        //          Title = x.Title,
        //          StatusId = x.StatusId,
        //          TaskType = x.TicketType.Title,
        //          TaskStatus = x.Status.Title
        //      })
        //      .ToList();
            
        //}

        public async Task<GetIntergrationResponse> CreateInteration(CreateIterationRequest createIterationRequest)
        {
            using var transaction = _iterationRepository.DatabaseTransaction();

            try
            {
                var newIterationRequest = new Interation
                {
					InterationName = createIterationRequest.InterationName,
                    StartDate = DateTime.Parse(createIterationRequest.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                    EndDate = DateTime.Parse(createIterationRequest.EndDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
                    BoardId = createIterationRequest.ProjectId,
                    StatusId = Guid.Parse("093416CB-1A26-43A4-9E11-DBDF5166DF4A"),
                };

                var newIteration = await _iterationRepository.CreateAsync(newIterationRequest);
                var board = await _boardRepository.GetAsync(x => x.BoardId == createIterationRequest.ProjectId, null);
                board.Interations.Add(newIteration);

                await _boardRepository.UpdateAsync(board);
                await _iterationRepository.SaveChanges();
                await _boardRepository.SaveChanges();

                transaction.Commit();
                return new GetIntergrationResponse
                {
                    BoardId = newIteration.BoardId,
                    EndDate = newIteration.EndDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    StartDate = newIteration.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
                    InterationId = newIteration.InterationId,
                    InterationName = newIteration.InterationName,
                    StatusId = newIteration.StatusId,
                    Response = new Common.DTOs.Base.BaseResponse
                    {
                        IsSucceed = true,
                        Message = "Create scucessfully"
                    },
				};
            }
            catch (Exception)
            {
                transaction.RollBack();
				return new GetIntergrationResponse
				{
					Response = new Common.DTOs.Base.BaseResponse
					{
						IsSucceed = false,
						Message = "Create fail"
					},
				};
			}
        }
        public async Task<bool> UpdateIterationRequest(UpdateIterationRequest updateIterationRequest, Guid iterationId)
        {

            using var transaction = _iterationRepository.DatabaseTransaction();

            try
            {
                var iteration = await _iterationRepository.GetAsync(x => x.InterationId == iterationId, null)!;

                iteration.InterationName = updateIterationRequest.InterationName;
                iteration.StartDate = updateIterationRequest.StartDate;
                iteration.EndDate = updateIterationRequest.EndDate;
                iteration.Status = await _statusRepository.GetAsync(x => x.StatusId == updateIterationRequest.StatusId, null);


                await _iterationRepository.UpdateAsync(iteration);

                await _iterationRepository.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.RollBack();
                return false;
            }
        }

        

    }
}
