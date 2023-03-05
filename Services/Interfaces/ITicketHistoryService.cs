using BugBanisher.Models;

namespace BugBanisher.Services.Interfaces;

public interface ITicketHistoryService
{
    Task AddTicketCreatedEventAsync(int ticketId);
    Task AddDeveloperAssignmentEventAsync(Ticket ticket, string userMakingChangeId);
    Task AddAttachmentEventAsync(Ticket ticket, TicketAttachment attachment);
    Task AddTicketHistoryItemAsync(TicketHistory history);
    Task AddArchiveChangeEventAsync(Ticket ticket, string userMakingChangeId);
}
