using AutoMapper;
using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Task;
using Capstone.Common.DTOs.TaskPriority;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.TicketService;
using Google.Apis.Drive.v3.Data;
using Org.BouncyCastle.Math.EC.Rfc7748;
using Task = Capstone.DataAccess.Entities.Task;

namespace Capstone.Service.TaskService
{
	public class TaskService : ITaskService
	{
		private readonly CapstoneContext _context;
		private readonly ITaskRepository _ticketRepository;
		private readonly ITicketStatusRepository _ticketStatusRepository;
		private readonly ITaskTypeRepository _typeRepository;
		private readonly ITicketHistoryRepository _ticketHistoryRepository;
		private readonly ITaskTypeRepository _ticketTypeRepository;
		private readonly IMapper _mapper;
		private readonly IUserRepository _userRepository;
		private readonly IInterationRepository _iterationRepository;
		private readonly IStatusRepository _statusRepository;
		private readonly IBoardStatusRepository _boardStatusRepository;
		private readonly ITaskTypeRepository _taskType;
		private readonly IPriorityRepository _priorityRepository;


		public TaskService(CapstoneContext context, ITaskRepository ticketRepository,
			ITicketStatusRepository ticketStatusRepository, ITaskTypeRepository typeRepository,
			ITicketHistoryRepository ticketHistoryRepository, ITaskTypeRepository ticketTypeRepository,
			IMapper mapper, IUserRepository userRepository, IInterationRepository iterationRepository,
			IStatusRepository statusRepository, IBoardStatusRepository boardStatusRepository, ITaskTypeRepository taskType, IPriorityRepository priorityRepository)
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
			_priorityRepository = priorityRepository;
		}

		public async Task<CreateTaskResponse> CreateTask(CreateTaskRequest request, Guid userId)
		{
			using var transaction = _ticketRepository.DatabaseTransaction();
			var interations = await _iterationRepository.GetAllWithOdata(x => x.BoardId == request.ProjectId, null);
			try
			{	
				if(request.TypeId == Guid.Empty)
				{
					if (request.InterationId != Guid.Empty)
					{
						var ticketEntity = new DataAccess.Entities.Task()
						{
							TaskId = Guid.NewGuid(),
							Title = request.Title,
							Description = request.Decription,
							StartDate = DateTime.Parse(request.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
							DueDate = DateTime.Parse(request.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
							CreateTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
							CreateBy = userId,
							TypeId = Guid.Parse("00BD0387-BFA1-403F-AB03-4839985CB29A"),
							PriorityId = request.PriorityId,
							PrevId = null,
							IsDelete = false,
							DeleteAt = null,
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
							CreateTime = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							Description = newTask.Description,
							DeleteAt = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							DueDate = newTask.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							TaskId = newTask.TaskId,
							InterationId = newTask.InterationId,
							IsDelete = newTask.IsDelete,
							PriorityId = newTask.PriorityId,
							TypeId = newTask.TypeId,
							StartDate = newTask.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							Title = newTask.Title,
							Status = status.Title,
							BaseResponse = new BaseResponse
							{
								IsSucceed = true,
								Message = "Create Successfully"
							},
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
									Description = request.Decription,
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
								transaction.Commit();

								var status = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == newTask.StatusId, null)!;
								return new CreateTaskResponse
								{
									AssignTo = newTask.AssignTo,
									CreateBy = userId,
									CreateTime = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Description = newTask.Description,
									DeleteAt = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									DueDate = newTask.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									TaskId = newTask.TaskId,
									InterationId = newTask.InterationId,
									IsDelete = newTask.IsDelete,
									PriorityId = newTask.PriorityId,
									TypeId = newTask.TypeId,
									StartDate = newTask.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Title = newTask.Title,
									Status = status.Title,
									StatusId = newTask.StatusId,
									BaseResponse = new BaseResponse
									{
										IsSucceed = true,
										Message = "Create Successfully"
									},
								};
							}
						}
					}
				}
				else
				{
					if (request.InterationId != Guid.Empty)
					{
						var ticketEntity = new DataAccess.Entities.Task()
						{
							TaskId = Guid.NewGuid(),
							Title = request.Title,
							Description = request.Decription,
							StartDate = DateTime.Parse(request.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
							DueDate = DateTime.Parse(request.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
							CreateTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
							CreateBy = userId,
							TypeId = request.TypeId,
							PriorityId = request.PriorityId,
							PrevId = null,
							IsDelete = false,
							DeleteAt = null,
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
							CreateTime = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							Description = newTask.Description,
							DeleteAt = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							DueDate = newTask.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							TaskId = newTask.TaskId,
							InterationId = newTask.InterationId,
							IsDelete = newTask.IsDelete,
							PriorityId = newTask.PriorityId,
							TypeId = newTask.TypeId,
							StartDate = newTask.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							Title = newTask.Title,
							Status = status.Title,
							BaseResponse = new BaseResponse
							{
								IsSucceed = true,
								Message = "Create Successfully"
							},
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
									Description = request.Decription,
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
								transaction.Commit();

								var status = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == newTask.StatusId, null)!;
								return new CreateTaskResponse
								{
									AssignTo = newTask.AssignTo,
									CreateBy = userId,
									CreateTime = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Description = newTask.Description,
									DeleteAt = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									DueDate = newTask.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									TaskId = newTask.TaskId,
									InterationId = newTask.InterationId,
									IsDelete = newTask.IsDelete,
									PriorityId = newTask.PriorityId,
									TypeId = newTask.TypeId,
									StartDate = newTask.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Title = newTask.Title,
									Status = status.Title,
									StatusId = newTask.StatusId,
									BaseResponse = new BaseResponse
									{
										IsSucceed = true,
										Message = "Create Successfully"
									},
								};
							}
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
						Message = "Create fail : " + ex.Message
					}
				};
			}
		}

		public async Task<CreateTaskResponse> UpdateTask(UpdateTaskRequest updateTicketRequest)
		{
			using var transaction = _iterationRepository.DatabaseTransaction();
			var task = await _ticketRepository.GetAsync(x => x.TaskId == updateTicketRequest.TaskId, null);
			try
			{
				var ticketEntity = new DataAccess.Entities.Task()
				{
					Title = updateTicketRequest.Title,
					Description = updateTicketRequest.Decription,
					DueDate = DateTime.Parse(updateTicketRequest.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
					TypeId = updateTicketRequest.TypeId,
					PriorityId = updateTicketRequest.PriorityId,
					AssignTo = updateTicketRequest.AssignTo,
				};

				var newTask = await _ticketRepository.CreateAsync(ticketEntity);
				await _ticketRepository.SaveChanges();
				transaction.Commit();

				return new CreateTaskResponse
				{
					AssignTo = newTask.AssignTo,
					CreateBy = newTask.CreateBy,
					CreateTime = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
					Description = newTask.Description,
					DeleteAt = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
					DueDate = newTask.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
					TaskId = newTask.TaskId,
					InterationId = newTask.InterationId,
					IsDelete = newTask.IsDelete,
					PriorityId = newTask.PriorityId,
					TypeId = newTask.TypeId,
					StartDate = newTask.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
					Title = newTask.Title,
					StatusId = newTask.StatusId,
					BaseResponse = new BaseResponse
					{
						IsSucceed = true,
						Message = "Update Successfully"
					},
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
						IsSucceed = true,
						Message = "Update fail"
					}
				};
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

			public async Task<BaseResponse> DeleteTask(Guid ticketId)
			{
				using var transaction = _iterationRepository.DatabaseTransaction();
				try
				{
					var selectedTicket = await _ticketRepository.GetAsync(x => x.TaskId == ticketId, null)!;

					selectedTicket.DeleteAt = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
					selectedTicket.IsDelete = true;
					selectedTicket.ExprireTime = DateTime.Parse(DateTime.UtcNow.AddDays(30).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));

					await _ticketRepository.UpdateAsync(selectedTicket);
					await _context.SaveChangesAsync();
					transaction.Commit();

					return new BaseResponse
					{
						IsSucceed = true,
						Message = "Delete Successfully"
					};
				}
				catch (Exception e)
				{
					transaction.RollBack();
					return new BaseResponse
					{
						IsSucceed = false,
						Message = "Delete fail"
					};
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
					var statusCount = await _boardStatusRepository.GetAllWithOdata(x => x.BoardId == createNewTaskStatus.ProjectId, null);
					var newStatus = new BoardStatus
					{
						BoardId = createNewTaskStatus.ProjectId,
						BoardStatusId = Guid.NewGuid(),
						Order = statusCount.Count() + 1,
						Title = createNewTaskStatus.Title
					};
					var status = await _boardStatusRepository.CreateAsync(newStatus);
					await _boardStatusRepository.SaveChanges();
					transaction.Commit();

					return new StatusTaskViewModel
					{
						BoardId = status.BoardId,
						BoardStatusId = status.BoardStatusId,
						Title = status.Title,
						Order = status.Order,
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

			public async Task<CreateTaskResponse> CreateSubTask(CreateSubTaskRequest request, Guid userId)
			{
				using var transaction = _ticketRepository.DatabaseTransaction();
				var task = await _ticketRepository.GetAsync(x => x.TaskId == request.TaskId, null);
				try
				{
					var ticketEntity = new Task()
					{
						TaskId = Guid.NewGuid(),
						Title = request.Title,
						Description = request.Decription,
						StartDate = DateTime.Parse(request.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						DueDate = DateTime.Parse(request.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						CreateTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						CreateBy = userId,
						IsDelete = false,
						TypeId = request.TypeId,
						PriorityId = request.PriorityId,
						InterationId = task.InterationId,
						AssignTo = request.AssignTo,
						StatusId = request.StatusId,
						PrevId = request.TaskId
					};

					var newTask = await _ticketRepository.CreateAsync(ticketEntity);
					await _ticketRepository.SaveChanges();
					transaction.Commit();

					var status = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == newTask.StatusId, null)!;

					return new CreateTaskResponse
					{
						TaskId = Guid.NewGuid(),
						AssignTo = newTask.AssignTo,
						CreateBy = userId,
						CreateTime = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
						Description = newTask.Description,
						DeleteAt = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
						DueDate = newTask.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
						InterationId = newTask.InterationId,
						IsDelete = newTask.IsDelete,
						PriorityId = newTask.PriorityId,
						TypeId = newTask.TypeId,
						StartDate = newTask.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
						Title = newTask.Title,
						Status = status.Title,
						StatusId = newTask.StatusId,
						BaseResponse = new BaseResponse
						{
							IsSucceed = true,
							Message = "Create Successfully"
						},
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

			public async Task<List<GetAllTaskPriority>> GetAllTaskPriotiry()
			{
				var result = await _priorityRepository.GetAllWithOdata(x => true, null);
				return _mapper.Map<List<GetAllTaskPriority>>(result.OrderBy(x => x.Level));
			}

			public async Task<BaseResponse> RestoreTask(Guid taskId)
			{
				using var transaction = _ticketRepository.DatabaseTransaction();
				try
				{
					var task = await _ticketRepository.GetAsync(x => x.TaskId == taskId, null)!;
					task.IsDelete = false;
					task.DeleteAt = null;
					task.ExprireTime = null;

					await _ticketRepository.UpdateAsync(task);
					await _ticketRepository.SaveChanges();

					transaction.Commit();

					return new BaseResponse
					{
						IsSucceed = true,
						Message = "Restore successfully"
					};
				}
				catch (Exception)
				{
					transaction.RollBack();
					return new BaseResponse
					{
						IsSucceed = false,
						Message = "Restore fail"
					};
				}
			}

		public async Task<List<TaskViewModel>> GetAllTaskDeleteAsync(Guid projectId)
		{
			var result = await _ticketRepository.GetAllTaskDelete(projectId);
			return result;
		}
	}
	}