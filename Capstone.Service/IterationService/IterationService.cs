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
        private readonly ITicketRepository _ticketRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly ITicketTypeRepository _ticketTypeRepository;


        public IterationService(CapstoneContext context, IProjectRepository projectRepository, IMapper mapper, IInterationRepository iterationRepository, IBoardRepository boardRepository, ITicketRepository ticketRepository, IStatusRepository statusRepository, ITicketTypeRepository ticketTypeRepository)
        {
            _context = context;
            _projectRepository = projectRepository;
            _mapper = mapper;
            _iterationRepository = iterationRepository;
            _boardRepository = boardRepository;
            _ticketRepository = ticketRepository;
            _statusRepository = statusRepository;
            _ticketTypeRepository = ticketTypeRepository;
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

        public async Task<IEnumerable<GetInterrationByBoardIdResonse>> GetIterationsByBoardId(Guid boardId)
        {
            var iterations = await _iterationRepository.GetAllWithOdata(x => x.BoardId == boardId, null);

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
            var Tickets = await _ticketRepository.GetAllWithOdata(x => x.InterationId == iteration.InterationId, null);

            if (Tickets == null) return null;

            foreach (var ticket in Tickets)
            {
                if (ticket.PrevId == null)
                {
                    ticket.Status = await _statusRepository.GetAsync(x => x.StatusId == ticket.StatusId, null);
                    var item = new WorkItemResponse
                    {
                        TicketId = ticket.TicketId,
                        Title = ticket.Title,
                        TicketType = "Work Item",
                        TicketStatus = ticket.Status.Title
                    };

                    item.Tickets = await GetChildTicketsAsync(ticket.TicketId, Tickets);

                    workItems.Add(item);
                }
            }

            return workItems;
        }

        private async Task<List<TicketResponse>> GetChildTicketsAsync(Guid parentId, IEnumerable<Ticket> allTickets)
        {
            foreach(var ticket in allTickets)
            {
                ticket.TicketType = await _ticketTypeRepository.GetAsync(x => x.TypeId == ticket.TypeId, null);
                ticket.Status = await _statusRepository.GetAsync(x => x.StatusId == ticket.StatusId, null);
            }
            return allTickets
              .Where(x => x.PrevId == parentId)
              .Select(x => new TicketResponse
              {
                  TicketId = x.TicketId,
                  Title = x.Title,
                  TicketType = x.TicketType.Title,
                  TicketStatus = x.Status.Title
              })
              .ToList();
            return null;
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
