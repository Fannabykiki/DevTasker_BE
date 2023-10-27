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
        private readonly IUserRepository _userRepository;
        private readonly IInterationRepository _iterationRepository;

        public TicketService(CapstoneContext context, ITicketRepository ticketRepository, ITicketStatusRepository statusRepository, ITicketTypeRepository typeRepository, ITicketHistoryRepository ticketHistoryRepository, ITicketTypeRepository ticketTypeRepository, IMapper mapper, IUserRepository userRepository, IInterationRepository iterationRepository)
        {
            _context = context;
            _ticketRepository = ticketRepository;
            _statusRepository = statusRepository;
            _typeRepository = typeRepository;
            _ticketHistoryRepository = ticketHistoryRepository;
            this.ticketTypeRepository = ticketTypeRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _iterationRepository = iterationRepository;
        }
        public async Task<bool> CreateTicket(CreateTicketRequest request, Guid iterationId)
        {
            using var transaction = _iterationRepository.DatabaseTransaction();

            try
            {
                var ticketEntity = _mapper.Map<Ticket>(request);
                ticketEntity.InterationId = iterationId;
                _context.Tickets.Add(ticketEntity);
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

