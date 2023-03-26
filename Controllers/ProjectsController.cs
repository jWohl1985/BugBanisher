using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BugBanisher.Models;
using BugBanisher.Extensions;
using BugBanisher.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using BugBanisher.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using BugBanisher.Models.Enums;
using System.ComponentModel.Design;
using System.Globalization;
using BugBanisher.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProjectManager.Controllers;

public class ProjectsController : Controller
{
    private readonly ICompanyService _companyService;
    private readonly IProjectService _projectService;
    private readonly IFileService _fileService;
    private readonly INotificationService _notificationService;
    private readonly UserManager<AppUser> _userManager;

    private bool UserIsAdmin => User.IsInRole(nameof(Roles.Admin));

    public ProjectsController(ICompanyService companyService, 
        IProjectService projectService, 
        IFileService fileService, 
        INotificationService notificationService, 
        UserManager<AppUser> userManager)
    {
        _companyService = companyService;
        _projectService = projectService;
        _fileService = fileService;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ViewResult> ViewProject(int projectId)
    {
        AppUser user = await _userManager.GetUserAsync(User);

        Project? project = await _projectService.GetProjectByIdAsync(projectId);

        if (project is null)
            return View(nameof(NotFound));

        if (!UserIsAdmin && project.ProjectManagerId != user.Id && !project.Team.Contains(user))
            return View(nameof(NotAuthorized));

        ProjectViewModel viewModel = new ProjectViewModel
        {
            Project = project,
            ProjectManager = await _projectService.GetProjectManagerAsync(project.Id),
            Developers = await _projectService.GetDevelopersOnProjectAsync(project.Id),
            Members = await _projectService.GetMembersOnProjectAsync(project.Id),
            Tickets = project.Tickets,
        };

        return View(viewModel);
    }

    [HttpGet]
    [Authorize]
    public async Task<ViewResult> ListActiveProjects(string sortBy = "Project Name", int pageNumber = 1, int perPage = 10)
    {
        int companyId = User.Identity!.GetCompanyId();
        AppUser user = await _userManager.GetUserAsync(User);

        List<Project> activeProjects = 
            UserIsAdmin ? await _projectService.GetAllActiveCompanyProjectsAsync(companyId) : await _projectService.GetUserActiveProjectsAsync(user.Id);

        ProjectListViewModel viewModel = new ProjectListViewModel()
        {
            ActiveOrArchived = "Active",
            Projects = activeProjects,
            PageNumber = pageNumber.ToString(),
            PerPage = perPage.ToString(),
            SortBy = sortBy,

            SortByOptions = new SelectList(new string[] { "Project Name", "Project Manager", "Due Date", "Open Tickets" }, sortBy),
            PerPageOptions = new SelectList(new string[] { "5", "10", "20", "30", "40", "50" }, perPage.ToString()),
        };

        return View("List", viewModel);
    }

    [HttpGet]
    [Authorize]
    public async Task<ViewResult> ListArchivedProjects(string sortBy = "Project Name", int pageNumber = 1, int perPage = 10)
    {
        int companyId = User.Identity!.GetCompanyId();
        AppUser user = await _userManager.GetUserAsync(User);

        List<Project> archivedProjects = 
            UserIsAdmin ? await _projectService.GetAllArchivedCompanyProjectsAsync(companyId) : await _projectService.GetUserArchivedProjectsAsync(user.Id);

        ProjectListViewModel viewModel = new ProjectListViewModel()
        {
            ActiveOrArchived = "Archived",
            Projects = archivedProjects,
            PageNumber = pageNumber.ToString(),
            PerPage = perPage.ToString(),
            SortBy = sortBy,

            SortByOptions = new SelectList(new string[] { "Project Name", "Project Manager", "Due Date", "Open Tickets" }, sortBy),
            PerPageOptions = new SelectList(new string[] { "5", "10", "20", "30", "40", "50" }, perPage.ToString()),
        };

        return View("List", viewModel);
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")]
    public async Task<ViewResult> CreateProject()
    {
        ViewData["Action"] = "Create";

        CreateOrEditProjectViewModel viewModel = await GenerateCreateProjectViewModel();

        return View("CreateOrEditProject", viewModel);
    }

    [HttpPost]
    [Authorize(Roles = nameof(Roles.Admin))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProject(CreateOrEditProjectViewModel viewModel)
    {
        ViewData["Action"] = "Create";

        Project project = viewModel.Project;

        foreach (string key in ModelState.Keys)
            if (key != "Image") ModelState.Remove(key);

        if (viewModel.Name is null)
            ModelState.AddModelError(string.Empty, "Project name cannot be empty");

        if (viewModel.Description is null)
            ModelState.AddModelError(string.Empty, "Project description cannot be empty");

        if (viewModel.Deadline < DateTime.Now)
            ModelState.AddModelError(string.Empty, "Project due date cannot be in the past");

        if (ModelState["Image"] is not null && ModelState["Image"]!.ValidationState == ModelValidationState.Invalid)
            ModelState.AddModelError(string.Empty, "Image is too large, or the wrong extension type");

        if (ModelState.ErrorCount > 0)
        {
            CreateOrEditProjectViewModel newViewModel = await GenerateCreateProjectViewModel();
            newViewModel.Name = viewModel.Name ?? string.Empty;
            newViewModel.Description = viewModel.Description ?? string.Empty;
            newViewModel.Deadline = viewModel.Deadline;
            newViewModel.SelectedManager = viewModel.SelectedManager;
            newViewModel.SelectedDevelopers = viewModel.SelectedDevelopers;
            newViewModel.SelectedMembers = viewModel.SelectedMembers;
            newViewModel.Image = viewModel.Image;
            return View("CreateOrEditProject", newViewModel);
        }

        project.Name = viewModel.Name!;
        project.Description = viewModel.Description!;
        project.Deadline = viewModel.Deadline;
        await UpdateProjectImageAsync(viewModel, project);

        project.Id = await _projectService.CreateNewProjectAsync(project);

        await AssignTeamToProjectAsync(viewModel, project);

        return RedirectToAction(nameof(ViewProject), new { projectId = project.Id });
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")]
    public async Task<ViewResult> EditProject(int projectId)
    {
        ViewData["Action"] = "Edit";

        Project? project = await _projectService.GetProjectByIdAsync(projectId);
        AppUser user = await _userManager.GetUserAsync(User);

        if (project is null)
            return View(nameof(NotFound));

        if (!UserIsAdmin && project.ProjectManagerId != user.Id && !project.Team.Contains(user))
            return View(nameof(NotAuthorized));

        CreateOrEditProjectViewModel viewModel = await GenerateEditProjectViewModel(project);

        return View("CreateOrEditProject", viewModel);
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProject(CreateOrEditProjectViewModel viewModel)
    {
        ViewData["Action"] = "Edit";

        Project? project = viewModel.Project;

        foreach (string key in ModelState.Keys)
            if (key != "Image") ModelState.Remove(key);

        if (viewModel.Name is null)
            ModelState.AddModelError(string.Empty, "Project name cannot be empty");

        if (viewModel.Description is null)
            ModelState.AddModelError(string.Empty, "Project description cannot be empty");

        if (viewModel.Deadline < DateTime.Now)
            ModelState.AddModelError(string.Empty, "Project due date cannot be in the past");

        if (ModelState["Image"] is not null && ModelState["Image"]!.ValidationState == ModelValidationState.Invalid)
            ModelState.AddModelError(string.Empty, "Image is too large, or the wrong extension type");

        if (ModelState.ErrorCount > 0)
        {
            CreateOrEditProjectViewModel newViewModel = await GenerateEditProjectViewModel(project);
            newViewModel.Name = viewModel.Name ?? string.Empty;
            newViewModel.Description = viewModel.Description ?? string.Empty;
            newViewModel.Deadline = viewModel.Deadline;
            newViewModel.SelectedManager = viewModel.SelectedManager;
            newViewModel.SelectedDevelopers = viewModel.SelectedDevelopers;
            newViewModel.SelectedMembers = viewModel.SelectedMembers;
            newViewModel.Image = viewModel.Image;
            return View("CreateOrEditProject", newViewModel);
        }

        project.Name = viewModel.Name!;
        project.Description = viewModel.Description!;
        project.Deadline = viewModel.Deadline;
		await UpdateProjectImageAsync(viewModel, project);

		await _projectService.UpdateProjectAsync(project);
		await AssignTeamToProjectAsync(viewModel, project);

		return RedirectToAction(nameof(ViewProject), new { projectId = project.Id });
	}

    [HttpGet]
    [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")]
    public async Task<ViewResult> ConfirmDeleteProject(int projectId)
    {
        Project? project = await _projectService.GetProjectByIdAsync(projectId);

        if (project is null)
            return View(nameof(NotFound));

        AppUser user = await _userManager.GetUserAsync(User);

        if (User.IsInRole(nameof(Roles.ProjectManager)) && project.ProjectManagerId != user.Id)
            return View(nameof(NotAuthorized));

        return View(project);
    }

    [HttpPost]
    [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")]
    [ValidateAntiForgeryToken]
    public async Task<RedirectToActionResult> DeleteProjectConfirmed(Project project)
    {
        AppUser user = await _userManager.GetUserAsync(User);

        if (User.IsInRole(nameof(Roles.ProjectManager)) && project.ProjectManagerId != user.Id)
            return RedirectToAction(nameof(NotAuthorized));

        await _projectService.DeleteProjectAsync(project.Id);

        return RedirectToAction(nameof(ListActiveProjects));
    }

    [HttpGet]
    [Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")]
    public async Task<ViewResult> ConfirmArchiveProject(int projectId)
    {
        Project? project = await _projectService.GetProjectByIdAsync(projectId);

        if (project is null)
            return View(nameof(NotFound));

        AppUser user = await _userManager.GetUserAsync(User);

        if (User.IsInRole(nameof(Roles.ProjectManager)) && project.ProjectManagerId != user.Id)
            return View(nameof(NotAuthorized));

        return View(project);
    }

	[HttpPost]
	[Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")]
    [ValidateAntiForgeryToken]
	public async Task<IActionResult> ArchiveProjectConfirmed(Project project)
	{
        AppUser user = await _userManager.GetUserAsync(User);

        if (User.IsInRole(nameof(Roles.ProjectManager)) && project.ProjectManagerId != user.Id)
            return RedirectToAction(nameof(NotAuthorized));

        await _projectService.ArchiveProjectAsync(project.Id);

		return RedirectToAction(nameof(ViewProject), new { projectId = project.Id });
	}

	[HttpGet]
	[Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")]
	public async Task<ViewResult> ConfirmUnarchiveProject(int projectId)
	{
		Project? project = await _projectService.GetProjectByIdAsync(projectId);

		if (project is null)
			return View(nameof(NotFound));

		return View(project);
	}

	[HttpPost]
	[Authorize(Roles = $"{nameof(Roles.Admin)}, {nameof(Roles.ProjectManager)}")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> UnarchiveProjectConfirmed(Project project)
	{
        AppUser user = await _userManager.GetUserAsync(User);

        if (User.IsInRole(nameof(Roles.ProjectManager)) && project.ProjectManagerId != user.Id)
            return RedirectToAction(nameof(NotAuthorized));

        await _projectService.UnarchiveProjectAsync(project.Id);

		return RedirectToAction(nameof(ViewProject), new { projectId = project.Id });
	}

    [HttpGet]
    public ViewResult NotAuthorized()
    {
        return View();
    }

	#region Private Helper Methods
	private async Task<CreateOrEditProjectViewModel> GenerateCreateProjectViewModel()
    {
        Project createdProject = new Project
        {
            CompanyId = User.Identity!.GetCompanyId(),
            Name = String.Empty,
            Description = String.Empty,
            Created = DateTime.Now,
            Deadline = DateTime.Now.AddDays(30),
        };

        return new CreateOrEditProjectViewModel()
        {
            Project = createdProject,

            Deadline = createdProject.Deadline,
            Name = createdProject.Name,
            Description = createdProject.Description,

            SelectedManager = "Unassigned",
            ProjectManagers = new SelectList(await _companyService.GetAllProjectManagersAsync(createdProject.CompanyId), "Id", "FullName", "Unassigned"),

            SelectedDevelopers = new string[] { },
            Developers = new MultiSelectList(await _companyService.GetAllDevelopersAsync(createdProject.CompanyId), "Id", "FullName", new string[] { }),

            SelectedMembers = new string[] { },
            Members = new MultiSelectList(await _companyService.GetAllMembersAsync(createdProject.CompanyId), "Id", "FullName", new string[] { }),
        };
    }

    private async Task<CreateOrEditProjectViewModel> GenerateEditProjectViewModel(Project project)
    {
        string currentProjectManager = project.ProjectManagerId is not null ? project.ProjectManagerId : "Unassigned";
        List<AppUser> developers = await _projectService.GetDevelopersOnProjectAsync(project.Id);
        List<AppUser> members = await _projectService.GetMembersOnProjectAsync(project.Id);

        List<string> developerIds = new List<string>();
        List<string> memberIds = new List<string>();

        foreach (AppUser developer in developers)
            developerIds.Add(developer.Id);

        foreach (AppUser member in members)
            memberIds.Add(member.Id);

        return new CreateOrEditProjectViewModel()
        {
            Project = project,
            Deadline = project.Deadline,
            Name = project.Name,
            Description = project.Description,

            SelectedManager = currentProjectManager,
            ProjectManagers = new SelectList(await _companyService.GetAllProjectManagersAsync(project.CompanyId), "Id", "FullName", currentProjectManager),

            SelectedDevelopers = developerIds,
            Developers = new MultiSelectList(await _companyService.GetAllDevelopersAsync(project.CompanyId), "Id", "FullName", developerIds),

            SelectedMembers = memberIds,
            Members = new MultiSelectList(await _companyService.GetAllMembersAsync(project.CompanyId), "Id", "FullName", memberIds),
        };
    }

    private async Task AssignTeamToProjectAsync(CreateOrEditProjectViewModel viewModel, Project project)
    {
        await AssignProjectManagerAsync(viewModel, project);
        await AssignDevelopersAsync(viewModel, project);
        await AssignMembersAsync(viewModel, project);
    }

    private async Task AssignProjectManagerAsync(CreateOrEditProjectViewModel viewModel, Project project)
    {
		AppUser selectedProjectManager = await _userManager.FindByIdAsync(viewModel.SelectedManager);

        if (selectedProjectManager is not null && selectedProjectManager.Id != project.ProjectManagerId)
			await _notificationService.CreateNewProjectNotificationAsync(selectedProjectManager.Id, project);

		if (project.ProjectManagerId is not null)
			await _projectService.RemoveProjectManagerAsync(project.Id);

        if (selectedProjectManager is not null)
            await _projectService.AssignProjectManagerAsync(selectedProjectManager.Id, project.Id);
	}

    private async Task AssignDevelopersAsync(CreateOrEditProjectViewModel viewModel, Project project)
    {
        if (viewModel.SelectedDevelopers is not null)
        {
            foreach (string employeeId in viewModel.SelectedDevelopers)
            {
                AppUser employee = await _userManager.FindByIdAsync(employeeId);

                if (!project.Team.Contains(employee))
                    await _notificationService.CreateNewProjectNotificationAsync(employeeId, project);
            }
        }

        foreach (AppUser projectMember in project.Team)
        {
            if (false == await _userManager.IsInRoleAsync(projectMember, nameof(Roles.Developer)))
                continue;

            if (viewModel.SelectedDevelopers is null || !viewModel.SelectedDevelopers.Contains(projectMember.Id))
            {
                await _projectService.RemoveEmployeeFromProjectAsync(projectMember.Id, project.Id);
            }
        }

        if (viewModel.SelectedDevelopers is null)
            return;

		foreach (string employeeId in viewModel.SelectedDevelopers)
		{
            if (!project.Team.Where(projectMember => projectMember.Id == employeeId).Any())
            {
                await _projectService.AssignEmployeeToProjectAsync(employeeId, project.Id);
            }
		}
	}

    private async Task AssignMembersAsync(CreateOrEditProjectViewModel viewModel, Project project)
    {
        if (viewModel.SelectedMembers is not null)
        {
            foreach (string employeeId in viewModel.SelectedMembers)
            {
                AppUser employee = await _userManager.FindByIdAsync(employeeId);

                if (!project.Team.Contains(employee))
                    await _notificationService.CreateNewProjectNotificationAsync(employeeId, project);
            }
        }

        foreach (AppUser projectMember in project.Team)
        {
			if (false == await _userManager.IsInRoleAsync(projectMember, nameof(Roles.Member)))
				continue;

			if (viewModel.SelectedMembers is null || !viewModel.SelectedMembers.Contains(projectMember.Id))
            {
                await _projectService.RemoveEmployeeFromProjectAsync(projectMember.Id, project.Id);
            }
        }

        if (viewModel.SelectedMembers is null)
            return;

        foreach (string employeeId in viewModel.SelectedMembers)
        {
            if (!project.Team.Where(projectMember => projectMember.Id == employeeId).Any())
            {
                await _projectService.AssignEmployeeToProjectAsync(employeeId, project.Id);
            }
        }
	}

    private async Task UpdateProjectImageAsync(CreateOrEditProjectViewModel viewModel, Project project)
    {
		if (viewModel.Image is not null)
		{
			byte[] imageData = await _fileService.ConvertFileToByteArrayAsync(viewModel.Image);
			string contentType = viewModel.Image.ContentType;

			project.PictureExtension = contentType;
			project.PictureData = imageData;
		}
	}

    #endregion
}
