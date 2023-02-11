﻿using System.Diagnostics;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BugBanisher.Extensions;
using BugBanisher.Models;
using BugBanisher.Models.Enums;
using BugBanisher.Models.ViewModels;
using BugBanisher.Services.Interfaces;

namespace BugBanisher.Controllers;

public class CompanyController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICompanyService _companyService;
    private readonly IProjectService _projectService;
    private readonly INotificationService _notificationService;
    private readonly IRoleService _roleService;

    public CompanyController(UserManager<AppUser> userManager, 
        ICompanyService companyService,
        IProjectService projectService,
        INotificationService notificationService,
        IRoleService roleService)
    {
        _userManager = userManager;
        _companyService = companyService;
        _projectService = projectService;
        _notificationService = notificationService;
        _roleService = roleService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ViewResult> ManageTeamMembers()
    {
        int companyId = User.Identity!.GetCompanyId();

        List<AppUser> companyEmployees = await _companyService.GetAllEmployeesAsync(companyId);
        return View(companyEmployees);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> EditEmployee(string id)
    {
        AppUser? employeeToEdit = await _userManager.FindByIdAsync(id);

        if (employeeToEdit is null)
            return NotFound();

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
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEmployee(EditEmployeeViewModel viewModel)
    {
        AppUser? employeeToEdit = await _userManager.FindByIdAsync(viewModel.Employee.Id);

        if (employeeToEdit is null)
            return NotFound();

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

        viewModel.AvailableRoles = new SelectList(await _roleService.GetRolesAsync(), "Name", "Name", viewModel.SelectedRole);

        TempData["Message"] = "Employee changes saved!";
        return RedirectToAction(nameof(ManageTeamMembers));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ConfirmRemoveEmployee(string employeeId)
    {
        AppUser? employee = await _userManager.FindByIdAsync(employeeId);

        if (employee is null)
            return NotFound();

        return View(employee);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveEmployeeConfirmed([Bind("Id")] AppUser employee)
    {
        int companyId = User.Identity!.GetCompanyId();
        AppUser? userRemoved = await _userManager.FindByIdAsync(employee.Id);

        if (userRemoved is null)
            return NotFound();

        await _companyService.RemoveEmployeeAsync(userRemoved.Id);
        await _projectService.RemoveEmployeeFromAllActiveProjectsAsync(companyId, userRemoved.Id);
        await _notificationService.CreateRemovedFromCompanyNotification(companyId, userRemoved);

        TempData["Message"] = $"{userRemoved.FirstName} {userRemoved.LastName} was successfully removed.";
        return RedirectToAction(nameof(ManageTeamMembers));
	}

    [HttpGet]
    [Authorize]
    public async Task<ViewResult> InviteUserToCompany()
    {
        int companyId = User.Identity!.GetCompanyId();
        Company? company = await _companyService.GetCompanyByIdAsync(companyId);
        ViewData["CompanyName"] = company is not null ? company.Name : "your company";
      
        return View(new AppUser());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<ViewResult> InviteUserToCompany([Bind("Email")] AppUser user)
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
    public async Task<IActionResult> AcceptCompanyInvite(int notificationId)
    {
        Notification? notification = await _notificationService.GetByIdAsync(notificationId);

        if (notification is null || notification.NotificationTypeId != (int)NotificationType.CompanyInvite)
            return Problem("Invalid invite.");

        Company? company = await _companyService.GetCompanyByIdAsync(notification.CompanyId);
        AppUser? appUser = await _userManager.GetUserAsync(User);

        if (appUser is null || company is null)
            return Problem("Invalid invite. The user or company does not exist anymore.");

        IEnumerable<string> currentUserRoles = await _roleService.GetUserRolesAsync(appUser);
        await _roleService.RemoveUserFromRolesAsync(appUser, currentUserRoles);
        
        appUser.CompanyId = company.Id;
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
            return Problem("Invalid invite.");

        Company? company = await _companyService.GetCompanyByIdAsync(notification.CompanyId);
        AppUser? appUser = await _userManager.GetUserAsync(User);

        if (appUser is null || company is null)
            return Problem("Invalid invite. The user or company does not exist anymore.");

        notification.HasBeenSeen = true;
        notification.Message = notification.Message + " (You declined)";
        await _notificationService.CreateInviteRejectedNotification(notification.Id, appUser);

        ViewData["CompanyName"] = $"{company.Name}";

        return RedirectToAction(nameof(NotificationsController.MyNotifications), "Notifications");
    }

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
}