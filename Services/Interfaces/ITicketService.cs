using BugBanisher.Models;

namespace BugBanisher.Services.Interfaces;

public interface ITicketService
{
	Task<Ticket?> GetTicketByIdAsync(int ticketId);
	Task<int> CreateTicketAsync(Ticket ticket);
	Task<List<TicketPriority>> GetTicketPrioritiesAsync();
	Task<List<TicketStatus>> GetTicketStatusesAsync();
	Task<List<TicketType>> GetTicketTypesAsync();
	Task UpdateTicketAsync(Ticket ticket);
	Task<bool> AddTicketCommentAsync(int ticketId, TicketComment comment);
	Task<bool> AddTicketAttachmentAsync(int ticketId, TicketAttachment attachment);
}
