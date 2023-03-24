using BugBanisher.Database;
using BugBanisher.Models;
using BugBanisher.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugBanisher.Services
{
    public class TicketHistoryService : ITicketHistoryService
    {
        private readonly BugBanisherContext _context;
        private readonly UserManager<AppUser> _userManager;

        public TicketHistoryService(BugBanisherContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task AddArchiveChangeEventAsync(Ticket ticket, string userMakingChangeId)
        {
            AppUser userMakingChange = await _userManager.FindByIdAsync(userMakingChangeId);

            string archivedOrUnarchived = ticket.IsArchived ? "archived" : "unarchived";

            TicketHistory archiveEvent = new TicketHistory()
            {
                TicketId = ticket.Id,
                AppUserId = userMakingChangeId,
                Created = DateTime.Now,
                Description = $"{userMakingChange.FullName} {archivedOrUnarchived} the ticket.",
            };

            await _context.TicketHistories.AddAsync(archiveEvent);
            await _context.SaveChangesAsync();
        }

        public async Task AddAttachmentEventAsync(Ticket ticket, TicketAttachment attachment)
        {
            AppUser attachingUser = await _userManager.FindByIdAsync(attachment.AppUserId);

            TicketHistory attachmentEvent = new TicketHistory()
            {
                TicketId = ticket.Id,
                AppUserId = attachment.AppUserId,
                Created = attachment.Created,
                Description = $"{attachingUser.FullName} added attachment {attachment.FileName}",
            };

            await _context.TicketHistories.AddAsync(attachmentEvent);
            await _context.SaveChangesAsync();
        }

        public async Task AddDeveloperAssignmentEventAsync(Ticket ticket, string userMakingChangeId)
        {
            AppUser userMakingChange = await _userManager.FindByIdAsync(userMakingChangeId);

            TicketHistory developerAssignmentEvent = new TicketHistory()
            {
                TicketId = ticket.Id,
                AppUserId = userMakingChangeId,
                Created = DateTime.Now,
                Description = $"{userMakingChange.FullName} changed the ticket assignment to {ticket.Developer!.FullName}",
            };

            await _context.TicketHistories.AddAsync(developerAssignmentEvent);
            await _context.SaveChangesAsync();
        }

        public async Task AddTicketHistoryItemAsync(TicketHistory history)
        {
            await _context.TicketHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        public async Task AddTicketCreatedEventAsync(int ticketId)
        {
            Ticket? ticket = await _context.Tickets
                .Include(t => t.Priority)
                .Include(t => t.Status)
                .Include(t => t.Type)
                .Include(t => t.Developer)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket is null)
                return;

            AppUser creator = await _userManager.FindByIdAsync(ticket.CreatorId);

            TicketHistory createdEvent = new TicketHistory()
            {
                TicketId = ticket.Id,
                AppUserId = ticket.CreatorId,
                Created = ticket.Created,
                Description = $"{creator.FullName} created the ticket</br></br>" +
                $"Title: {ticket.Title}</br>" +
                $"Priority: {ticket.Priority!.Description}</br>" +
                $"Type: {ticket.Type!.Description}</br>" +
                $"Assigned To: {(ticket.Developer is not null ? ticket.Developer.FullName : "Unassigned")}</br></br>" +
                $"Description: {ticket.Description}",
            };

            await _context.TicketHistories.AddAsync(createdEvent);
            await _context.SaveChangesAsync();
        }
    }
}
