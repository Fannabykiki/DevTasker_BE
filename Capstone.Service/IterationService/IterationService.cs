﻿using AutoMapper;
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
        
        public async Task<IEnumerable<GetInterrationByBoardIdResonse>> GetIterationsByBoardId(Guid boardId)
        {
            var iterations = await _iterationRepository.GetAllWithOdata(x => x.BoardId == boardId, null);

            var result = new List<GetInterrationByBoardIdResonse>();

            foreach (var iteration in iterations)
            {
                var response = new GetInterrationByBoardIdResonse
                {
                    InterationId = iteration.InterationId,
                    InterationName = iteration.InterationName,
                    Status = iteration.Status
                };

                if (iteration.Status == InterationStatusEnum.Current)
                {
                    response.workItemResponses = await GetWorkItemsForIterationAsync(iteration);
                }

                result.Add(response);
            }

            return result;
        }

        private async Task<List<WorkItemResponse>> GetWorkItemsForIterationAsync(Interation iteration)
        {
            var workItems = new List<WorkItemResponse>();

            foreach (var ticket in iteration.Tickets)
            {
                if (ticket.PrevId == null)
                {
                    var item = new WorkItemResponse
                    {
                        TicketId = ticket.TicketId,
                        Title = ticket.Title,
                        TicketType = ticket.TicketType,
                        TicketStatus = ticket.TicketStatus
                    };

                    item.Tickets = await GetChildTicketsAsync(ticket.TicketId, iteration.Tickets);

                    workItems.Add(item);
                }
            }

            return workItems;
        }

        private async Task<List<TicketResponse>> GetChildTicketsAsync(Guid parentId, List<Ticket> allTickets)
        {
            return allTickets
              .Where(x => x.PrevId == parentId)
              .Select(x => new TicketResponse
              {
                  TicketId = x.TicketId,
                  Title = x.Title,
                  TicketStatus = x.TicketStatus,
                  TicketType = x.TicketType
              })
              .ToList();
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
