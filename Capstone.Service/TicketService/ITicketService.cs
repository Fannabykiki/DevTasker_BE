using Capstone.Common.DTOs.Task;
using Capstone.DataAccess.Entities;

namespace Capstone.Service.TicketService
{
    public  interface ITicketService
    {
        Task<bool> CreateTicket(CreateTicketRequest createTicketRequest, Guid iterationId);
       

    }
}
