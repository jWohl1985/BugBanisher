﻿using BugBanisher.Database;
using BugBanisher.Models;
using BugBanisher.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

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

	public async Task<List<Ticket>> GetAllOpenCompanyTicketsAsync(int companyId)
	{
		List<Project> activeCompanyProjects = await _context.Projects
			.Where(p => p.CompanyId == companyId && !p.IsArchived)
			.Include(p => p.Tickets)
			.ToListAsync();

		return await _context.Tickets.Where(t => !t.IsArchived && !(t.TicketStatusId == "complete"))
			.Include(t => t.Project)
			.Where(t => activeCompanyProjects.Contains(t.Project!))
			.Include(t => t.Priority)
			.Include(t => t.Status)
			.Include(t => t.Type)
			.Include(t => t.Developer)
			.ToListAsync();
	}

	public async Task<List<Ticket>> GetUserOpenTicketsAsync(string userId)
	{
		List<Ticket> userTickets = new List<Ticket>();

		AppUser? user = await _userManager.FindByIdAsync(userId);

		if (user is null)
			return userTickets;

        List<Project> activeCompanyProjects = await _context.Projects
            .Where(p => p.CompanyId == user.CompanyId && !p.IsArchived)
            .Include(p => p.Tickets)
            .ToListAsync();

        return await _context.Tickets.Where(t => !t.IsArchived && !(t.TicketStatusId == "complete"))
            .Include(t => t.Project)
            .Where(t => activeCompanyProjects.Contains(t.Project!))
			.Where(t => t.Project!.ProjectManagerId == userId || t.DeveloperId == userId)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.Type)
            .Include(t => t.Developer)
            .ToListAsync();
    }

	public async Task<List<Ticket>> GetAllActionRequiredTicketsAsync(int companyId)
	{
        List<Project> activeCompanyProjects = await _context.Projects
            .Where(p => p.CompanyId == companyId && !p.IsArchived)
            .Include(p => p.Tickets)
            .ToListAsync();

		return await _context.Tickets.Where(t => !t.IsArchived && !(t.TicketStatusId == "complete"))
            .Include(t => t.Project)
            .Where(t => activeCompanyProjects.Contains(t.Project!))
			.Where(t => t.TicketStatusId == "unassigned" || t.TicketStatusId == "hold" || t.TicketStatusId == "pending")
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.Type)
            .Include(t => t.Developer)
            .ToListAsync();
    }

    public async Task<List<Ticket>> GetUserActionRequiredTicketsAsync(string userId)
    {
        List<Ticket> problemTickets = new List<Ticket>();

		AppUser user = await _userManager.FindByIdAsync(userId);

		if (user is null)
			return problemTickets;

        List<Project> activeCompanyProjects = await _context.Projects
            .Where(p => p.CompanyId == user.CompanyId && !p.IsArchived)
            .Include(p => p.Tickets)
            .ToListAsync();

        return await _context.Tickets.Where(t => !t.IsArchived && !(t.TicketStatusId == "complete"))
            .Include(t => t.Project)
            .Where(t => activeCompanyProjects.Contains(t.Project!))
            .Where(t => t.TicketStatusId == "unassigned" || t.TicketStatusId == "hold" || t.TicketStatusId == "pending")
			.Where(t => t.Project!.ProjectManagerId == userId || t.DeveloperId == userId)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.Type)
            .Include(t => t.Developer)
            .ToListAsync();
    }

    public async Task<List<Ticket>> GetCompletedTicketsAsync(int companyId)
    {
        List<Project> activeProjects = await _context.Projects
            .Where(p => p.CompanyId == companyId && !p.IsArchived)
            .Include(p => p.Tickets)
            .ToListAsync();

        return await _context.Tickets
            .Include(t => t.Project)
            .Where(t => activeProjects.Contains(t.Project!))
            .Where(t => !t.IsArchived && (t.TicketStatusId == "complete"))
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.Type)
            .Include(t => t.Developer)
            .ToListAsync();
    }

    public async Task<List<Ticket>> GetUserCompletedTicketsAsync(string userId)
    {
        List<Ticket> completedTickets = new List<Ticket>();

        AppUser user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return completedTickets;

        List<Project> activeCompanyProjects = await _context.Projects
            .Where(p => p.CompanyId == user.CompanyId && !p.IsArchived)
            .Include(p => p.Tickets)
            .ToListAsync();

        return await _context.Tickets
            .Include(t => t.Project)
            .Where(t => activeCompanyProjects.Contains(t.Project!))
            .Where(t => !t.IsArchived && (t.TicketStatusId == "complete"))
            .Where(t => t.Project!.ProjectManagerId == userId || t.DeveloperId == userId)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.Type)
            .Include(t => t.Developer)
            .ToListAsync();
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
		return await _context.TicketAttachments
			.Include(ta => ta.Ticket)
			.Include(ta => ta.Uploader)
			.FirstOrDefaultAsync(ta => ta.Id == ticketAttachmentId);
	}

	public async Task<bool> ArchiveTicketAsync(Ticket ticket)
	{
		ticket.IsArchived = true;

		try
		{
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
		catch (DbUpdateConcurrencyException)
		{
			return false;
		}
	}

    public async Task<bool> UnarchiveTicketAsync(Ticket ticket)
    {
        ticket.IsArchived = false;

        try
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }

    public async Task UpdateTicketAsync(Ticket ticket)
	{
		_context.Tickets.Update(ticket);
		await _context.SaveChangesAsync();
	}

	public async Task<bool> DeleteTicketAsync(Ticket ticket)
	{
		try
		{
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
		catch (DbUpdateConcurrencyException)
		{
			return false;
		}
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

	public async Task<string> GetTicketTypeDescriptionByIdAsync(string? typeId)
	{
		TicketType? ticketType = await _context.TicketTypes.FirstOrDefaultAsync(t => t.Id == typeId);

		return ticketType is not null ? ticketType.Description : "";
	}

	public async Task<string> GetTicketStatusDescriptionByIdAsync(string? statusId)
	{
		TicketStatus? ticketStatus = await _context.TicketStatuses.FirstOrDefaultAsync(t => t.Id == statusId);

		return ticketStatus is not null ? ticketStatus.Description : "";
	}

	public async Task<string> GetTicketPriorityDescriptionByIdAsync(string? priorityId)
	{
		TicketPriority? ticketPriority = await _context.TicketPriorities.FirstOrDefaultAsync(t => t.Id == priorityId);

		return ticketPriority is not null ? ticketPriority.Description : "";
	}

	public async Task RemoveEmployeeFromAllTicketsAsync(int companyId, string employeeId)
	{
		List<Ticket> tickets = await _context.Tickets.Where(t => t.DeveloperId == employeeId).ToListAsync();

		foreach (Ticket ticket in tickets)
		{
			ticket.DeveloperId = null;
			ticket.TicketStatusId = "unassigned";
		}

		await _context.SaveChangesAsync();
	}
}
