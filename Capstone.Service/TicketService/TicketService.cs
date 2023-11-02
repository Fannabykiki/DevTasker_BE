using AutoMapper;
using Capstone.Common.DTOs.Ticket;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;

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
            try
            {
                var ticketEntity = new Ticket()
                {
                    TicketId = Guid.NewGuid(),
                    Title = request.Title,
                    Decription = request.Decription,
                    StartDate = request.StartDate,
                    DueDate = request.DueDate,
                    CreateTime = DateTime.Now,
                    CreateBy = request.CreateBy,
                    TypeId = Guid.Parse("00BD0387-BFA1-403F-AB03-4839985CB29A"),
                    PriorityId = request.PriorityId,
                    PrevId = null,
                    StatusId = Guid.Parse("8891827D-AFAC-4A3B-8C0B-F01582B43719"),
                    InterationId = interationId,
                    AssignTo = request.AssignTo,
                    IsDelete = false
                };
                await _ticketRepository.CreateAsync(ticketEntity);
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
        }

        public async Task<bool> UpdateTicket(UpdateTicketRequest updateTicketRequest, Guid ticketId)
        {
            using var transaction = _iterationRepository.DatabaseTransaction();

            try
            {
                var ticketEntity = await _ticketRepository.GetAsync(t => t.TicketId == ticketId, null)!;

                ticketEntity.Title = updateTicketRequest.Title;
                ticketEntity.Decription = updateTicketRequest.Decription;
                ticketEntity.DueDate = updateTicketRequest.DueDate;
                ticketEntity.AssignTo = updateTicketRequest.AssignTo;
                ticketEntity.TypeId = updateTicketRequest.TypeId;
                ticketEntity.PriorityId = updateTicketRequest.PriorityId;
                ticketEntity.StatusId = updateTicketRequest.StatusId;
                await _ticketRepository.UpdateAsync(ticketEntity);
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

        public Task<IQueryable<Ticket>> GetAllTicketAsync()
        {
            var result = _ticketRepository.GetAllAsync(x => x.IsDelete != true, null);

            return Task.FromResult(result);
        }

        public Task<IQueryable<Ticket>> GetAllTicketByInterationIdAsync(Guid interationId)
        {
            var result = _ticketRepository.GetAllAsync(x => x.InterationId == interationId && x.IsDelete != true, null);

            return Task.FromResult(result);
        }

        public async Task<bool> DeleteTicket(Guid ticketId)
        {
            using var transaction = _iterationRepository.DatabaseTransaction();
            try
            {
                var selectedTicket =
                    await _ticketRepository.GetAsync(x => x.TicketId == ticketId && x.IsDelete != true, null)!;
                selectedTicket.DeleteAt = DateTime.UtcNow;
                selectedTicket.IsDelete = true;
                await _ticketRepository.UpdateAsync(selectedTicket);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                transaction.RollBack();
                return false;
            }
        }

        public async Task<bool> CreateSubTicket(CreateTicketRequest request, Guid prevId)
        {
            using var transaction = _iterationRepository.DatabaseTransaction();
            var selectedTicket =
                await _ticketRepository.GetAsync(x => x.TicketId == prevId && x.IsDelete != true, null)!;
            try
            {
                var subTicketEntity = new Ticket()
                {
                    TicketId = Guid.NewGuid(),
                    Title = request.Title,
                    Decription = request.Decription,
                    StartDate = request.StartDate,
                    DueDate = request.DueDate,
                    CreateTime = DateTime.Now,
                    CreateBy = request.CreateBy,
                    TypeId = Guid.Parse("00BD0387-BFA1-403F-AB03-4839985CB29A"),
                    PriorityId = request.PriorityId,
                    PrevId = prevId,
                    StatusId = Guid.Parse("8891827D-AFAC-4A3B-8C0B-F01582B43719"),
                    InterationId = selectedTicket.InterationId,
                    AssignTo = request.AssignTo,
                    IsDelete = false
                };
                await _ticketRepository.CreateAsync(subTicketEntity);
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
        }
    }
}