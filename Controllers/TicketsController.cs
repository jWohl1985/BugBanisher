using BugBanisher.Models;
using BugBanisher.Models.ViewModels;
using BugBanisher.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugBanisher.Controllers;



public class TicketsController : Controller
{
	private readonly IProjectService _projectService;
	private readonly ITicketService _ticketService;
	private readonly UserManager<AppUser> _userManager;

	public TicketsController(IProjectService projectService, ITicketService ticketService, UserManager<AppUser> userManager)
	{
		_projectService = projectService;
		_ticketService = ticketService;
		_userManager = userManager;
	}

	[HttpGet]
	[Authorize(Roles = "Admin, ProjectManager, Developer")]
	public async Task<ViewResult> CreateTicket(int projectId)
	{
		ViewData["Action"] = "Create";

		CreateOrEditTicketViewModel viewModel = await GenerateCreateTicketViewModel(projectId);

		return View("CreateOrEditTicket", viewModel);
	}

	[HttpPost]
	[Authorize(Roles = "Admin, ProjectManager, Developer")]
	[ValidateAntiForgeryToken]
	public async Task<RedirectToActionResult> CreateTicket(CreateOrEditTicketViewModel viewModel)
	{
		ViewData["Action"] = "Create";

		Ticket ticket = viewModel.Ticket;
		ticket.Title = viewModel.Title;
		ticket.Description = viewModel.Description;
		ticket.Created = DateTime.Now;
		ticket.DeveloperId = viewModel.SelectedDeveloper;
		ticket.TicketTypeId = viewModel.SelectedType!;
		ticket.TicketPriorityId = viewModel.SelectedPriority!;

		ticket.TicketStatusId = ticket.DeveloperId is not null ? "pending" : "unassigned";

		ticket.Id = await _ticketService.CreateTicketAsync(ticket);

		return RedirectToAction("Index", "Home");
	}

	[HttpGet]
	[Authorize(Roles = "Admin, ProjectManager, Developer")]
	public async Task<IActionResult> EditTicket(int ticketId)
	{
		ViewData["Action"] = "Edit";

		Ticket? ticket = await _ticketService.GetTicketByIdAsync(ticketId);

		if (ticket is null)
			return NotFound();

		CreateOrEditTicketViewModel viewModel = await GenerateEditTicketViewModel(ticket);

		return View("CreateOrEditTicket", viewModel);
	}

    [HttpPost]
    [Authorize(Roles = "Admin, ProjectManager, Developer")]
	[ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTicket(CreateOrEditTicketViewModel viewModel)
    {
        ViewData["Action"] = "Edit";

		Ticket ticket = viewModel.Ticket;

		ticket.Title = viewModel.Title;
		ticket.Description = viewModel.Description;
		ticket.TicketTypeId = viewModel.SelectedType!;
		ticket.TicketPriorityId = viewModel.SelectedPriority!;
		ticket.Updated = DateTime.Now;

		if (viewModel.SelectedStatus is not null)
            ticket.TicketStatusId = viewModel.SelectedStatus;

		if (viewModel.SelectedDeveloper is null)
		{
            ticket.DeveloperId = null;
			ticket.TicketStatusId = "unassigned";
		}
		else
		{
            ticket.DeveloperId = viewModel.SelectedDeveloper;

			if (ticket.TicketStatusId == "unassigned")
				ticket.TicketStatusId = "pending";
        }

        await _ticketService.UpdateTicketAsync(ticket);

		return RedirectToAction("Index", "Home");
    }



    #region Private helper methods

    private async Task<CreateOrEditTicketViewModel> GenerateCreateTicketViewModel(int projectId)
	{
		AppUser appUser = await _userManager.GetUserAsync(User);

		Ticket ticket = new Ticket()
		{
			ProjectId = projectId,
			CreatorId = appUser.Id,
		};

		CreateOrEditTicketViewModel viewModel = new CreateOrEditTicketViewModel
		{
			Ticket = ticket,
			Developers = new SelectList(await _projectService.GetDevelopersOnProjectAsync(ticket.ProjectId), "Id", "FullName", "Unassigned"),
			Priorities = new SelectList(await _ticketService.GetTicketPrioritiesAsync(), "Id", "Description", "medium"),
			Types = new SelectList(await _ticketService.GetTicketTypesAsync(), "Id", "Description", "bug"),
			Statuses = new SelectList(await _ticketService.GetTicketStatusesAsync(), "Id", "Description", "unassigned"),
		};

		return viewModel;
	}

	private async Task<CreateOrEditTicketViewModel> GenerateEditTicketViewModel(Ticket ticket)
	{
		AppUser appUser = await _userManager.GetUserAsync(User);

		CreateOrEditTicketViewModel viewModel = new CreateOrEditTicketViewModel()
		{
			Ticket = ticket,
			Title = ticket.Title,
			Description = ticket.Description,
			Developers = new SelectList(await _projectService.GetDevelopersOnProjectAsync(ticket.ProjectId), "Id", "FullName", ticket.DeveloperId),
			Priorities = new SelectList(await _ticketService.GetTicketPrioritiesAsync(), "Id", "Description", ticket.TicketPriorityId),
			Types = new SelectList(await _ticketService.GetTicketTypesAsync(), "Id", "Description", ticket.TicketTypeId),
			Statuses = new SelectList(await _ticketService.GetTicketStatusesAsync(), "Id", "Description", ticket.TicketStatusId),
			IsStatusDropdownEnabled = appUser.Id == ticket.DeveloperId ? true : false,
        };

		return viewModel;
	}

	#endregion




}
