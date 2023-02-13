using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BugBanisher.Database;
using BugBanisher.Models;
using BugBanisher.Models.Enums;
using BugBanisher.Services.Interfaces;

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
        return await _context.Notifications.Where(n => n.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Notification>> GetUnseenNotificationsForUserAsync(AppUser appUser)
    {
        return await _context.Notifications.Where(n => n.AppUserId == appUser.Id)
            .Where(n => n.HasBeenSeen == false)
            .OrderByDescending(n => n.Created)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetAllNotificationsForUserAsync(AppUser appUser)
    {
        return await _context.Notifications.Where(n => n.AppUserId == appUser.Id)
            .OrderByDescending(n => n.Created)
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
            Title = $"New invite from {sendingUser.FullName}",
            Message = $"{sendingUser.FullName} has invited you to join their company.",
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
            CompanyId = originalInvite.CompanyId,
            Created = DateTime.Now,
            Title = $"{invitee.FullName} joined your company",
            Message = $"{invitee.FullName} has accepted the invite to join your company. You can now assign them a role and job title. " +
            $"Visit the Company\\Manage Team Members page to do so.",
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
            Title = $"{invitee.FullName} declined your invite",
            Message = $"{invitee.FullName} declined your invite to join your company.",
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

    public async Task<bool> CreateNewTicketNotificationAsync(string developerId, Ticket ticket)
    {
        AppUser? developer = await _context.Users.FirstOrDefaultAsync(u => u.Id == developerId);
        Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == ticket.ProjectId);

        if (developer is null || project is null)
            return false;

        Notification newTicketNotification = new Notification
        {
            AppUserId = developerId,
            CompanyId = developer.CompanyId!.Value,
            Created = DateTime.Now,
            Title = $"You've been assigned a new ticket.",
            Message = $"You have a new ticket to accept on the {project.Name} project.\n\n" + $"Ticket Title: {ticket.Title}\n\n",
            NotificationTypeId = (int)NotificationType.NewTicket,
        };

        await _context.Notifications.AddAsync(newTicketNotification);
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
            Message = $"Hopefully we're not the ones breaking the news to you, but you were removed from your company. On the bright side, " +
            $"you can now be invited to a different one.",
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
