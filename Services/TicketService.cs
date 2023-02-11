using BugBanisher.Database;
using BugBanisher.Models;
using BugBanisher.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugBanisher.Services;

public class TicketService : ITicketService
{
	private readonly BugBanisherContext _context;
	private readonly UserManager<AppUser> _userManager;

	public TicketService(BugBanisherContext context, UserManager<AppUser> userManager)
	{
		_context = context;
		_userManager = userManager;
	}

	public async Task<Ticket?> GetTicketByIdAsync(int ticketId)
	{
		return await _context.Tickets.Where(t => t.Id == ticketId)
			.Include(t => t.Attachments)
			.Include(t => t.History)
			.Include(t => t.Comments)
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

	public async Task UpdateTicketAsync(Ticket ticket)
	{
		_context.Tickets.Update(ticket);
		await _context.SaveChangesAsync();
	}


}
