﻿using BugBanisher.Models;
using BugBanisher.Models.ViewModels;
using BugBanisher.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography.X509Certificates;
using BugBanisher.Extensions;
using BugBanisher.Models.Enums;
using System.Globalization;
using System.Net.Sockets;

namespace BugBanisher.Controllers;

public class TicketsController : Controller
{
	private readonly IProjectService _projectService;
	private readonly ITicketService _ticketService;
	private readonly IFileService _fileService;
	private readonly INotificationService _notificationService;
	private readonly ITicketHistoryService _ticketHistoryService;
	private readonly UserManager<AppUser> _userManager;

	private bool UserIsAdmin => User.IsInRole(nameof(Roles.Admin));

	public TicketsController(IProjectService projectService, 
		ITicketService ticketService, 
		IFileService fileService,
		INotificationService notificationService,
		ITicketHistoryService ticketHistoryService,
		UserManager<AppUser> userManager)
	{
		_projectService = projectService;
		_ticketService = ticketService;
		_fileService = fileService;
		_notificationService = notificationService;
		_ticketHistoryService = ticketHistoryService;
		_userManager = userManager;
	}

	[HttpGet]
	[Authorize]
	public async Task<ViewResult> ViewTicket(int ticketId)
	{
		Ticket? ticket = await _ticketService.GetTicketByIdAsync(ticketId);
		
		if (ticket is null)
			return View(nameof(NotFound));

        Project? project = await _projectService.GetProjectByIdAsync(ticket.ProjectId);
		AppUser? user = await _userManager.GetUserAsync(User);

		if (project is null || user is null)
			return View(nameof(NotFound));

		if (!UserIsAdmin && !project.Team.Contains(user))
			return View(nameof(NotAuthorized));

        TicketViewModel viewModel = new TicketViewModel
		{
			Ticket = ticket,
			Project = ticket.Project!,
			ProjectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId),
			Developer = ticket.Developer,
		};

		return View(viewModel);
	}

	[HttpGet]
	[Authorize]
	public async Task<ViewResult> ListOpenTickets(string sortBy = "Title", int pageNumber = 1, int perPage = 10)
	{
        int companyId = User.Identity!.GetCompanyId();
        AppUser user = await _userManager.GetUserAsync(User);

        List<Ticket> activeTickets =
            UserIsAdmin ? await _ticketService.GetAllOpenCompanyTicketsAsync(companyId) : await _ticketService.GetUserOpenTicketsAsync(user.Id);

        TicketListViewModel viewModel = new TicketListViewModel()
        {
            TicketListType = "Open",
            Tickets = activeTickets,
            PageNumber = pageNumber.ToString(),
            PerPage = perPage.ToString(),
			SortBy = sortBy,

            SortByOptions = new SelectList(new string[] { "Title", "Project", "Assigned", "Priority", "Type", "Status" }, sortBy),
            PerPageOptions = new SelectList(new string[] { "5", "10", "20", "30", "40", "50" }, perPage.ToString()),
        };

        return View("List", viewModel);
	}

    [HttpGet]
    [Authorize]
    public async Task<ViewResult> ListActionRequiredTickets(string sortBy = "Project Name", int pageNumber = 1, int perPage = 10)
    {
        int companyId = User.Identity!.GetCompanyId();
        AppUser user = await _userManager.GetUserAsync(User);
        
        List<Ticket> actionRequiredTickets =
            UserIsAdmin ? await _ticketService.GetAllActionRequiredTicketsAsync(companyId) : await _ticketService.GetUserActionRequiredTicketsAsync(user.Id);

        TicketListViewModel viewModel = new TicketListViewModel()
        {
            TicketListType = "Action Required",
            Tickets = actionRequiredTickets,
            PageNumber = pageNumber.ToString(),
            PerPage = perPage.ToString(),
            SortBy = sortBy,

            SortByOptions = new SelectList(new string[] { "Project Name", "Assigned", "Priority", "Type", "Status" }, sortBy),
            PerPageOptions = new SelectList(new string[] { "5", "10", "20", "30", "40", "50" }, perPage.ToString()),
        };

        return View("List", viewModel);
    }

    [HttpGet]
    [Authorize]
    public async Task<ViewResult> ListCompletedTickets(string sortBy = "Project Name", int pageNumber = 1, int perPage = 10)
    {
        int companyId = User.Identity!.GetCompanyId();
        AppUser user = await _userManager.GetUserAsync(User);

        List<Ticket> activeTickets =
            UserIsAdmin ? await _ticketService.GetCompletedTicketsAsync(companyId) : await _ticketService.GetUserCompletedTicketsAsync(user.Id);

        TicketListViewModel viewModel = new TicketListViewModel()
        {
            TicketListType = "Completed",
            Tickets = activeTickets,
            PageNumber = pageNumber.ToString(),
            PerPage = perPage.ToString(),
            SortBy = sortBy,

            SortByOptions = new SelectList(new string[] { "Project Name", "Assigned", "Priority", "Type", "Status" }, sortBy),
            PerPageOptions = new SelectList(new string[] { "5", "10", "20", "30", "40", "50" }, perPage.ToString()),
        };

        return View("List", viewModel);
    }

    [HttpGet]
	[Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}, {nameof(Roles.Developer)}")]
	public async Task<ViewResult> CreateTicket(int projectId)
	{
		ViewData["Action"] = "Create";

		Project? project = await _projectService.GetProjectByIdAsync(projectId);
		AppUser? user = await _userManager.GetUserAsync(User);

        if (project is null || user is null)
            return View(nameof(NotFound));

        if (!UserIsAdmin && !project.Team.Contains(user))
            return View(nameof(NotAuthorized));

        CreateOrEditTicketViewModel viewModel = await GenerateCreateTicketViewModel(projectId);

		return View("CreateOrEditTicket", viewModel);
	}

	[HttpPost]
	[Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}, {nameof(Roles.Developer)}")]
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

		await _ticketHistoryService.AddTicketCreatedEventAsync(ticket.Id);

        if (ticket.DeveloperId is not null)
            await _notificationService.CreateNewTicketNotificationAsync(ticket.DeveloperId, ticket.Id);

        return RedirectToAction("ViewTicket", new { ticketId = ticket.Id });
	}

	[HttpGet]
	[Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}, {nameof(Roles.Developer)}")]
	public async Task<ViewResult> EditTicket(int ticketId)
	{
		ViewData["Action"] = "Edit";

		Ticket? ticket = await _ticketService.GetTicketByIdAsync(ticketId);

		if (ticket is null)
			return View(nameof(NotFound));

        Project? project = await _projectService.GetProjectByIdAsync(ticket.ProjectId);
        AppUser? user = await _userManager.GetUserAsync(User);

        if (project is null || user is null)
            return View(nameof(NotFound));

        if (!UserIsAdmin && !project.Team.Contains(user))
            return View(nameof(NotAuthorized));

		if (User.IsInRole(nameof(Roles.Developer)) && ticket.DeveloperId != user.Id)
			return View(nameof(NotAuthorized));

        CreateOrEditTicketViewModel viewModel = await GenerateEditTicketViewModel(ticket);

		return View("CreateOrEditTicket", viewModel);
	}

    [HttpPost]
    [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}, {nameof(Roles.Developer)}")]
	[ValidateAntiForgeryToken]
    public async Task<RedirectToActionResult> EditTicket(CreateOrEditTicketViewModel viewModel)
    {
        ViewData["Action"] = "Edit";

		Ticket ticket = viewModel.Ticket;

        bool aPropertyWasChanged = DetermineIfTicketChangesWereMade(viewModel);

		if (aPropertyWasChanged)
		{
            TicketHistory historyItem = await CreateTicketChangeHistory(viewModel);

			ticket.Title = viewModel.Title;
			ticket.Description = viewModel.Description;
			ticket.TicketStatusId = viewModel.SelectedStatus!;
			ticket.TicketPriorityId = viewModel.SelectedPriority!;
			ticket.TicketTypeId = viewModel.SelectedType!;
			ticket.Updated = DateTime.Now;

			if (viewModel.SelectedDeveloper is null)
			{
				ticket.DeveloperId = null;
				ticket.TicketStatusId = "unassigned";
			}

			if (ticket.DeveloperId != viewModel.SelectedDeveloper && viewModel.SelectedDeveloper is not null)
			{
				ticket.TicketStatusId = "pending";
				ticket.DeveloperId = viewModel.SelectedDeveloper;
				await _notificationService.CreateNewTicketNotificationAsync(ticket.DeveloperId, ticket.Id);
			}

			await _ticketService.UpdateTicketAsync(ticket);
			await _ticketHistoryService.AddTicketHistoryItemAsync(historyItem);
        }
        
		return RedirectToAction("ViewTicket", new { ticketId = ticket.Id });
    }

	[HttpPost]
	[Authorize]
	[ValidateAntiForgeryToken]
	public async Task<RedirectResult> AddComment(TicketViewModel viewModel)
	{
		if (string.IsNullOrEmpty(viewModel.NewComment))
		{
			TempData["CommentError"] = "Comment cannot be empty.";
			return Redirect(Url.RouteUrl(new { controller = "Tickets", action = "ViewTicket", ticketId = viewModel.Ticket.Id }) + "#comments");
		}

        AppUser user = await _userManager.GetUserAsync(User);
		Project project = (await _projectService.GetProjectByIdAsync(viewModel.Ticket.ProjectId))!;

        TicketComment comment = new TicketComment()
		{
			TicketId = viewModel.Ticket.Id,
			AppUserId = user.Id,
			Created = DateTime.Now,
			Comment = viewModel.NewComment,
		};

		await _ticketService.AddTicketCommentAsync(viewModel.Ticket.Id, comment);

		foreach (AppUser teamMember in project.Team)
		{
			if (comment.Comment.Contains($"@{teamMember.FullName}"))
				await _notificationService.CreateMentionNotificationAsync(teamMember.Id, user.Id, viewModel.Ticket.Id);
		}

		return Redirect(Url.RouteUrl(new { controller = "Tickets", action = "ViewTicket", ticketId = viewModel.Ticket.Id }) + "#comments");
	}

	[HttpPost]
	[Authorize]
	[ValidateAntiForgeryToken]
	public async Task<RedirectResult> AddAttachment(TicketViewModel viewModel)
	{
		AppUser user = await _userManager.GetUserAsync(User);

		if (viewModel.NewAttachment is null)
		{
			TempData["AttachmentError"] = "Please choose a file to add.";
			return Redirect(Url.RouteUrl(new { controller = "Tickets", action = "ViewTicket", ticketId = viewModel.Ticket.Id }) + "#attachments");
		}

		if (viewModel.FileDescription is null)
		{
			TempData["AttachmentError"] = "Please provide a brief file description.";
			return Redirect(Url.RouteUrl(new { controller = "Tickets", action = "ViewTicket", ticketId = viewModel.Ticket.Id }) + "#attachments");
		}

		TicketAttachment attachment = new TicketAttachment()
		{
			TicketId = viewModel.Ticket.Id,
			AppUserId = user.Id,
			Created = DateTime.Now,
			Description = viewModel.FileDescription,
			FileData = await _fileService.ConvertFileToByteArrayAsync(viewModel.NewAttachment),
			FileName = viewModel.NewAttachment.FileName,
			FileType = viewModel.NewAttachment.ContentType,
		};

		await _ticketService.AddTicketAttachmentAsync(viewModel.Ticket.Id, attachment);
		await _ticketHistoryService.AddAttachmentEventAsync(viewModel.Ticket, attachment);
		return Redirect(Url.RouteUrl(new { controller = "Tickets", action = "ViewTicket", ticketId = viewModel.Ticket.Id }) + "#attachments");
	}

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> ShowAttachment(int ticketAttachmentId)
	{
		TicketAttachment? attachment = await _ticketService.GetTicketAttachmentByIdAsync(ticketAttachmentId);

		if (attachment is null)
			return View(nameof(NotFound));

		Project project = (await _projectService.GetProjectByIdAsync(attachment.Ticket!.ProjectId))!;
		AppUser user = await _userManager.GetUserAsync(User);

		if (!UserIsAdmin && !project.Team.Contains(user))
			return View(nameof(NotAuthorized));

		string fileName = attachment.FileName;
		byte[] fileData = attachment.FileData;
		string ext = Path.GetExtension(fileName).Replace(".", "");

		Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
		return File(fileData, $"application/{ext}");
	}

	[HttpGet]
	[Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}, {nameof(Roles.Developer)}")]
	public async Task<ViewResult> ConfirmArchiveTicket(int ticketId)
	{
		Ticket? ticket = await _ticketService.GetTicketByIdAsync(ticketId);

		if (ticket is null)
			return View(nameof(NotFound));

		Project project = (await _projectService.GetProjectByIdAsync(ticket.ProjectId))!;
		AppUser user = await _userManager.GetUserAsync(User);

		if (!UserIsAdmin && project.ProjectManagerId != user.Id && ticket.DeveloperId != user.Id)
			return View(nameof(NotAuthorized));

		return View(ticket);
	}

	[HttpPost]
	[Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}, {nameof(Roles.Developer)}")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ArchiveTicketConfirmed(Ticket ticket)
	{
		Ticket? ticketToArchive = await _ticketService.GetTicketByIdAsync(ticket.Id);

		if (ticketToArchive is null)
			return View(nameof(NotFound));

		AppUser user = await _userManager.GetUserAsync(User);

		await _ticketService.ArchiveTicketAsync(ticketToArchive);
		await _ticketHistoryService.AddArchiveChangeEventAsync(ticketToArchive, user.Id);

		return RedirectToAction("ViewProject", "Projects", new { projectId = ticketToArchive.ProjectId });
	}

    [HttpGet]
    [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}, {nameof(Roles.Developer)}")]
    public async Task<ViewResult> ConfirmUnarchiveTicket(int ticketId)
    {
        Ticket? ticket = await _ticketService.GetTicketByIdAsync(ticketId);

        if (ticket is null)
            return View(nameof(NotFound));

        Project project = (await _projectService.GetProjectByIdAsync(ticket.ProjectId))!;
        AppUser user = await _userManager.GetUserAsync(User);

        if (!UserIsAdmin && project.ProjectManagerId != user.Id && ticket.DeveloperId != user.Id)
            return View(nameof(NotAuthorized));

        return View(ticket);
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}, {nameof(Roles.Developer)}")]
	[ValidateAntiForgeryToken]
    public async Task<IActionResult> UnarchiveTicketConfirmed(Ticket ticket)
    {
        Ticket? ticketToUnarchive = await _ticketService.GetTicketByIdAsync(ticket.Id);

        if (ticketToUnarchive is null)
            return View(nameof(NotFound));

        AppUser user = await _userManager.GetUserAsync(User);

        await _ticketService.UnarchiveTicketAsync(ticketToUnarchive);
        await _ticketHistoryService.AddArchiveChangeEventAsync(ticketToUnarchive, user.Id);

        return RedirectToAction(nameof(ViewTicket), new { ticketId = ticketToUnarchive.Id });
    }

	[HttpGet]
	[Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}, {nameof(Roles.Developer)}")]
	public async Task<ViewResult> ConfirmDeleteTicket(int ticketId)
	{
		Ticket? ticket = await _ticketService.GetTicketByIdAsync(ticketId);

		if (ticket is null)
			return View(nameof(NotFound));

        Project project = (await _projectService.GetProjectByIdAsync(ticket.ProjectId))!;
        AppUser user = await _userManager.GetUserAsync(User);

        if (!UserIsAdmin && project.ProjectManagerId != user.Id && ticket.DeveloperId != user.Id)
            return View(nameof(NotAuthorized));

        return View(ticket);
	}

    [HttpPost]
    [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}, {nameof(Roles.Developer)}")]
	[ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTicketConfirmed(Ticket ticket)
    {
        Ticket? ticketToDelete = await _ticketService.GetTicketByIdAsync(ticket.Id);

        if (ticketToDelete is null)
            return View(nameof(NotFound));

		await _ticketService.DeleteTicketAsync(ticketToDelete);
        return RedirectToAction("ViewProject", "Projects", new { projectId = ticketToDelete.ProjectId });
    }

	[HttpGet]
	public ViewResult NotAuthorized()
	{
		return View();
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

		List<AppUser> usersAvailableToAssign = await _projectService.GetDevelopersOnProjectAsync(ticket.ProjectId);

		if (!usersAvailableToAssign.Contains(appUser))
			usersAvailableToAssign.Add(appUser); // creator should be able to assign themself the ticket

		CreateOrEditTicketViewModel viewModel = new CreateOrEditTicketViewModel
		{
			Ticket = ticket,
			Developers = new SelectList(usersAvailableToAssign, "Id", "FullName", "Unassigned"),
			Priorities = new SelectList(await _ticketService.GetTicketPrioritiesAsync(), "Id", "Description", "medium"),
			Types = new SelectList(await _ticketService.GetTicketTypesAsync(), "Id", "Description", "bug"),
			Statuses = new SelectList(await _ticketService.GetTicketStatusesAsync(), "Id", "Description", "unassigned"),
		};

		return viewModel;
	}

	private async Task<CreateOrEditTicketViewModel> GenerateEditTicketViewModel(Ticket ticket)
	{
		AppUser appUser = await _userManager.GetUserAsync(User);

        List<AppUser> usersAvailableToAssign = await _projectService.GetDevelopersOnProjectAsync(ticket.ProjectId);

        if (!usersAvailableToAssign.Contains(appUser))
            usersAvailableToAssign.Add(appUser); // creator/editor should be able to assign themself the ticket

        CreateOrEditTicketViewModel viewModel = new CreateOrEditTicketViewModel()
		{
			Ticket = ticket,
			Title = ticket.Title,
			Description = ticket.Description,
			Developers = new SelectList(usersAvailableToAssign, "Id", "FullName", ticket.DeveloperId),
			Priorities = new SelectList(await _ticketService.GetTicketPrioritiesAsync(), "Id", "Description", ticket.TicketPriorityId),
			Types = new SelectList(await _ticketService.GetTicketTypesAsync(), "Id", "Description", ticket.TicketTypeId),
			Statuses = new SelectList(await _ticketService.GetTicketStatusesAsync(), "Id", "Description", ticket.TicketStatusId),
			IsStatusDropdownEnabled = appUser.Id == ticket.DeveloperId ? true : false,
        };

		return viewModel;
	}

	private bool DetermineIfTicketChangesWereMade(CreateOrEditTicketViewModel viewModel)
	{
        Ticket ticket = viewModel.Ticket;

		return !(ticket.Title == viewModel.Title
			&& ticket.Description == viewModel.Description
			&& ticket.TicketTypeId == viewModel.SelectedType
			&& ticket.TicketPriorityId == viewModel.SelectedPriority
			&& ticket.TicketStatusId == viewModel.SelectedStatus
			&& ticket.DeveloperId == viewModel.SelectedDeveloper);
    }

	private async Task<TicketHistory> CreateTicketChangeHistory(CreateOrEditTicketViewModel viewModel)
	{
        Ticket ticket = viewModel.Ticket;
        AppUser editingUser = await _userManager.GetUserAsync(User);

        TicketHistory historyItem = new TicketHistory()
        {
            TicketId = ticket.Id,
            AppUserId = (await _userManager.GetUserAsync(User)).Id,
            Created = DateTime.Now,
            Description = $"{editingUser.FullName} made the following changes:</br><ul>",
        };

        if (ticket.Title != viewModel.Title)
            historyItem.Description += $"<li>Changed the ticket title to <strong>{viewModel.Title}</strong></li>";

        if (ticket.Description != viewModel.Description)
            historyItem.Description += $"<li>Changed the ticket description to:</br>{viewModel.Description}</li>";

        if (ticket.TicketTypeId != viewModel.SelectedType)
            historyItem.Description += $"<li>Changed the ticket type to <strong>{await _ticketService.GetTicketTypeDescriptionByIdAsync(viewModel.SelectedType)}</strong></li>";

        if (ticket.TicketPriorityId != viewModel.SelectedPriority)
            historyItem.Description += $"<li>Changed the ticket priority to <strong>{await _ticketService.GetTicketPriorityDescriptionByIdAsync(viewModel.SelectedPriority)}</strong></li>";

        if (ticket.TicketStatusId != viewModel.SelectedStatus && ticket.TicketStatusId != "unassigned")
            historyItem.Description += $"<li>Changed the ticket status to <strong>{await _ticketService.GetTicketStatusDescriptionByIdAsync(viewModel.SelectedStatus)}</strong></li>";

        if (ticket.DeveloperId != viewModel.SelectedDeveloper)
        {
			if (viewModel.SelectedDeveloper is null)
				historyItem.Description += $"<li>Changed the ticket developer to <strong>Unassigned</strong></li>";

			else
			{
				AppUser? newDeveloper = await _userManager.FindByIdAsync(viewModel.SelectedDeveloper);
				historyItem.Description += $"<li>Changed the ticket developer to <strong>{newDeveloper.FullName ?? "Unassigned"}</strong></li>";
			}
        }

		historyItem.Description += "</ul>";

		return historyItem;
    }

	#endregion
}
