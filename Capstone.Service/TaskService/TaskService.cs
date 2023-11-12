using AutoMapper;
using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Task;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.TicketService;
using Org.BouncyCastle.Math.EC.Rfc7748;
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
		private readonly IBoardStatusRepository _boardStatusRepository;
		private readonly ITicketTypeRepository _taskType;

		public TaskService(CapstoneContext context, ITicketRepository ticketRepository,
			ITicketStatusRepository ticketStatusRepository, ITicketTypeRepository typeRepository,
			ITicketHistoryRepository ticketHistoryRepository, ITicketTypeRepository ticketTypeRepository,
			IMapper mapper, IUserRepository userRepository, IInterationRepository iterationRepository,
			IStatusRepository statusRepository, IBoardStatusRepository boardStatusRepository, ITicketTypeRepository taskType)
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
			_boardStatusRepository = boardStatusRepository;
			_taskType = taskType;
		}

		public async Task<CreateTaskResponse> CreateTask(CreateTaskRequest request,Guid userId)
		{
			using var transaction = _ticketRepository.DatabaseTransaction();
			var interations = await _iterationRepository.GetAllWithOdata(x => x.BoardId == request.ProjectId, null);
			try
			{
				if (request.InterationId != Guid.Empty)
				{
					var ticketEntity = new DataAccess.Entities.Task()
					{
						TaskId = Guid.NewGuid(),
						Title = request.Title,
						Decription = request.Decription,
						StartDate = DateTime.Parse(request.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						DueDate = DateTime.Parse(request.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						CreateTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						CreateBy = userId,
						TypeId = Guid.Parse("00BD0387-BFA1-403F-AB03-4839985CB29A"),
						PriorityId = request.PriorityId,
						PrevId = null,
						InterationId = request.InterationId,
						AssignTo = request.AssignTo,
						StatusId = request.StatusId
					};
					var newTask =await _ticketRepository.CreateAsync(ticketEntity);
					await _ticketRepository.SaveChanges();
					var status = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == newTask.StatusId, null);
					return new CreateTaskResponse
					{
						AssignTo = newTask.AssignTo,
						CreateBy = userId,
						CreateTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						Decription = newTask.Decription,
						DeleteAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						DueDate = DateTime.Parse(newTask.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						TaskId = newTask.TaskId,
						InterationId = newTask.InterationId,
						IsDelete = newTask.IsDelete,
						BaseResponse = new BaseResponse
						{
							IsSucceed = true,
							Message = "Create Successfully"
						},
						PrevId = newTask.PrevId,
						PriorityId = newTask.PriorityId,
						TypeId = newTask.TypeId,
						StartDate = DateTime.Parse(newTask.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						Title = newTask.Title,
						Status = status.Title
					};
				}
				else
				{
					foreach (var interation in interations)
					{
						if (interation.StartDate <= DateTime.Now && DateTime.Now <= interation.EndDate)
						{
							var ticketEntity = new DataAccess.Entities.Task()
							{
								TaskId = Guid.NewGuid(),
								Title = request.Title,
								Decription = request.Decription,
								StartDate = DateTime.Parse(request.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								DueDate = DateTime.Parse(request.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								CreateTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								CreateBy = userId,
								TypeId = Guid.Parse("00BD0387-BFA1-403F-AB03-4839985CB29A"),
								PriorityId = request.PriorityId,
								PrevId = null,
								InterationId = interation.InterationId,
								AssignTo = request.AssignTo,
								StatusId = request.StatusId
							};
							var newTask = await _ticketRepository.CreateAsync(ticketEntity);
							await _ticketRepository.SaveChanges();
							var status = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == newTask.StatusId, null);
							return new CreateTaskResponse
							{
								AssignTo = newTask.AssignTo,
								CreateBy = userId,
								CreateTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								Decription = newTask.Decription,
								DeleteAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								DueDate = DateTime.Parse(newTask.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								TaskId = newTask.TaskId,
								InterationId= newTask.InterationId,
								IsDelete = newTask.IsDelete,
								PrevId = newTask.PrevId,
								PriorityId = newTask.PriorityId,
								TypeId = newTask.TypeId,
								StartDate = DateTime.Parse(newTask.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								Title = newTask.Title,
								Status = status.Title,
								BaseResponse = new BaseResponse
								{
									IsSucceed = true,
									Message = "Create Successfully"
								}
							};
						}
					}
				}
				transaction.Commit();

				return new CreateTaskResponse
				{
					BaseResponse = new BaseResponse
					{
						IsSucceed = true,
						Message = "Create fail"
					}
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error occurred: " + ex.Message);
				transaction.RollBack();
				return new CreateTaskResponse
				{
					BaseResponse = new BaseResponse
					{
						IsSucceed = false,
						Message = "Create fail"
					}
				};
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


		public async Task<List<TaskViewModel>> GetAllTaskAsync(Guid projectId)
		{
			var result = await _ticketRepository.GetAllTask(projectId);
			return result;
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

		public async Task<List<StatusTaskViewModel>> GetAllTaskStatus(Guid projectId)
		{
			var result = await _boardStatusRepository.GetAllWithOdata(x => x.BoardId == projectId, null);
			return _mapper.Map<List<StatusTaskViewModel>>(result);
		}

		public async Task<List<TaskTypeViewModel>> GetAllTaskType()
		{
			var result = await _typeRepository.GetAllWithOdata(x => true, null);
			return _mapper.Map<List<TaskTypeViewModel>>(result);
		}

		public async Task<StatusTaskViewModel> CreateTaskStatus(CreateNewTaskStatus createNewTaskStatus)
		{
			using var transaction = _boardStatusRepository.DatabaseTransaction();
			try
			{
				var newStatus = new BoardStatus
				{
					BoardId = createNewTaskStatus.ProjectId,
					BoardStatusId = Guid.NewGuid(),
					Title = createNewTaskStatus.Title
				};
				var status = await _boardStatusRepository.CreateAsync(newStatus);
				await _boardStatusRepository.SaveChanges();
				transaction.Commit();

				return new StatusTaskViewModel
				{
					BoardId = status.BoardId,
					BoardStatusId = status.BoardStatusId,
					Title= status.Title,
					BaseResponse = new BaseResponse
					{	
						IsSucceed = true,
						Message = "Create successfully"
					}
				};
			}
			catch (Exception ex)
			{
				transaction.RollBack();
				return new StatusTaskViewModel
				{
					BaseResponse = new BaseResponse
					{
						IsSucceed = false,
						Message = "Create fail"
					}
				};
			}
		}

		public async Task<CreateTaskResponse> CreateSubTask(CreateTaskRequest request, Guid userId)
		{
			using var transaction = _ticketRepository.DatabaseTransaction();
			var interations = await _iterationRepository.GetAllWithOdata(x => x.BoardId == request.ProjectId, null);
			try
			{
				if (request.InterationId != Guid.Empty)
				{
					var ticketEntity = new DataAccess.Entities.Task()
					{
						TaskId = Guid.NewGuid(),
						Title = request.Title,
						Decription = request.Decription,
						StartDate = DateTime.Parse(request.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						DueDate = DateTime.Parse(request.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						CreateTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						CreateBy = userId,
						TypeId = Guid.Parse("00BD0387-BFA1-403F-AB03-4839985CB29A"),
						PriorityId = request.PriorityId,
						PrevId = request.PrevId,
						InterationId = request.InterationId,
						AssignTo = request.AssignTo,
						StatusId = request.StatusId
					};
					var newTask = await _ticketRepository.CreateAsync(ticketEntity);
					await _ticketRepository.SaveChanges();
					transaction.Commit();
					var status = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == newTask.StatusId, null);
					return new CreateTaskResponse
					{
						AssignTo = newTask.AssignTo,
						CreateBy = userId,
						CreateTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						Decription = newTask.Decription,
						DeleteAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						DueDate = DateTime.Parse(newTask.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						TaskId = newTask.TaskId,
						InterationId = newTask.InterationId,
						IsDelete = newTask.IsDelete,
						PrevId = newTask.PrevId,
						PriorityId = newTask.PriorityId,
						TypeId = newTask.TypeId,
						StartDate = DateTime.Parse(newTask.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						Title = newTask.Title,
						Status = status.Title,
						BaseResponse = new BaseResponse
						{
							IsSucceed = true,
							Message = "Create Successfully"
						}
					};
				}
				else
				{
					foreach (var interation in interations)
					{
						if (interation.StartDate <= DateTime.Now && DateTime.Now <= interation.EndDate)
						{
							var ticketEntity = new DataAccess.Entities.Task()
							{
								TaskId = Guid.NewGuid(),
								Title = request.Title,
								Decription = request.Decription,
								StartDate = DateTime.Parse(request.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								DueDate = DateTime.Parse(request.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								CreateTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								CreateBy = userId,
								TypeId = Guid.Parse("00BD0387-BFA1-403F-AB03-4839985CB29A"),
								PriorityId = request.PriorityId,
								PrevId = request.PrevId,
								InterationId = interation.InterationId,
								AssignTo = request.AssignTo,
								StatusId = request.StatusId
							};
							var newTask = await _ticketRepository.CreateAsync(ticketEntity);
							await _ticketRepository.SaveChanges();
							transaction.Commit();
							var status = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == newTask.StatusId, null);
							return new CreateTaskResponse
							{
								AssignTo = newTask.AssignTo,
								CreateBy = userId,
								CreateTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								Decription = newTask.Decription,
								DeleteAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								DueDate = DateTime.Parse(newTask.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								TaskId = newTask.TaskId,
								InterationId = newTask.InterationId,
								IsDelete = newTask.IsDelete,
								PrevId = newTask.PrevId,
								PriorityId = newTask.PriorityId,
								TypeId = newTask.TypeId,
								StartDate = DateTime.Parse(newTask.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
								Title = newTask.Title,
								Status = status.Title,
								BaseResponse = new BaseResponse
								{
									IsSucceed = true,
									Message = "Create Successfully"
								}
							};
							
						}
					}
				}
			

				return new CreateTaskResponse
				{
					BaseResponse = new BaseResponse
					{
						IsSucceed = true,
						Message = "Create fail"
					}
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error occurred: " + ex.Message);
				transaction.RollBack();
				return new CreateTaskResponse
				{
					BaseResponse = new BaseResponse
					{
						IsSucceed = false,
						Message = "Create fail"
					}
				};
			}
		}
	}
}