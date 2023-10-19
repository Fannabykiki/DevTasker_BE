using AutoMapper;
using Capstone.Common.DTOs.Task;
using Capstone.DataAccess;
using Capstone.DataAccess.Repository.Interfaces;

namespace Capstone.Service.TicketService
{
    public class TicketService : ITicketService
    {

        private readonly CapstoneContext _context;
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketStatusRepository _statusRepository;
        private readonly ITicketTypeRepository _typeRepository;
        private readonly ITicketHistoryRepository _ticketHistoryRepository;
        private readonly ITicketTypeRepository ticketTypeRepository;
        private readonly IMapper _mapper;
        private readonly IBoardRepository _boardRepository;


        public TicketService(CapstoneContext context, ITicketRepository ticketRepository, ITicketStatusRepository statusRepository, ITicketTypeRepository typeRepository, ITicketHistoryRepository ticketHistoryRepository, ITicketTypeRepository ticketTypeRepository, IMapper mapper, IBoardRepository boardRepository)
        {
            _context = context;
            _ticketRepository = ticketRepository;
            _statusRepository = statusRepository;
            _typeRepository = typeRepository;
            _ticketHistoryRepository = ticketHistoryRepository;
            this.ticketTypeRepository = ticketTypeRepository;
            _mapper = mapper;
            _boardRepository = boardRepository;
        }

        public async Task<bool> CreateTicket(CreateTicketRequest createTicketRequest)
        {
            using var transaction = _ticketRepository.DatabaseTransaction();
            try
            {
                var newTask = new DataAccess.Entities.Ticket
                {
                    TicketId = Guid.NewGuid(),
                    Title = createTicketRequest.Title,
                    Decription = createTicketRequest.Decription,
                    StartDate = createTicketRequest.StartDate,
                    DueDate = createTicketRequest.DueDate,
                    CreateTime = DateTime.Now,
                    DeleteTime = null,
                    AssignTo = createTicketRequest.AssignTo,
                    CreateBy = createTicketRequest.CreateBy,
                    TicketType = createTicketRequest.TicketType,
                    PriorityId = createTicketRequest.PriorityId,
                    PrevId = createTicketRequest.PrevId,
                    TicketStatus = createTicketRequest.TicketStatus,
                    BoardId = createTicketRequest.BoardId
                };
                await _ticketRepository.CreateAsync(newTask);
                await _context.SaveChangesAsync();

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

