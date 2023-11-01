using AutoMapper;
using Capstone.Common.DTOs.Task;
using Capstone.Common.DTOs.Ticket;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Capstone.Service.TicketService
{
    public class TicketService : ITicketService
    {
        private readonly CapstoneContext _context;
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketStatusRepository _ticketStatusRepository;
        private readonly ITicketTypeRepository _typeRepository;
        private readonly ITicketHistoryRepository _ticketHistoryRepository;
        private readonly ITicketTypeRepository _ticketTypeRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IInterationRepository _iterationRepository;
        private readonly IStatusRepository _statusRepository;

        public TicketService(CapstoneContext context, ITicketRepository ticketRepository,
            ITicketStatusRepository ticketStatusRepository, ITicketTypeRepository typeRepository,
            ITicketHistoryRepository ticketHistoryRepository, ITicketTypeRepository ticketTypeRepository,
            IMapper mapper, IUserRepository userRepository, IInterationRepository iterationRepository,
            IStatusRepository statusRepository)
        {
            _context = context;
            _ticketRepository = ticketRepository;
            _ticketStatusRepository = ticketStatusRepository;
            _typeRepository = typeRepository;
            _ticketHistoryRepository = ticketHistoryRepository;
            this._ticketTypeRepository = ticketTypeRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _iterationRepository = iterationRepository;
            _statusRepository = statusRepository;
        }


        public async Task<bool> CreateTicket(CreateTicketRequest request, Guid interationId)
        {
            using var transaction = _iterationRepository.DatabaseTransaction();
            var listStatus = _statusRepository.GetAllAsync(x => true, null);
            try
            {
                // var ticketEntity = _mapper.Map<Ticket>(request);
                var ticketEntity = new Ticket()
                {
                    Title = request.Title,
                    Decription = request.Decription,
                    StartDate = request.StartDate,
                    DueDate = request.DueDate,
                    CreateTime = DateTime.Now,
                    CreateBy = request.CreateBy,
                    TypeId = Guid.Parse("00BD0387-BFA1-403F-AB03-4839985CB29A"),
                    PriorityId = request.PriorityId,
                    PrevId = null,
                    StatusId = request.StatusId,
                    InterationId = interationId
                };
             //   ticketEntity.InterationId = interationId;
                var newProject = await _ticketRepository.CreateAsync(ticketEntity);
                await _ticketRepository.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
                transaction.RollBack();
                return false;
            }
            // try
            // {
            //     var ticketEntity = _mapper.Map<Ticket>(request);
            //     ticketEntity.InterationId = iterationId;
            //     _context.Tickets?.Add(ticketEntity);
            //     await _context.SaveChangesAsync();
            //
            //     transaction.Commit();
            //     return true;
            // }
            // catch (Exception)
            // {
            //     transaction.RollBack();
            //     return false;
            // }
        }

        public async Task<bool> UpdateTicket(UpdateTicketRequest updateTicketRequest, Guid ticketId)
        {
            using var transaction = _iterationRepository.DatabaseTransaction();

            /* try
             {
                 var ticketEntity = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == ticketId);

                if (ticketEntity != null)
                {
                    // ticketEntity.Title = updateTicketRequest.Title;
                    // ticketEntity.Decription = updateTicketRequest.Decription;
                    // ticketEntity.StartDate = updateTicketRequest.StartDate;
                    // ticketEntity.DueDate = updateTicketRequest.DueDate;
                    // ticketEntity.AssignTo = updateTicketRequest.AssignTo;
                    // ticketEntity.TicketType = updateTicketRequest.TicketType;
                    // ticketEntity.PriorityId = updateTicketRequest.PriorityId;
                    // ticketEntity.TicketStatus = updateTicketRequest.TicketStatus;
                    // ticketEntity.InterationId = updateTicketRequest.InterationId;

                     await _context.SaveChangesAsync();

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
             }*/
            return true;
        }

        public Task<IQueryable<Ticket>> GetAllTicketAsync()
        {
            var query = _ticketRepository.GetAllAsync(x => true, null);

            return Task.FromResult(query);
        }
    }
}