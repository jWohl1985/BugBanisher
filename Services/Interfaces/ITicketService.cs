﻿using BugBanisher.Models;

namespace BugBanisher.Services.Interfaces;

public interface ITicketService
{
	Task<Ticket?> GetTicketByIdAsync(int ticketId);
	Task<int> CreateTicketAsync(Ticket ticket);
	Task<List<Ticket>> GetAllOpenCompanyTicketsAsync(int companyId);
	Task<List<Ticket>> GetUserOpenTicketsAsync(string userId);
	Task<List<Ticket>> GetAllActionRequiredTicketsAsync(int companyId);
	Task<List<Ticket>> GetUserActionRequiredTicketsAsync(string userId);
	Task<List<Ticket>> GetCompletedTicketsAsync(int companyId);
	Task<List<Ticket>> GetUserCompletedTicketsAsync(string userId);
	Task<List<TicketPriority>> GetTicketPrioritiesAsync();
	Task<List<TicketStatus>> GetTicketStatusesAsync();
	Task<List<TicketType>> GetTicketTypesAsync();
	Task<TicketAttachment?> GetTicketAttachmentByIdAsync(int ticketAttachmentId);
	Task<bool> ArchiveTicketAsync(Ticket ticket);
	Task<bool> UnarchiveTicketAsync(Ticket ticket);
	Task UpdateTicketAsync(Ticket ticket);
	Task<bool> DeleteTicketAsync(Ticket ticket);
	Task<bool> AddTicketCommentAsync(int ticketId, TicketComment comment);
	Task<bool> AddTicketAttachmentAsync(int ticketId, TicketAttachment attachment);
	Task<string> GetTicketTypeDescriptionByIdAsync(string? typeId);
	Task<string> GetTicketStatusDescriptionByIdAsync(string? statusId);
	Task<string> GetTicketPriorityDescriptionByIdAsync(string? priorityId);
	Task RemoveEmployeeFromAllTicketsAsync(int companyId, string employeeId);
}
