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

namespace ProjectManager.Controllers;

public class ProjectsController : Controller
{
    private readonly ICompanyService _companyService;
    private readonly IProjectService _projectService;
    private readonly IFileService _fileService;
    private readonly UserManager<AppUser> _userManager;

    private const string ListView = "List";
    private const string CreateOrEditProjectView = "CreateOrEditProject";

    private bool UserIsAdmin => User.IsInRole(nameof(Roles.Admin));

    public ProjectsController(ICompanyService companyService, IProjectService projectService, IFileService fileService, UserManager<AppUser> userManager)
    {
        _companyService = companyService;
        _projectService = projectService;
        _fileService = fileService;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize]
    public async Task<ViewResult> ViewProject(int projectId)
    {
        Project? project = await _projectService.GetProjectByIdAsync(projectId);

        if (project is null)
            return View("NotFound");

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

        return View(ListView, viewModel);
    }

    [HttpGet]
    [Authorize]
    public async Task<ViewResult> ListArchivedProjects(string sortBy = "Project Name", int pageNumber = 1, int perPage = 10)
    {
        int companyId = User.Identity!.GetCompanyId();
        AppUser user = await _userManager.GetUserAsync(User);

        List<Project> archivedProjects = 
            UserIsAdmin ? await _projectService.GetAllArchivedCompanyProjectsAsync(companyId) : archivedProjects = await _projectService.GetUserArchivedProjectsAsync(user.Id);

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

        return View(ListView, viewModel);
    }

    [HttpGet]
    [Authorize(Roles = "Admin, ProjectManager")]
    public async Task<ViewResult> CreateProject()
    {
        ViewData["Action"] = "Create";

        CreateOrEditProjectViewModel viewModel = await GenerateCreateProjectViewModel();

        return View(CreateOrEditProjectView, viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Admin, ProjectManager")]
    [ValidateAntiForgeryToken]
    public async Task<RedirectToActionResult> CreateProject(CreateOrEditProjectViewModel viewModel)
    {
        ViewData["Action"] = "Create";

        Project project = viewModel.Project;

        project.Name = viewModel.Name;
        project.Description = viewModel.Description;
        project.Deadline = viewModel.Deadline;
        await UpdateProjectImage(viewModel, project);

        project.Id = await _projectService.CreateNewProjectAsync(project);
        await AssignSelectedTeamToProject(viewModel, project);

        return RedirectToAction(nameof(ListActiveProjects));
    }

    [HttpGet]
    [Authorize(Roles = "Admin, ProjectManager")]
    public async Task<ViewResult> EditProject(int projectId)
    {
        ViewData["Action"] = "Edit";

        Project? project = await _projectService.GetProjectByIdAsync(projectId);

        if (project is null)
            return View("NotFound");

        CreateOrEditProjectViewModel viewModel = await GenerateEditProjectViewModel(project);

        return View(CreateOrEditProjectView, viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Admin, ProjectManager")]
    [ValidateAntiForgeryToken]
    public async Task<RedirectToActionResult> EditProject(CreateOrEditProjectViewModel viewModel)
    {
        ViewData["Action"] = "Edit";

        Project? project = viewModel.Project;

        project.Name = viewModel.Name;
        project.Description = viewModel.Description;
        project.Deadline = viewModel.Deadline;
		await UpdateProjectImage(viewModel, project);

		await _projectService.UpdateProjectAsync(project);
		await AssignSelectedTeamToProject(viewModel, project);
        
        return RedirectToAction(nameof(ListActiveProjects));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ViewResult> ConfirmDeleteProject(int projectId)
    {
        Project? project = await _projectService.GetProjectByIdAsync(projectId);

        if (project is null)
            return View("NotFound");

        return View(project);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProjectConfirmed(Project project)
    {
        if(!await _projectService.DeleteProjectAsync(project.Id))
			return Problem($"Could not delete project. Id: {project.Id}, Name: {project.Name}");

        return RedirectToAction(nameof(ListActiveProjects));
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
            Deadline = DateTime.Now.AddDays(1),
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

    private async Task AssignSelectedTeamToProject(CreateOrEditProjectViewModel viewModel, Project project)
    {
        await AssignProjectManager(viewModel, project);
        await AssignDevelopers(viewModel, project);
        await AssignMembers(viewModel, project);
    }

    private async Task AssignProjectManager(CreateOrEditProjectViewModel viewModel, Project project)
    {
		AppUser selectedProjectManager = await _userManager.FindByIdAsync(viewModel.SelectedManager);

		if (project.ProjectManagerId is not null)
			await _projectService.RemoveProjectManagerAsync(project.Id);

		if (selectedProjectManager is not null)
			await _projectService.AssignProjectManagerAsync(selectedProjectManager.Id, project.Id);
	}

    private async Task AssignDevelopers(CreateOrEditProjectViewModel viewModel, Project project)
    {

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

    private async Task AssignMembers(CreateOrEditProjectViewModel viewModel, Project project)
    {
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

    private async Task UpdateProjectImage(CreateOrEditProjectViewModel viewModel, Project project)
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
