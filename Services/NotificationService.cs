using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BugBanisher.Database;
using BugBanisher.Models;
using BugBanisher.Models.Enums;
using BugBanisher.Services.Interfaces;
using System.Security.Policy;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BugBanisher.Services;

public class NotificationService : INotificationService
{
    private readonly BugBanisherContext _context;

    public NotificationService(BugBanisherContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(int id)
    {
        return await _context.Notifications
            .Where(n => n.Id == id)
            .Include(n => n.Ticket)
            .Include(n => n.Project)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Notification>> GetUnseenNotificationsForUserAsync(AppUser appUser)
    {
        return await _context.Notifications.Where(n => n.AppUserId == appUser.Id)
            .Where(n => n.HasBeenSeen == false)
            .OrderByDescending(n => n.Created)
            .Include(n => n.Ticket)
            .Include(n => n.Project)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetAllNotificationsForUserAsync(AppUser appUser)
    {
        return await _context.Notifications.Where(n => n.AppUserId == appUser.Id)
            .OrderByDescending(n => n.Created)
            .Include(n => n.Ticket)
            .Include(n => n.Project)
            .ToListAsync();
    }

    public async Task MarkAsRead(Notification notification)
    {
        notification.HasBeenSeen = true;

        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CreateInvitedToCompanyNotification(AppUser sendingUser, AppUser receivingUser)
    {
        Notification invitedToCompanyNotification = new Notification
        {
            AppUserId = receivingUser.Id,
            SenderId = sendingUser.Id,
            CompanyId = (sendingUser.CompanyId!).Value,
            Created = DateTime.Now,
            Title = $"New invite",
            PreviewText = "You've been invited to join a company.",
            NotificationTypeId = (int)NotificationType.CompanyInvite,
        };

        try
        {
            await _context.Notifications.AddAsync(invitedToCompanyNotification);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CreateInviteAcceptedNotification(int originalInviteId, AppUser invitee)
    {
        Notification? originalInvite = await GetByIdAsync(originalInviteId);

        if (originalInvite is null)
            return false;

        Notification inviteAcceptedNotification = new Notification
        {
            AppUserId = originalInvite.SenderId!,
            SenderId = originalInvite.AppUserId,
            CompanyId = originalInvite.CompanyId,
            Created = DateTime.Now,
            Title = $"Invite accepted",
            PreviewText = "Your company has a new employee.",
            NotificationTypeId = (int)NotificationType.CompanyInviteAccepted,
        };

        try
        {
            await _context.Notifications.AddAsync(inviteAcceptedNotification);
            await RemoveAllCompanyInvitesForUserAsync(invitee); // user shouldn't be able to join another company now
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CreateInviteRejectedNotification(int originalInviteId, AppUser invitee)
    {
        Notification? originalInvite = await GetByIdAsync(originalInviteId);

        if (originalInvite is null)
            return false;

        Notification inviteRejectedNotification = new Notification
        {
            AppUserId = originalInvite.SenderId!,
            CompanyId = originalInvite.CompanyId,
            Created = DateTime.Now,
            Title = $"Invite declined",
            PreviewText = "A user declined your company invite.",
            NotificationTypeId = (int)NotificationType.CompanyInviteRejected,
        };

        try
        {
            await _context.Notifications.AddAsync(inviteRejectedNotification);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

	public async Task<bool> CreateNewProjectNotificationAsync(string userId, Project project)
	{
		AppUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

		if (user is null)
			return false;

		Notification newProjectNotification = new Notification
		{
			AppUserId = userId,
			CompanyId = user.CompanyId!.Value,
			Created = DateTime.Now,
            ProjectId = project.Id,
			Title = $"New project",
            PreviewText = "You have a new project to work on.",
			NotificationTypeId = (int)NotificationType.NewProject,
		};

		await _context.Notifications.AddAsync(newProjectNotification);
		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<bool> CreateNewTicketNotificationAsync(string developerId, int ticketId)
    {
        AppUser? developer = await _context.Users.FirstOrDefaultAsync(u => u.Id == developerId);
        Ticket? ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

        if (developer is null || ticket is null)
            return false;

        Notification newTicketNotification = new Notification
        {
            AppUserId = developerId,
            CompanyId = developer.CompanyId!.Value,
            Created = DateTime.Now,
            ProjectId = ticket.ProjectId,
            TicketId = ticket.Id,
            Title = $"New ticket",
            PreviewText = "You have a new ticket to work on.",
            NotificationTypeId = (int)NotificationType.NewTicket,
        };

        await _context.Notifications.AddAsync(newTicketNotification);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CreateMentionNotificationAsync(string mentionedUserId, string commentingUserId, int ticketId)
    {
        AppUser? mentionedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == mentionedUserId);
        AppUser? commentingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == commentingUserId);
        Ticket? ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

        if (mentionedUser is null || commentingUser is null || ticket is null)
            return false;

        Notification newMentionNotification = new Notification
        {
            AppUserId = mentionedUserId,
            CompanyId = commentingUser.CompanyId!.Value,
            SenderId = commentingUserId,
            TicketId = ticketId,
            ProjectId = ticket.ProjectId,
            Created = DateTime.Now,
            Title = $"New comment",
            PreviewText = "Someone mentioned you in a comment.",
            NotificationTypeId = (int)NotificationType.Mention,
        };

        await _context.Notifications.AddAsync(newMentionNotification);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task RemoveAllCompanyInvitesForUserAsync(AppUser appUser)
    {
        List<Notification> companyInvites = await _context.Notifications
            .Where(n => n.AppUserId == appUser.Id && n.NotificationTypeId == (int)NotificationType.CompanyInvite)
            .ToListAsync();

        foreach (Notification notification in companyInvites)
        {
            _context.Notifications.Remove(notification);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> CreateRemovedFromCompanyNotification(int companyId, AppUser employee)
    {
        Notification removedFromCompanyNotification = new()
        {
            AppUserId = employee.Id,
            CompanyId = companyId,
            Created = DateTime.Now,
            Title = $"You were removed from your company.",
            PreviewText = "You were removed from your company.",
            NotificationTypeId = (int)NotificationType.RemovedFromCompany,
        };

        try
        {
			await _context.Notifications.AddAsync(removedFromCompanyNotification);
            await _context.SaveChangesAsync();
            return true;
		}
        catch
        {
            return false;
        }
    }
}
