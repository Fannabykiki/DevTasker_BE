using AutoMapper;
using Capstone.Common.DTOs.Task;
using Capstone.DataAccess;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.TicketService;
using Task = Capstone.DataAccess.Entities.Task;

namespace Capstone.Service.TaskService
{
	public class TaskService : ITaskService
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

		public TaskService(CapstoneContext context, ITicketRepository ticketRepository,
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

		public async Task<bool> CreateTask(CreateTaskRequest request, Guid interationId, Guid userId)
		{
			using var transaction = _iterationRepository.DatabaseTransaction();
			var listStatus = _statusRepository.GetAllAsync(x => true, null);
			try
			{
				var ticketEntity = new DataAccess.Entities.Task()
				{
					TaskId = Guid.NewGuid(),
					Title = request.Title,
					Decription = request.Decription,
					StartDate = request.StartDate,
					DueDate = request.DueDate,
					CreateTime = DateTime.Now,
					CreateBy = userId,
					TypeId = Guid.Parse("00BD0387-BFA1-403F-AB03-4839985CB29A"),
					PriorityId = request.PriorityId,
					PrevId = null,
					// StatusId = Guid.Parse("8891827D-AFAC-4A3B-8C0B-F01582B43719"),
					InterationId = interationId,
					AssignTo = request.AssignTo
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

		public async Task<bool> UpdateTask(UpdateTaskRequest updateTicketRequest, Guid ticketId)
		{
            using var transaction = _iterationRepository.DatabaseTransaction();
            var listStatus = _statusRepository.GetAllAsync(x => true, null);
            try
            {
                var ticketEntity = new DataAccess.Entities.Task()
                {
                    TaskId = Guid.NewGuid(),
                    Title = updateTicketRequest.Title,
                    Decription = updateTicketRequest.Decription,
                    DueDate = updateTicketRequest.DueDate,
                    CreateTime = DateTime.Now,
                    TypeId = Guid.Parse("00BD0387-BFA1-403F-AB03-4839985CB29A"),
                    PriorityId = updateTicketRequest.PriorityId,
                    PrevId = null,
                    // StatusId = Guid.Parse("8891827D-AFAC-4A3B-8C0B-F01582B43719"),
                    AssignTo = updateTicketRequest.AssignTo
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


		public Task<IQueryable<Task>> GetAllTaskAsync()
		{
			var result = _ticketRepository.GetAllAsync(x => x.IsDelete != true, null);

			return System.Threading.Tasks.Task.FromResult<IQueryable<Task>>(result);
		}

		public Task<IQueryable<Task>> GetAllTaskByInterationIdAsync(Guid interationId)
		{
			var result = _ticketRepository.GetAllAsync(x => x.InterationId == interationId && x.IsDelete == false, null);

			return System.Threading.Tasks.Task.FromResult<IQueryable<Task>>(result);
		}

		public async Task<bool> DeleteTask(Guid ticketId)
		{
			using var transaction = _iterationRepository.DatabaseTransaction();
			try
			{
				var selectedTicket = await _ticketRepository.GetAsync(x => x.TaskId == ticketId, null)!;
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
	}
}