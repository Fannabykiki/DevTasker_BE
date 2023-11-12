using AutoMapper;
using Capstone.Common.DTOs.Iteration;
using Capstone.Common.Enums;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

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


        public IterationService(CapstoneContext context, IProjectRepository projectRepository, IMapper mapper, IInterationRepository iterationRepository, IBoardRepository boardRepository, ITaskRepository TaskRepository, IStatusRepository statusRepository, ITaskTypeRepository TaskTypeRepository)
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
            var iteration = await _iterationRepository.GetAsync(x => x.InterationId == iterationId, null)!;

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
            response.workItemResponses = await GetWorkItemsForIterationAsync(iteration);

            return new List<GetInterrationByBoardIdResonse> { response };
        }

        public async Task<IEnumerable<GetInterrationByBoardIdResonse>> GetIterationTasksByProjectId(Guid projectId)
        {
            var board = await _boardRepository.GetAsync(x => x.ProjectId == projectId, null);
            if (board == null) return null;
            var iterations = await _iterationRepository.GetAllWithOdata(x => x.BoardId == board.BoardId, null);

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

                response.workItemResponses = await GetWorkItemsForIterationAsync(iteration);
                result.Add(response);
            }

            return result;
        }

        private async Task<List<WorkItemResponse>> GetWorkItemsForIterationAsync(Interation iteration)
        {
            var workItems = new List<WorkItemResponse>();
            var Tasks = await _TaskRepository.GetAllWithOdata(x => x.InterationId == iteration.InterationId, x => x.Status);

            if (Tasks == null) return null;

            foreach (var Task in Tasks)
            {
                if (Task.PrevId == null)
                {
                    var item = new WorkItemResponse
                    {
                        TaskId = Task.TaskId,
                        Title = Task.Title,
                        TaskType = "Work Item",
                        StatusId= Task.StatusId,
                        TaskStatus = Task.Status.Title
                    };

                    item.Tasks = await GetChildTasksAsync(Task.TaskId, Tasks);

                    workItems.Add(item);
                }
            }

            return workItems;
        }

        private async Task<List<TaskResponse>> GetChildTasksAsync(Guid parentId, IEnumerable<DataAccess.Entities.Task> allTasks)
        {

            foreach (var Task in allTasks)
            {
                Task.TicketType = await _TaskTypeRepository.GetAsync(x => x.TypeId == Task.TypeId, null);
            }
            return allTasks
              .Where(x => x.PrevId == parentId)
              .Select(x => new TaskResponse
              {
                  TaskId = x.TaskId,
                  Title = x.Title,
                  StatusId = x.StatusId,
                  TaskType = x.TicketType.Title,
                  TaskStatus = x.Status.Title
              })
              .ToList();
            
        }

        public async Task<bool> CreateInteration(CreateIterationRequest createIterationRequest, Guid boarId)
        {
            using var transaction = _iterationRepository.DatabaseTransaction();

            try
            {
                var newIterationRequest = new Interation
                {
                    InterationName = createIterationRequest.InterationName,
                    StartDate = createIterationRequest.StartDate,
                    EndDate = createIterationRequest.EndDate,
                    BoardId = boarId,
                   StatusId = Guid.Parse("093416CB-1A26-43A4-9E11-DBDF5166DF4A")
                };


                var newIteration = await _iterationRepository.CreateAsync(newIterationRequest);
                var board = await _boardRepository.GetAsync(x => x.BoardId == boarId, null);
                board.Interations.Add(newIteration);
                await _boardRepository.UpdateAsync(board);

                _iterationRepository.SaveChanges();
                _boardRepository.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.RollBack();
                return false;
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
