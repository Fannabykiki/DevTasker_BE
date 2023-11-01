using Capstone.Common.DTOs.Ticket;
using Capstone.DataAccess.Entities;

namespace Capstone.Service.TicketService
{
    public  interface ITicketService
    {
        Task<bool> CreateTicket(CreateTicketRequest createTicketRequest, Guid interationId);
        Task<bool> UpdateTicket(UpdateTicketRequest updateTicketRequest, Guid ticketId);
        Task<IQueryable<Ticket>> GetAllTicketAsync();
        Task<IQueryable<Ticket>> GetAllTicketByInterationIdAsync(Guid interationId);
        Task<bool> DeleteTicket(Guid ticketId);
        
        
    }
}
