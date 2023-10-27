using AutoMapper;
using Capstone.Common.DTOs.Iteration;
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
        private readonly ITicketRepository _ticketRepository;


        public IterationService(CapstoneContext context, IProjectRepository projectRepository, IMapper mapper, IInterationRepository iterationRepository, IBoardRepository boardRepository, ITicketRepository ticketRepository)
        {
            _context = context;
            _projectRepository = projectRepository;
            _mapper = mapper;
            _iterationRepository = iterationRepository;
            _boardRepository = boardRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<List<GetAllInterrationByProjectIdResonse>> GetIterationsByProjectId(Guid boardId)
        {
            /*var interations = await _iterationRepository.GetAllWithOdata(x => x.BoardId == boardId, null);
            foreach (var interation in interations)
            {
                if (interation.Status == InterationStatusEnum.Current)
                {
                    var listWorkItem = new List<WorkItemResponse>();
                    foreach (var board in interation.tick)
                    {
                        var workItems = board.Tickets.Where(x => x.PrevId == null);

                        var listTaskInWorkItem = new List<TicketResponse>();
                        foreach (var workItem in workItems)
                        {
                            var taskInWorkItems = board.Tickets.Where(x => x.PrevId == workItem.TicketId);
                        }
                    }
                }
            }

            var iterations = await _iterationRepository.GetAllWithOdata(x => x.ProjectId == projectId, null);

            // Map iterations to response model
            var response = _mapper.Map<List<GetAllInterrationByProjectIdResonse>>(interations);*/
            return null;
        }

        public async Task<bool> CreateIteration(CreateIterationRequest createIterationRequest, Guid boarId)
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
                   Status = createIterationRequest.Status
                };

               
                var newIteration = await _iterationRepository.CreateAsync(newIterationRequest);
                var board = await _boardRepository.GetAsync(x => x.BoardId == boarId, null);
                board.Interations.Add(newIteration);
                await _boardRepository.UpdateAsync(board);

                _iterationRepository.SaveChanges();
                _projectRepository.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.RollBack();
                return false;
            }
        }
        public async Task<bool> UpdateIterationRequest(UpdateIterationRequest updateIterationRequest, Guid boardId)
        {

            using var transaction = _iterationRepository.DatabaseTransaction();

            try
            {
                var iteration = await _iterationRepository.GetAsync(x => x.InterationId == boardId, null);

                if (iteration != null)
                {
                    iteration.InterationName = updateIterationRequest.InterationName;
                    iteration.StartDate = updateIterationRequest.StartDate;
                    iteration.EndDate = updateIterationRequest.EndDate;
                    iteration.Status = updateIterationRequest.Status;


                    await _iterationRepository.UpdateAsync(iteration);

                    _iterationRepository.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                else
                {
                    transaction.RollBack();
                    return false;
                }
            }
            catch (Exception)
            {
                transaction.RollBack();
                return false;
            }
        }
    }
}
