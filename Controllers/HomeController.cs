using BugBanisher.Extensions;
using BugBanisher.Models;
using BugBanisher.Models.Enums;
using BugBanisher.Models.ViewModels;
using BugBanisher.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace BugBanisher.Controllers;

public class HomeController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    private readonly ICompanyService _companyService;
    private readonly IProjectService _projectService;
    private readonly ITicketService _ticketService;


    public HomeController(
        SignInManager<AppUser> signInManager, 
        UserManager<AppUser> userManager, 
        ICompanyService companyService, 
        IProjectService projectService, 
        ITicketService ticketService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _companyService = companyService;
        _projectService = projectService;
        _ticketService = ticketService;
    }

    public async Task<IActionResult> Index()
    {
        if (!_signInManager.IsSignedIn(User))
            return Redirect("/Identity/Account/Login");

        DashboardViewModel dashboardViewModel;

        if (User.IsInRole(nameof(Roles.Admin)))
            dashboardViewModel = await GenerateAdminDashboard();

        else
            dashboardViewModel = await GenerateNonAdminDashboard();

        return View(dashboardViewModel);
    }

    private async Task<DashboardViewModel> GenerateAdminDashboard()
    {
        int companyId = User.Identity!.GetCompanyId();

        List<Project> projects = await _projectService.GetAllActiveCompanyProjectsAsync(companyId);
        List<Ticket> tickets = await _ticketService.GetAllOpenCompanyTicketsAsync(companyId);
        List<AppUser> employees = await _companyService.GetAllEmployeesAsync(companyId);

        int adminCount = (await _companyService.GetAllAdminsAsync(companyId)).Count();
        int projectManagerCount = (await _companyService.GetAllProjectManagersAsync(companyId)).Count();
        int developerCount = (await _companyService.GetAllDevelopersAsync(companyId)).Count();
        int memberCount = (await _companyService.GetAllMembersAsync(companyId)).Count();

        return new DashboardViewModel()
        {
            ActiveProjects = projects,
            OpenTickets = tickets,
            Employees = employees,
            AdminCount = adminCount,
            ProjectManagerCount = projectManagerCount,
            DeveloperCount = developerCount,
            MemberCount = memberCount,
        };
    }

    private async Task<DashboardViewModel> GenerateNonAdminDashboard()
    {
        AppUser user = await _userManager.GetUserAsync(User);

        int companyId = User.Identity!.GetCompanyId();

        List<Project> projects = await _projectService.GetProjectsByProjectManagerAsync(user.Id);
        List<Ticket> tickets = await _ticketService.GetUserOpenTicketsAsync(user.Id);
        List<AppUser> employees = await _companyService.GetAllEmployeesAsync(companyId);

        return new DashboardViewModel()
        {
            ActiveProjects = projects,
            OpenTickets = tickets,
            Employees = employees,
        };
    }
}