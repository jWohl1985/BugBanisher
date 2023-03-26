using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BugBanisher.Extensions;
using BugBanisher.Models;
using BugBanisher.Models.Enums;
using BugBanisher.Models.ViewModels;
using BugBanisher.Services.Interfaces;
using System.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BugBanisher.Controllers;

public class CompanyController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICompanyService _companyService;
    private readonly IProjectService _projectService;
    private readonly ITicketService _ticketService;
    private readonly INotificationService _notificationService;
    private readonly IRoleService _roleService;
    private readonly IFileService _fileService;

    public CompanyController(UserManager<AppUser> userManager, 
        ICompanyService companyService,
        IProjectService projectService,
        ITicketService ticketService,
        INotificationService notificationService,
        IRoleService roleService,
        IFileService fileService)
    {
        _userManager = userManager;
        _companyService = companyService;
        _projectService = projectService;
        _ticketService = ticketService;
        _notificationService = notificationService;
        _roleService = roleService;
        _fileService = fileService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ViewResult> ViewEmployee(string userId)
    {
        AppUser? appUser = await _userManager.FindByIdAsync(userId);

        if (appUser is null)
            return View("NotFound");

        if (appUser.CompanyId != User.Identity!.GetCompanyId())
            return View("NotAuthorized");

        return View(appUser);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateEmployeeProfile(AppUser updatedUser)
    {
        AppUser user = await _userManager.GetUserAsync(User);

        if (ModelState["ProfilePicture"] is not null && ModelState["ProfilePicture"]!.ValidationState == ModelValidationState.Invalid)
        {
            ModelState.AddModelError(string.Empty, "Profile picture is too large or the wrong extension type.");
            return View("ViewEmployee", user);
        }

        if (updatedUser.ProfilePicture is not null)
        {
            updatedUser.PictureData = await _fileService.ConvertFileToByteArrayAsync(updatedUser.ProfilePicture);
            updatedUser.PictureExtension = updatedUser.ProfilePicture.ContentType;
        }
        else
        {
            updatedUser.PictureData = user.PictureData;
            updatedUser.PictureExtension = user.PictureExtension;
        }

        await _companyService.UpdateEmployeeProfileAsync(updatedUser);

        return RedirectToAction(nameof(ViewEmployee), new { userId = user.Id });
    }

    [HttpGet]
    [Authorize]
    public async Task<RedirectToActionResult> DeleteProfilePicture(string userId)
    {
        if ((await _userManager.GetUserAsync(User)).Id != userId)
            return RedirectToAction(nameof(NotAuthorized));

        await _companyService.DeleteProfilePictureAsync(userId);

        return RedirectToAction(nameof(ViewEmployee), new { userId = userId });
    }

    [HttpGet]
    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<ViewResult> ManageEmployees()
    {
        int companyId = User.Identity!.GetCompanyId();

        List<AppUser> companyEmployees = await _companyService.GetAllEmployeesAsync(companyId);
        return View(companyEmployees);
    }

    [HttpGet]
    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<ViewResult> EditEmployee(string id)
    {
        AppUser? employeeToEdit = await _userManager.FindByIdAsync(id);

        if (employeeToEdit is null)
            return View("NotFound");

        IEnumerable<IdentityRole> roles = await _roleService.GetRolesAsync();
        string? selectedRole = (await _roleService.GetUserRolesAsync(employeeToEdit)).FirstOrDefault();

        EditEmployeeViewModel viewModel = new()
        {
            Employee = employeeToEdit,
            JobTitle = employeeToEdit.JobTitle,
            AvailableRoles = new SelectList(roles, "Name", "Name", selectedRole),
            SelectedRole = selectedRole,
        };

        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = nameof(Roles.Admin))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEmployee(EditEmployeeViewModel viewModel)
    {
        AppUser? employeeToEdit = await _userManager.FindByIdAsync(viewModel.Employee.Id);

        if (employeeToEdit is null)
            return View("NotFound");

        if (await _userManager.IsInRoleAsync(employeeToEdit, nameof(Roles.Admin)))
        {
            List<AppUser> admins = await _companyService.GetAllAdminsAsync(employeeToEdit.CompanyId!.Value);

            if (admins.Count == 1)
            {
                ViewData["Error"] = "You cannot remove the last administrator for your company! You must designate another user as an administrator first.";
                IEnumerable<IdentityRole> roles = await _roleService.GetRolesAsync();

                EditEmployeeViewModel newViewModel = new()
                {
                    Employee = employeeToEdit,
                    JobTitle = viewModel.JobTitle,
                    AvailableRoles = new SelectList(roles, "Name", "Name", "Admin"),
                    SelectedRole = "Admin",
                };

                return View(newViewModel);
            }
        }

        if (viewModel.JobTitle != employeeToEdit.JobTitle)
        {
            employeeToEdit.JobTitle = viewModel.JobTitle;
        }

        if (viewModel.SelectedRole is not null)
        {
            IEnumerable<string> oldRoles = await _roleService.GetUserRolesAsync(employeeToEdit);
            await _roleService.RemoveUserFromRolesAsync(employeeToEdit, oldRoles);
            await _roleService.AddUserToRoleAsync(employeeToEdit, viewModel.SelectedRole);
        }

        TempData["Message"] = "Employee changes saved!";
        return RedirectToAction(nameof(ManageEmployees));
    }

    [HttpGet]
    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<IActionResult> ConfirmRemoveEmployee(string employeeId)
    {
        AppUser? employeeToRemove = await _userManager.FindByIdAsync(employeeId);

        if (employeeToRemove is null)
            return View("NotFound");

        if (await _userManager.IsInRoleAsync(employeeToRemove, nameof(Roles.Admin)))
        {
            List<AppUser> admins = await _companyService.GetAllAdminsAsync(employeeToRemove.CompanyId!.Value);

            if (admins.Count == 1)
            {
                TempData["Error"] = "You cannot remove the last administrator for your company! You must designate another user as an administrator first.";
                IEnumerable<IdentityRole> roles = await _roleService.GetRolesAsync();

                return RedirectToAction(nameof(ManageEmployees));
            }
        }

        return View(employeeToRemove);
    }

    [HttpPost]
    [Authorize(Roles = nameof(Roles.Admin))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveEmployeeConfirmed([Bind("Id")] AppUser employee)
    {
        int companyId = User.Identity!.GetCompanyId();
        AppUser? userRemoved = await _userManager.FindByIdAsync(employee.Id);

        if (userRemoved is null)
            return View("NotFound");

        await _companyService.RemoveEmployeeAsync(userRemoved.Id);
        await _projectService.RemoveEmployeeFromAllProjectsAsync(companyId, userRemoved.Id);
        await _ticketService.RemoveEmployeeFromAllTicketsAsync(companyId, userRemoved.Id);
        await _notificationService.CreateRemovedFromCompanyNotification(companyId, userRemoved);

        TempData["Message"] = $"{userRemoved.FirstName} {userRemoved.LastName} was successfully removed.";
        return RedirectToAction(nameof(ManageEmployees));
	}

    [HttpGet]
    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<ViewResult> InviteUserToCompany()
    {
        int companyId = User.Identity!.GetCompanyId();
        Company? company = await _companyService.GetCompanyByIdAsync(companyId);
        ViewData["CompanyName"] = company is not null ? company.Name : "your company";
      
        return View(new AppUser());
    }

    [HttpPost]
    [Authorize(Roles = nameof(Roles.Admin))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InviteUserToCompany([Bind("Email")] AppUser user)
    {
        AppUser? receivingUser = await _userManager.FindByEmailAsync(user.Email!);

        AppUser sendingUser = (await _userManager.GetUserAsync(User))!;
        Company? company = await _companyService.GetCompanyByIdAsync(sendingUser.CompanyId!.Value);
        ViewData["CompanyName"] = company is not null ? company.Name : "your company";
        
        ViewData["Errors"] = await GetInviteErrors(sendingUser, receivingUser);

        if (ViewData["Errors"] is not null)
            return View();

        if (!await _notificationService.CreateInvitedToCompanyNotification(sendingUser, receivingUser!))
        {
            ViewData["Errors"] = "Something went wrong, please contact the site administrator.";
            return View();
        }

        ViewData["Message"] = $"{receivingUser!.Email} ({receivingUser.FullName}) has been invited!";
        return View();
    }

    [HttpGet]
    [Authorize]
    public async Task<ViewResult> AcceptCompanyInvite(int notificationId)
    {
        Notification? notification = await _notificationService.GetByIdAsync(notificationId);

        if (notification is null || notification.NotificationTypeId != (int)NotificationType.CompanyInvite)
            return View("NotAuthorized");

        Company? company = await _companyService.GetCompanyByIdAsync(notification.CompanyId);
        AppUser? appUser = await _userManager.GetUserAsync(User);

        if (appUser is null || company is null)
            return View("NotFound");

        IEnumerable<string> currentUserRoles = await _roleService.GetUserRolesAsync(appUser);
        await _roleService.RemoveUserFromRolesAsync(appUser, currentUserRoles);
        
        await _companyService.AddEmployeeAsync(appUser, company.Id);
		await _roleService.AddUserToRoleAsync(appUser, nameof(Roles.Member));
		await _notificationService.CreateInviteAcceptedNotification(notification.Id, appUser);

        ViewData["CompanyName"] = $"{company.Name}";

        return View();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> RejectCompanyInvite(int notificationId)
    {
        Notification? notification = await _notificationService.GetByIdAsync(notificationId);

        if (notification is null || notification.NotificationTypeId != (int)NotificationType.CompanyInvite)
            return View("NotAuthorized");

        Company? company = await _companyService.GetCompanyByIdAsync(notification.CompanyId);
        AppUser? appUser = await _userManager.GetUserAsync(User);

        if (appUser is null || company is null)
            return View("NotFound");

        notification.HasBeenSeen = true;
        await _notificationService.CreateInviteRejectedNotification(notification.Id, appUser);

        ViewData["CompanyName"] = $"{company.Name}";

        return RedirectToAction(nameof(NotificationsController.MyNotifications), "Notifications");
    }

    [HttpGet]
    public ViewResult NotAuthorized()
    {
        return View();
    }

	#region Private Helper Methods
	private async Task<string?> GetInviteErrors(AppUser sendingUser, AppUser? receivingUser)
    {
        if (receivingUser is null)
            return "Could not find a user with that e-mail address.";

        if (receivingUser.CompanyId == sendingUser.CompanyId)
            return "That user has already joined your company!";

        if (receivingUser.CompanyId is not null)
            return "That user has already joined a company.";

        List<Notification> existingNotifications = await _notificationService.GetUnseenNotificationsForUserAsync(receivingUser);

        if (existingNotifications
            .Where(n => (n.CompanyId == sendingUser.CompanyId) && (n.NotificationType == NotificationType.CompanyInvite))
            .Any())
            return "That user already has a pending invite to your company.";

        return null;
    }
    #endregion
}