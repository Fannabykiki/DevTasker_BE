using AutoMapper;
using Capstone.Common.DTOs.Iteration;
using Capstone.Common.DTOs.PermissionSchema;
using Capstone.Common.Enums;
using Capstone.DataAccess;
using Capstone.DataAccess.Entities;
using Capstone.DataAccess.Repository.Implements;
using Capstone.DataAccess.Repository.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public IterationService(CapstoneContext context, IProjectRepository projectRepository, IMapper mapper, IInterationRepository iterationRepository, IBoardRepository boardRepository, ITicketRepository ticketRepository)
        {
            _context = context;
            _projectRepository = projectRepository;
            _mapper = mapper;
            _iterationRepository = iterationRepository;
            _boardRepository = boardRepository;
            _ticketRepository = ticketRepository;

        }
        
        public async Task<List<GetAllInterrationByProjectIdResonse>> GetIterationsByProjectId(Guid projectId)
        {
            var interations = await _iterationRepository.GetAllWithOdata(x => x.ProjectId == projectId, null);
            foreach (var interation in interations)
            {
                if (interation.Status == InterationStatusEnum.Current)
                {
                    var listWorkItem = new List<WorkItemResponse>();
                    foreach (var board in interation.Boards)
                    {
                        var workItems = board.Tickets.Where(x => x.PrevId == null);

                        var listTaskInWorkItem = new List<TicketResponse>();
                        foreach (var workItem in workItems)
                        {
                            var taskInWorkItems = board.Tickets.Where(x => x.PrevId == workItem.TicketId);
                        }
                    }
                }
            }

            var iterations = await _iterationRepository.GetAllWithOdata(x => x.ProjectId == projectId, null);

            // Map iterations to response model
            var response = _mapper.Map<List<GetAllInterrationByProjectIdResonse>>(interations);
            return response;
        }

       

        public async Task<bool> CreateIteration(CreateIterationRequest createIterationRequest, Guid projectId)
        {
            using var transaction = _iterationRepository.DatabaseTransaction();

            try
            {
                //var newIterationRequest = new Interation
                //{
                //    InterationName = createIterationRequest.InterationName,
                //    StartDate = createIterationRequest.StartDate,
                //    EndDate = createIterationRequest.EndDate,
                //    ProjectId = projectId,
                //    Status = createIterationRequest.Status
                //};

               
                //var newIteration = await _iterationRepository.CreateAsync(newIterationRequest);
                //var project = await _projectRepository.GetAsync(x => x.ProjectId == projectId, null);
                //project.Interations.Add(newIteration);
                //await _projectRepository.UpdateAsync(project);

                //_iterationRepository.SaveChanges();
                //_projectRepository.SaveChanges();

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
                var iteration = await _iterationRepository.GetAsync(x => x.InterationId == iterationId, null);

                if (iteration != null)
                {
                    iteration.InterationName = updateIterationRequest.InterationName;
                    iteration.StartDate = updateIterationRequest.StartDate;
                    iteration.EndDate = updateIterationRequest.EndDate;
                    iteration.Status = updateIterationRequest.Status;


                    await _iterationRepository.UpdateAsync(iteration);

                    _iterationRepository.SaveChanges();
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
            }
        }
    }
}
