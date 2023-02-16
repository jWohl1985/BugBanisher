﻿using BugBanisher.Models;

namespace BugBanisher.Services.Interfaces;

public interface ITicketService
{
	Task<Ticket?> GetTicketByIdAsync(int ticketId);
	Task<int> CreateTicketAsync(Ticket ticket);
	Task<List<Ticket>> GetAllActiveCompanyTicketsAsync(int companyId);
	Task<List<Ticket>> GetUserActiveTicketsAsync(string userId);
	Task<List<Ticket>> GetAllProblemTicketsAsync(int companyId);
	Task<List<Ticket>> GetUserProblemTicketsAsync(string userId);
	Task<List<TicketPriority>> GetTicketPrioritiesAsync();
	Task<List<TicketStatus>> GetTicketStatusesAsync();
	Task<List<TicketType>> GetTicketTypesAsync();
	Task<TicketAttachment?> GetTicketAttachmentByIdAsync(int ticketAttachmentId);
	Task UpdateTicketAsync(Ticket ticket);
	Task<bool> AddTicketCommentAsync(int ticketId, TicketComment comment);
	Task<bool> AddTicketAttachmentAsync(int ticketId, TicketAttachment attachment);
}
