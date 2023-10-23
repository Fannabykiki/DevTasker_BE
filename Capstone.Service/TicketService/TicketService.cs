using AutoMapper;
using Capstone.Common.DTOs.Task;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Org.BouncyCastle.Asn1.Ocsp;

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
        private readonly IUserRepository _userRepository;

        public TicketService(CapstoneContext context, ITicketRepository ticketRepository, ITicketStatusRepository statusRepository, ITicketTypeRepository typeRepository, ITicketHistoryRepository ticketHistoryRepository, ITicketTypeRepository ticketTypeRepository, IMapper mapper, IBoardRepository boardRepository, IUserRepository userRepository)
        {
            _context = context;
            _ticketRepository = ticketRepository;
            _statusRepository = statusRepository;
            _typeRepository = typeRepository;
            _ticketHistoryRepository = ticketHistoryRepository;
            this.ticketTypeRepository = ticketTypeRepository;
            _mapper = mapper;
            _boardRepository = boardRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> CreateTicket(CreateTicketRequest request, Guid boardId)
        {
            using var transaction = _boardRepository.DatabaseTransaction();

            try
            {
                var board = await _boardRepository.GetAsync(x => x.BoardId == boardId, null);

                
                var assignToUser = await _userRepository.GetAsync(x => x.UserId == request.AssignTo, null);
                if (assignToUser == null)
                {
                    throw new ArgumentException($"User with ID {request.AssignTo} does not exist", nameof(request.AssignTo));
                }

                var ticket = _mapper.Map<CreateTicketRequest, Ticket>(request);

                ticket.BoardId = boardId;

                await _ticketRepository.UpdateAsync(ticket);
                await _context.SaveChangesAsync();

                transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                transaction.RollBack();
                return false;
            }
        }



    }
}

