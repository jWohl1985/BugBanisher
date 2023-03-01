using BugBanisher.Models;
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

namespace BugBanisher.Controllers;



public class TicketsController : Controller
{
	private readonly IProjectService _projectService;
	private readonly ITicketService _ticketService;
	private readonly IFileService _fileService;
	private readonly INotificationService _notificationService;
	private readonly UserManager<AppUser> _userManager;

	private bool UserIsAdmin => User.IsInRole(nameof(Roles.Admin));

	public TicketsController(IProjectService projectService, 
		ITicketService ticketService, 
		IFileService fileService,
		INotificationService notificationService,
		UserManager<AppUser> userManager)
	{
		_projectService = projectService;
		_ticketService = ticketService;
		_fileService = fileService;
		_notificationService = notificationService;
		_userManager = userManager;
	}

	[HttpGet]
	[Authorize]
	public async Task<ViewResult> ViewTicket(int ticketId)
	{
		Ticket? ticket = await _ticketService.GetTicketByIdAsync(ticketId);

		if (ticket is null)
			return View("NotFound");

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
	public async Task<ViewResult> ListOpenTickets(string sortBy = "Project Name", int pageNumber = 1, int perPage = 10)
	{
		ViewData["OpenOrActionRequired"] = "Open";
        int companyId = User.Identity!.GetCompanyId();
        AppUser user = await _userManager.GetUserAsync(User);

        List<Ticket> activeTickets =
            UserIsAdmin ? await _ticketService.GetAllOpenCompanyTicketsAsync(companyId) : await _ticketService.GetUserOpenTicketsAsync(user.Id);

        TicketListViewModel viewModel = new TicketListViewModel()
        {
            OpenOrActionRequired = "Open",
            Tickets = activeTickets,
            PageNumber = pageNumber.ToString(),
            PerPage = perPage.ToString(),
			SortBy = sortBy,

            SortByOptions = new SelectList(new string[] { "Project Name", "Developer", "Priority", "Type", "Status" }, sortBy),
            PerPageOptions = new SelectList(new string[] { "5", "10", "20", "30", "40", "50" }, perPage.ToString()),
        };

        return View("List", viewModel);
	}

    [HttpGet]
    [Authorize]
    public async Task<ViewResult> ListActionRequiredTickets(string sortBy = "Project Name", int pageNumber = 1, int perPage = 10)
    {
		ViewData["OpenOrActionRequired"] = "Action Required";
        int companyId = User.Identity!.GetCompanyId();
        AppUser user = await _userManager.GetUserAsync(User);
        
        List<Ticket> actionRequiredTickets =
            UserIsAdmin ? await _ticketService.GetAllActionRequiredTicketsAsync(companyId) : await _ticketService.GetUserActionRequiredTicketsAsync(user.Id);

        TicketListViewModel viewModel = new TicketListViewModel()
        {
            OpenOrActionRequired = "Action Required",
            Tickets = actionRequiredTickets,
            PageNumber = pageNumber.ToString(),
            PerPage = perPage.ToString(),
            SortBy = sortBy,

            SortByOptions = new SelectList(new string[] { "Project Name", "Developer", "Priority", "Type", "Status" }, sortBy),
            PerPageOptions = new SelectList(new string[] { "5", "10", "20", "30", "40", "50" }, perPage.ToString()),
        };

        return View("List", viewModel);
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

		if (ticket.DeveloperId is not null)
			await _notificationService.CreateNewTicketNotificationAsync(ticket.DeveloperId, ticket);

		ticket.Id = await _ticketService.CreateTicketAsync(ticket);

		return RedirectToAction("Index", "Home");
	}

	[HttpGet]
	[Authorize(Roles = "Admin, ProjectManager, Developer")]
	public async Task<ViewResult> EditTicket(int ticketId)
	{
		ViewData["Action"] = "Edit";

		Ticket? ticket = await _ticketService.GetTicketByIdAsync(ticketId);

		if (ticket is null)
			return View("NotFound");

		CreateOrEditTicketViewModel viewModel = await GenerateEditTicketViewModel(ticket);

		return View("CreateOrEditTicket", viewModel);
	}

    [HttpPost]
    [Authorize(Roles = "Admin, ProjectManager, Developer")]
	[ValidateAntiForgeryToken]
    public async Task<RedirectToActionResult> EditTicket(CreateOrEditTicketViewModel viewModel)
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

		return RedirectToAction("ViewTicket", new { ticketId = ticket.Id });
    }

	[HttpPost]
	[Authorize]
	[ValidateAntiForgeryToken]
	public async Task<RedirectResult> AddComment(TicketViewModel viewModel)
	{
		AppUser user = await _userManager.GetUserAsync(User);

		if (string.IsNullOrEmpty(viewModel.NewComment))
		{
			TempData["CommentError"] = "Comment cannot be empty.";
			return Redirect(Url.RouteUrl(new { controller = "Tickets", action = "ViewTicket", ticketId = viewModel.Ticket.Id }) + "#comments");
		}

		TicketComment comment = new TicketComment()
		{
			TicketId = viewModel.Ticket.Id,
			AppUserId = user.Id,
			Created = DateTime.Now,
			Comment = viewModel.NewComment,
		};

		await _ticketService.AddTicketCommentAsync(viewModel.Ticket.Id, comment);

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
		return Redirect(Url.RouteUrl(new { controller = "Tickets", action = "ViewTicket", ticketId = viewModel.Ticket.Id }) + "#attachments");
	}

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> ShowAttachment(int ticketAttachmentId)
	{
		TicketAttachment? attachment = await _ticketService.GetTicketAttachmentByIdAsync(ticketAttachmentId);

		if (attachment is null)
			return View("NotFound");

		string fileName = attachment.FileName;
		byte[] fileData = attachment.FileData;
		string ext = Path.GetExtension(fileName).Replace(".", "");

		Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
		return File(fileData, $"application/{ext}");
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
