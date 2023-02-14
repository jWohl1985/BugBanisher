using BugBanisher.Database;
using BugBanisher.Models;
using BugBanisher.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugBanisher.Services;

public class TicketService : ITicketService
{
	private readonly BugBanisherContext _context;

	public TicketService(BugBanisherContext context, UserManager<AppUser> userManager)
	{
		_context = context;
	}

	public async Task<Ticket?> GetTicketByIdAsync(int ticketId)
	{
		return await _context.Tickets.Where(t => t.Id == ticketId)
			.Include(t => t.Project)
			.Include(t => t.Developer)
			.Include(t => t.Creator)
			.Include(t => t.Status)
			.Include(t => t.Priority)
			.Include(t => t.Type)
			.Include(t => t.Attachments)
				.ThenInclude(t => t.Uploader)
			.Include(t => t.History)
			.Include(t => t.Comments)
				.ThenInclude(t => t.AppUser)
			.FirstOrDefaultAsync();
	}

	public async Task<int> CreateTicketAsync(Ticket ticket)
	{
		await _context.Tickets.AddAsync(ticket);
		await _context.SaveChangesAsync();
		return ticket.Id;
	}

	public async Task<List<TicketPriority>> GetTicketPrioritiesAsync()
	{
		return await _context.TicketPriorities.ToListAsync();
	}

    public async Task<List<TicketStatus>> GetTicketStatusesAsync()
    {
        return await _context.TicketStatuses.ToListAsync();
    }

    public async Task<List<TicketType>> GetTicketTypesAsync()
    {
        return await _context.TicketTypes.ToListAsync();
    }

	public async Task<TicketAttachment?> GetTicketAttachmentByIdAsync(int ticketAttachmentId)
	{
		return await _context.TicketAttachments.FirstOrDefaultAsync(ta => ta.Id == ticketAttachmentId);
	}

	public async Task UpdateTicketAsync(Ticket ticket)
	{
		_context.Tickets.Update(ticket);
		await _context.SaveChangesAsync();
	}

	public async Task<bool> AddTicketCommentAsync(int ticketId, TicketComment comment)
	{
		Ticket? ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

		if (ticket is null)
			return false;

		ticket.Comments.Add(comment);
		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<bool> AddTicketAttachmentAsync(int ticketId, TicketAttachment attachment)
	{
		Ticket? ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

		if (ticket is null)
			return false;

		ticket.Attachments.Add(attachment);
		await _context.SaveChangesAsync();
		return true;
	}


}
