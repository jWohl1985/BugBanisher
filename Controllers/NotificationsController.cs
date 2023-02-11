using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BugBanisher.Models;
using BugBanisher.Services.Interfaces;

namespace BugBanisher.Controllers;

public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly UserManager<AppUser> _userManager;

    public NotificationsController(INotificationService notificationsService, UserManager<AppUser> userManager)
    {
        _notificationService = notificationsService;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public async Task<ViewResult> MyNotifications()
    {
        AppUser? appUser = await _userManager.GetUserAsync(User);
        List<Notification> notifications;

        if (appUser is null)
            notifications = new List<Notification>();
        else
            notifications = await _notificationService.GetAllNotificationsForUserAsync(appUser);

        return View(notifications);
    }

    [HttpGet]
    [Authorize]
    public async Task<RedirectToActionResult> MarkAsRead(int id)
    {
        Notification? notification = await _notificationService.GetByIdAsync(id);

        if (notification is not null)
            await _notificationService.MarkAsRead(notification);

        return RedirectToAction(nameof(MyNotifications));
    }
}
