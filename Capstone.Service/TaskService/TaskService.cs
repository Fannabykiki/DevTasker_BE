﻿using AutoMapper;
using Capstone.Common.DTOs.Base;
using Capstone.Common.DTOs.Task;
using Capstone.Common.DTOs.TaskPriority;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Interfaces;
using Capstone.Service.TicketService;
using System.Threading.Tasks;
using Task = Capstone.DataAccess.Entities.Task;

namespace Capstone.Service.TaskService
{
	public class TaskService : ITaskService
	{
		private readonly CapstoneContext _context;
		private readonly ITaskRepository _ticketRepository;
		private readonly ITicketStatusRepository _ticketStatusRepository;
		private readonly ITaskTypeRepository _typeRepository;
		private readonly ITicketHistoryRepository _taskHistoryRepository;
		private readonly ITaskTypeRepository _ticketTypeRepository;
		private readonly IMapper _mapper;
		private readonly IUserRepository _userRepository;
		private readonly IInterationRepository _iterationRepository;
		private readonly IStatusRepository _statusRepository;
		private readonly IBoardStatusRepository _boardStatusRepository;
		private readonly ITaskTypeRepository _taskType;
		private readonly IPriorityRepository _priorityRepository;
		private readonly IProjectMemberRepository _projectMemberRepository;
		private readonly IBoardRepository _boardRepository;

		public TaskService(CapstoneContext context, ITaskRepository ticketRepository,
			ITicketStatusRepository ticketStatusRepository, ITaskTypeRepository typeRepository,
			ITicketHistoryRepository ticketHistoryRepository, ITaskTypeRepository ticketTypeRepository,
			IMapper mapper, IUserRepository userRepository, IInterationRepository iterationRepository,
			IStatusRepository statusRepository, IBoardStatusRepository boardStatusRepository, ITaskTypeRepository taskType, IPriorityRepository priorityRepository, IProjectMemberRepository projectMemberRepository, IBoardRepository boardRepository)
		{
			_context = context;
			_ticketRepository = ticketRepository;
			_ticketStatusRepository = ticketStatusRepository;
			_typeRepository = typeRepository;
			_taskHistoryRepository = ticketHistoryRepository;
			this._ticketTypeRepository = ticketTypeRepository;
			_mapper = mapper;
			_userRepository = userRepository;
			_iterationRepository = iterationRepository;
			_statusRepository = statusRepository;
			_boardStatusRepository = boardStatusRepository;
			_taskType = taskType;
			_priorityRepository = priorityRepository;
			_projectMemberRepository = projectMemberRepository;
			_boardRepository = boardRepository;
		}

		public async Task<CreateTaskResponse> CreateTask(CreateTaskRequest request, Guid userId)
		{
			using var transaction = _ticketRepository.DatabaseTransaction();
			var interations = await _iterationRepository.GetAllWithOdata(x => x.BoardId == request.ProjectId, null);
			try
			{
				if (request.TypeId == Guid.Empty)
				{
					if (request.InterationId != Guid.Empty)
					{
						var ticketEntity = new DataAccess.Entities.Task()
						{
							TaskId = Guid.NewGuid(),
							Title = request.Title,
							Description = request.Description,
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
						var status = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == newTask.StatusId, null);

						await _ticketRepository.SaveChanges();
						transaction.Commit();

						return new CreateTaskResponse
						{
							AssignTo = newTask.AssignTo,
							CreateBy = userId,
							CreateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							Description = newTask.Description,
							DeleteAt = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							DueDate = newTask.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							TaskId = newTask.TaskId,
							InterationId = newTask.InterationId,
							IsDelete = newTask.IsDelete,
							PriorityId = newTask.PriorityId,
							TypeId = newTask.TypeId,
							StartDate = newTask.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							Title = newTask.Title,
							StatusId = newTask.StatusId,
							StatusName = status.Title,
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
									Description = request.Description,
									StartDate = DateTime.Parse(request.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
									DueDate = DateTime.Parse(request.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
									CreateTime = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
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
									CreateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Description = newTask.Description,
									DeleteAt = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									DueDate = newTask.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									TaskId = newTask.TaskId,
									InterationId = newTask.InterationId,
									IsDelete = newTask.IsDelete,
									PriorityId = newTask.PriorityId,
									TypeId = newTask.TypeId,
									StartDate = newTask.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
									Title = newTask.Title,
									StatusName = status.Title,
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
							Description = request.Description,
							StartDate = DateTime.Parse(request.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
							DueDate = DateTime.Parse(request.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
							CreateTime = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
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
							CreateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							Description = newTask.Description,
							DeleteAt = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							DueDate = newTask.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							TaskId = newTask.TaskId,
							InterationId = newTask.InterationId,
							IsDelete = newTask.IsDelete,
							PriorityId = newTask.PriorityId,
							TypeId = newTask.TypeId,
							StartDate = newTask.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
							Title = newTask.Title,
							StatusName = status.Title,
							StatusId = newTask.StatusId,
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
									Description = request.Description,
									StartDate = DateTime.Parse(request.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
									DueDate = DateTime.Parse(request.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
									CreateTime = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
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
									StatusName = status.Title,
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
			using var transaction = _ticketRepository.DatabaseTransaction();
			try
			{
				var task = await _ticketRepository.GetAsync(x => x.TaskId == updateTicketRequest.TaskId, null);
				var member = await _projectMemberRepository.GetAsync(x => x.MemberId == updateTicketRequest.MemberId, x => x.Users);
				var newHistory = new TaskHistory
				{
					ChangeAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
					ChangeBy = updateTicketRequest.MemberId,
					CurrentStatusId = updateTicketRequest.StatusId,
					PreviousStatusId = task.StatusId,
					HistoryId = Guid.NewGuid(),
					TaskId = updateTicketRequest.TaskId,
					Title = $"Task {task.Title} has been updated by {member.Users.UserName}"
				};

				if(task.InterationId != updateTicketRequest.InterationId)
				{
					var subTasks = await _ticketRepository.GetAllWithOdata(x => x.PrevId == updateTicketRequest.TaskId, null);
					foreach (var subTask in subTasks)
					{
						subTask.InterationId = updateTicketRequest.InterationId;

						await _ticketRepository.UpdateAsync(subTask);
						await _ticketRepository.SaveChanges();

						var newSubTaskHistory = new TaskHistory
						{
							ChangeAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
							ChangeBy = updateTicketRequest.MemberId,
							CurrentStatusId = updateTicketRequest.StatusId,
							PreviousStatusId = task.StatusId,
							HistoryId = Guid.NewGuid(),
							TaskId = subTask.TaskId,
							Title = $"Task {subTask.Title} has been updated sprint because task {task.Title} has been updated by {member.Users.UserName}"
						};

						await _taskHistoryRepository.CreateAsync(newSubTaskHistory);
						await _taskHistoryRepository.SaveChanges();
					}
				}

				await _taskHistoryRepository.CreateAsync(newHistory);
				await _taskHistoryRepository.SaveChanges();

				task.InterationId = updateTicketRequest.InterationId;
				task.Title = updateTicketRequest.Title;
				task.Description = updateTicketRequest.Description;
				task.DueDate = DateTime.Parse(updateTicketRequest.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
				task.AssignTo = updateTicketRequest.AssignTo;
				task.TypeId = updateTicketRequest.TypeId;
				task.PriorityId = updateTicketRequest.PriorityId;
				task.StatusId = updateTicketRequest.StatusId;

				var updateTask = await _ticketRepository.UpdateAsync(task);
				await _ticketRepository.SaveChanges();
				transaction.Commit();

				return new CreateTaskResponse
				{
					AssignTo = task.AssignTo,
					CreateBy = task.CreateBy,
					CreateTime = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
					Description = task.Description,
					DeleteAt = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
					DueDate = task.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
					TaskId = task.TaskId,
					InterationId = task.InterationId,
					IsDelete = task.IsDelete,
					PriorityId = task.PriorityId,
					TypeId = task.TypeId,
					StartDate = task.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
					Title = task.Title,
					StatusId = task.StatusId,
					BaseResponse = new BaseResponse
					{
						IsSucceed = true,
						Message = task.PrevId == null ? "Update Task Successfully" : "Update Subtask Successfully"
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

		public async Task<CreateTaskResponse> UpdateTaskStatus(Guid taskId, UpdateTaskStatusRequest updateTaskStatusRequest)
		{
			using var transaction = _iterationRepository.DatabaseTransaction();
			try
			{
				var task = await _ticketRepository.GetAsync(x => x.TaskId == taskId, x => x.Status);
				var newStatus = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == updateTaskStatusRequest.StatusId, null);
				var member = await _projectMemberRepository.GetAsync(x => x.MemberId == updateTaskStatusRequest.MemberId, x => x.Users);
				task.StatusId = updateTaskStatusRequest.StatusId;

				var newHistory = new TaskHistory
				{
					ChangeAt = DateTime.Now,
					ChangeBy = updateTaskStatusRequest.MemberId,
					CurrentStatusId = updateTaskStatusRequest.StatusId,
					PreviousStatusId = task.StatusId,
					HistoryId = Guid.NewGuid(),
					TaskId = taskId,
					Title = $"Task {task.Title} has been changed from {task.Status.Title} to {newStatus.Title} by {member.Users.UserName}"
				};

				await _taskHistoryRepository.CreateAsync(newHistory);
				await _taskHistoryRepository.SaveChanges();
				var updateTask = await _ticketRepository.UpdateAsync(task);
				await _ticketRepository.SaveChanges();
				transaction.Commit();

				var status = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == task.StatusId, null);

				return new CreateTaskResponse
				{
					AssignTo = task.AssignTo,
					CreateBy = task.CreateBy,
					CreateTime = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
					Description = task.Description,
					DeleteAt = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
					DueDate = task.DueDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
					TaskId = task.TaskId,
					InterationId = task.InterationId,
					IsDelete = task.IsDelete,
					PriorityId = task.PriorityId,
					TypeId = task.TypeId,
					StartDate = task.StartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"),
					Title = task.Title,
					StatusId = task.StatusId,
					StatusName = status.Title,
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

		public async Task<BaseResponse> DeleteTask(RestoreTaskRequest restoreTaskRequest)
		{
			using var transaction = _iterationRepository.DatabaseTransaction();
			try
			{
				var selectedTicket = await _ticketRepository.GetAsync(x => x.TaskId == restoreTaskRequest.TaskId, x => x.Status)!;
				var member = await _projectMemberRepository.GetAsync(x => x.MemberId == restoreTaskRequest.MemberId, x => x.Users);
				var subTaskList = await _ticketRepository.GetAllWithOdata(x => x.PrevId == restoreTaskRequest.TaskId, null);

				selectedTicket.DeleteAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
				selectedTicket.IsDelete = true;
				selectedTicket.ExprireTime = DateTime.Parse(DateTime.Now.AddDays(30).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));

				foreach (var subTask in subTaskList)
				{
					if (subTask.PrevId == restoreTaskRequest.TaskId)
					{
						subTask.DeleteAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
						subTask.IsDelete = true;
						subTask.ExprireTime = DateTime.Parse(DateTime.Now.AddDays(30).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));

						await _ticketRepository.UpdateAsync(subTask);
						await _ticketRepository.SaveChanges();
					}

					var newHistorySubTask = new TaskHistory
					{
						ChangeAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						ChangeBy = restoreTaskRequest.MemberId,
						CurrentStatusId = subTask.StatusId,
						PreviousStatusId = subTask.StatusId,
						HistoryId = Guid.NewGuid(),
						TaskId = restoreTaskRequest.TaskId,
						Title = $"Task {subTask.Title} has been deleted by {member.Users.UserName}"
					};

					await _taskHistoryRepository.CreateAsync(newHistorySubTask);
					await _taskHistoryRepository.SaveChanges();
				}

				var newHistory = new TaskHistory
				{
					ChangeAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
					ChangeBy = restoreTaskRequest.MemberId,
					CurrentStatusId = selectedTicket.StatusId,
					PreviousStatusId = selectedTicket.StatusId,
					HistoryId = Guid.NewGuid(),
					TaskId = restoreTaskRequest.TaskId,
					Title = $"Task {selectedTicket.Title} has been deleted by {member.Users.UserName}"
				};

				await _taskHistoryRepository.CreateAsync(newHistory);
				await _taskHistoryRepository.SaveChanges();

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
			var result = (await _boardStatusRepository.GetAllWithOdata(x => x.BoardId == projectId, null)).OrderBy(x => x.Order);
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
					Title = createNewTaskStatus.Title,
					StatusId = Guid.Parse("BB93DD2D-B9E7-401F-83AA-174C588AB9DE"),
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

			try
			{
				var task = await _ticketRepository.GetAsync(x => x.TaskId == request.TaskId, null);
				if (request.StartDate.Date < task.StartDate.Date)
				{
					return new CreateTaskResponse
					{
						BaseResponse = new BaseResponse
						{
							StatusCode = 400,
							Message = "Can't create subtask's start date before task's start date",
							IsSucceed = false,
						}
					};
				}
				else if (request.DueDate.Date > task.DueDate.Date)
				{
					return new CreateTaskResponse
					{
						BaseResponse = new BaseResponse
						{
							StatusCode = 400,
							Message = "Can't create subtask's end date after task's end date",
							IsSucceed = false,
						}
					};
				}
				var ticketEntity = new Task()
				{
					TaskId = Guid.NewGuid(),
					Title = request.Title,
					Description = request.Description,
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
					TaskId = newTask.TaskId,
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
					StatusName = status.Title,
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

		public async Task<BaseResponse> RestoreTask(RestoreTaskRequest restoreTaskRequest)
		{
			using var transaction = _ticketRepository.DatabaseTransaction();
			try
			{
				var task = await _ticketRepository.GetAsync(x => x.TaskId == restoreTaskRequest.TaskId, null)!;
				var member = await _projectMemberRepository.GetAsync(x => x.MemberId == restoreTaskRequest.MemberId, x => x.Users);

				task.IsDelete = false;
				task.DeleteAt = null;
				task.ExprireTime = null;

				var newHistory = new TaskHistory
				{
					ChangeAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
					ChangeBy = restoreTaskRequest.MemberId,
					CurrentStatusId = task.StatusId,
					PreviousStatusId = task.StatusId,
					HistoryId = Guid.NewGuid(),
					TaskId = restoreTaskRequest.TaskId,
					Title = $"Task {task.Title} has been restored by {member.Users.UserName}"
				};

				await _taskHistoryRepository.CreateAsync(newHistory);
				await _taskHistoryRepository.SaveChanges();

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

		public async Task<TaskDetailViewModel> GetTaskDetail(Guid taskId)
		{
			var result = await _ticketRepository.GetTaskDetail(taskId);
			return result;
		}

		public async Task<bool> CheckExist(Guid taskId)
		{
			var task = await _ticketRepository.GetAsync(x => x.TaskId == taskId, null);
			if (task == null) return false;
			return true;
		}
		public async Task<UpdateTaskOrderResponse> UpdateTaskOrder(UpdateTaskOrderRequest updateTaskOrderRequest)
		{
			using var transaction = _ticketRepository.DatabaseTransaction();
			try
			{
				var count = 0;
				var boardstatus = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == updateTaskOrderRequest.StatusId, null);
				var taskStatus = await _boardStatusRepository.GetAllWithOdata(x => x.BoardId == boardstatus.BoardId, null);
				//2					<		//5
				if (boardstatus.Order < updateTaskOrderRequest.Order)
				{
					//5
					foreach (var status in taskStatus)
					{
						if (count == updateTaskOrderRequest.Order - boardstatus.Order)
						{
							break;
						}
						if (updateTaskOrderRequest.Order == boardstatus.Order)
						{
							break;
						}
						//1 2 3 4 5
						if (status.Order > boardstatus.Order)
						{
							status.Order -= 1;
							var board = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == status.BoardStatusId, null);
							await _boardStatusRepository.UpdateAsync(board);
							await _boardStatusRepository.SaveChanges();
							count++;
						}
					}
				}
				else if (boardstatus.Order == updateTaskOrderRequest.Order)
				{
					return new UpdateTaskOrderResponse
					{
						BoardId = boardstatus.BoardId,
						BoardStatusId = boardstatus.BoardStatusId,
						Order = boardstatus.Order,
						Title = boardstatus.Title,
						IsSucceed = true,
						Message = "Nothing to update"
					};
				}
				else
				{
					foreach (var status in taskStatus)
					{
						if (count == boardstatus.Order - updateTaskOrderRequest.Order)
						{
							break;
						}
						if (updateTaskOrderRequest.Order == boardstatus.Order)
						{
							break;
						}
						else if (status.Order >= updateTaskOrderRequest.Order)
						{
							status.Order += 1;
							var board = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == status.BoardStatusId, null);
							await _boardStatusRepository.UpdateAsync(board);
							await _boardStatusRepository.SaveChanges();
							count++;
						}
					}
				}
				boardstatus.Order = updateTaskOrderRequest.Order;
				await _boardStatusRepository.UpdateAsync(boardstatus);
				await _boardStatusRepository.SaveChanges();
				transaction.Commit();

				return new UpdateTaskOrderResponse
				{
					BoardId = boardstatus.BoardId,
					BoardStatusId = boardstatus.BoardStatusId,
					Order = boardstatus.Order,
					Title = boardstatus.Title,
					IsSucceed = true,
					Message = "Update successfully"
				};
			}
			catch
			{
				transaction.RollBack();
				return new UpdateTaskOrderResponse
				{
					IsSucceed = false,
					Message = "Update fail"
				};
			}
		}

		public async Task<Guid?> GetProjectIdOfTask(Guid taskId)
		{
			var task = await _ticketRepository.GetAsync(x => x.TaskId == taskId, x => x.Interation);
			var projectId = task.Interation.BoardId;
			return projectId;
		}

		public async Task<UpdateTaskOrderResponse> UpdateTaskTitle(UpdateTaskNameRequest updateTaskNameRequest)
		{
			using var transaction = _boardStatusRepository.DatabaseTransaction();
			try
			{
				var status = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == updateTaskNameRequest.StatusTaskId, null);
				status.Title = updateTaskNameRequest.Title;
				await _boardStatusRepository.UpdateAsync(status);
				await _boardStatusRepository.SaveChanges();
				transaction.Commit();

				return new UpdateTaskOrderResponse
				{
					IsSucceed = true,
					Message = "Update title successfully",
					BoardId = status.BoardId,
					BoardStatusId = status.BoardStatusId,
					Order = status.Order,
					Title = status.Title,
				};
			}
			catch
			{
				transaction.RollBack();
				return new UpdateTaskOrderResponse
				{
					IsSucceed = false,
					Message = "Update title fail",
				};
			}
		}

		public async Task<BaseResponse> DeleteTaskStatus(DeleteTaskStatusRequest deleteTaskStatusRequest)
		{
			using var transaction = _boardStatusRepository.DatabaseTransaction();
			try
			{
				var status = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == deleteTaskStatusRequest.TaskStatusId, null);
				var member = await _projectMemberRepository.GetAsync(x => x.MemberId == deleteTaskStatusRequest.MemberId, x => x.Users);

				status.StatusId = Guid.Parse("C59F200A-C557-4492-8D0A-5556A3BA7D31");

				await _boardStatusRepository.UpdateAsync(status);
				await _boardStatusRepository.SaveChanges();

				var taskList = await _ticketRepository.GetAllWithOdata(x => x.StatusId == deleteTaskStatusRequest.TaskStatusId, null);
				foreach (var task in taskList)
				{
					var taskDetail = await _ticketRepository.GetAsync(x => x.TaskId == task.TaskId, null);

					taskDetail.DeleteAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
					taskDetail.IsDelete = true;
					taskDetail.ExprireTime = DateTime.Parse(DateTime.Now.AddDays(30).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));

					var newHistory = new TaskHistory
					{
						ChangeAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
						ChangeBy = deleteTaskStatusRequest.MemberId,
						CurrentStatusId = taskDetail.StatusId,
						PreviousStatusId = taskDetail.StatusId,
						HistoryId = Guid.NewGuid(),
						TaskId = taskDetail.TaskId,
						Title = $"Task {taskDetail.Title} has been deleted by {member.Users.UserName} because of removing {status.Title} status"
					};

					await _taskHistoryRepository.CreateAsync(newHistory);
					await _taskHistoryRepository.SaveChanges();
				}
				transaction.Commit();

				return new UpdateTaskOrderResponse
				{
					IsSucceed = true,
					Message = "Delete successfully",
					BoardId = status.BoardId,
					BoardStatusId = status.BoardStatusId,
					Order = status.Order,
					Title = status.Title,
				};
			}
			catch
			{
				transaction.RollBack();
				return new UpdateTaskOrderResponse
				{
					IsSucceed = false,
					Message = "Delete fail",
				};
			}
		}

		public async Task<bool> CheckTaskStatus(Guid statusId)
		{
			var status = await _boardStatusRepository.GetAsync(x => x.BoardStatusId == statusId, null);
			if (status.StatusId == Guid.Parse("C59F200A-C557-4492-8D0A-5556A3BA7D31")) return false;
			return true;
		}

		public async Task<BaseResponse> DeleteEachTask(RestoreTaskRequest restoreTaskRequest)
		{
			using var transaction = _iterationRepository.DatabaseTransaction();
			try
			{
				var selectedTicket = await _ticketRepository.GetAsync(x => x.TaskId == restoreTaskRequest.TaskId, x => x.Status)!;
				var member = await _projectMemberRepository.GetAsync(x => x.MemberId == restoreTaskRequest.MemberId, x => x.Users);

				selectedTicket.DeleteAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
				selectedTicket.IsDelete = true;
				selectedTicket.ExprireTime = DateTime.Parse(DateTime.Now.AddDays(30).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
			
				var newHistory = new TaskHistory
				{
					ChangeAt = DateTime.Parse(DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
					ChangeBy = restoreTaskRequest.MemberId,
					CurrentStatusId = selectedTicket.StatusId,
					PreviousStatusId = selectedTicket.StatusId,
					HistoryId = Guid.NewGuid(),
					TaskId = restoreTaskRequest.TaskId,
					Title = $"Task {selectedTicket.Title} has been deleted by {member.Users.UserName}"
				};

				await _taskHistoryRepository.CreateAsync(newHistory);
				await _taskHistoryRepository.SaveChanges();

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

		public async Task<TaskDetail> GetTask(Guid taskId)
		{
			var result = await _ticketRepository.GetAsync(x=>x.TaskId == taskId, null);
			return _mapper.Map<TaskDetail>(result);
		}

		public async Task<TaskDetail> GetTaskParentDetail(Guid? prevId)
		{
			var result = await _ticketRepository.GetAsync(x => x.TaskId == prevId, null);
			return _mapper.Map<TaskDetail>(result);
		}
	}
}