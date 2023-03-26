using BugBanisher.Models;

namespace BugBanisher.Services.Interfaces;

public interface INotificationService
{
    Task<Notification?> GetByIdAsync(int id);
    Task<List<Notification>> GetUnseenNotificationsForUserAsync(AppUser appUser);
    Task<List<Notification>> GetAllNotificationsForUserAsync(AppUser appUser);
    Task MarkAsRead(Notification notification);
    Task<bool> CreateInvitedToCompanyNotification(AppUser sendingUser, AppUser receivingUser);
    Task<bool> CreateInviteAcceptedNotification(int originalInviteId, AppUser invitee);
    Task<bool> CreateInviteRejectedNotification(int originalInviteId, AppUser invitee);
    Task<bool> CreateNewProjectNotificationAsync(string userId, Project project);
    Task<bool> CreateNewTicketNotificationAsync(string developerId, int ticketId);
    Task<bool> CreateMentionNotificationAsync(string mentionedUserId, string commentingUserId, int ticketId);
    Task<bool> CreateRemovedFromCompanyNotification(int companyId, AppUser employee);
}
