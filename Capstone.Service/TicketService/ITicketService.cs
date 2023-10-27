using Capstone.Common.DTOs.Task;

namespace Capstone.Service.TicketService
{
    public  interface ITicketService
    {
        Task<bool> CreateTicket(CreateTicketRequest createTicketRequest, Guid iterationId);

    }
}
