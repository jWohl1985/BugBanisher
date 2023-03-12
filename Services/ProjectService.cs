using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BugBanisher.Database;
using BugBanisher.Models;
using BugBanisher.Models.Enums;
using BugBanisher.Services.Interfaces;

namespace BugBanisher.Services;

public class ProjectService : IProjectService
{
	private readonly BugBanisherContext _context;
	private readonly UserManager<AppUser> _userManager;

	public ProjectService(BugBanisherContext context, UserManager<AppUser> userManager)
	{
		_context = context;
		_userManager = userManager;
	}

	public async Task<int> CreateNewProjectAsync(Project project)
	{
		try
		{
			await _context.Projects.AddAsync(project);
			await _context.SaveChangesAsync();
			return project.Id;
		}
		catch
		{
			return 0;
		}
	}

	public async Task<bool> DeleteProjectAsync(int projectId)
	{
		Project? project = await GetProjectByIdAsync(projectId);

		if (project is null)
			return false;

		foreach (AppUser projectMember in project.Team)
			project.Team.Remove(projectMember);

		_context.Projects.Remove(project);
		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<List<Project>> GetAllActiveCompanyProjectsAsync(int companyId)
	{
		return await _context.Projects.Where(p => p.CompanyId == companyId && !p.IsArchived)
            .Include(p => p.Team)
			.Include(p => p.Tickets)
				.ThenInclude(t => t.Type)
			.Include(p => p.Tickets)
				.ThenInclude(t => t.Status)
			.Include(p => p.Tickets)
				.ThenInclude(t => t.Priority)
			.ToListAsync();
	}

	public async Task<List<Project>> GetAllArchivedCompanyProjectsAsync(int companyId)
	{
		return await _context.Projects.Where(p => p.CompanyId == companyId && p.IsArchived)
            .Include(p => p.Team)
			.Include(p => p.Tickets)
				.ThenInclude(t => t.Type)
			.Include(p => p.Tickets)
				.ThenInclude(t => t.Status)
			.Include(p => p.Tickets)
				.ThenInclude(t => t.Priority)
			.ToListAsync();
	}

	public async Task<Project?> GetProjectByIdAsync(int projectId)
	{
		return await _context.Projects.Where(p => p.Id == projectId)
			.Include(p => p.Team)
            .Include(p => p.Tickets)
				.ThenInclude(t => t.Priority)
			.Include(p => p.Tickets)
				.ThenInclude(t => t.Type)
			.Include(p => p.Tickets)
				.ThenInclude(t => t.Status)
			.Include(p => p.Tickets)
				.ThenInclude(t => t.Developer)
			.Include (p => p.Tickets)
				.ThenInclude(t => t.Comments)
			.Include(p => p.Tickets)
				.ThenInclude(t => t.Attachments)
			.Include(p => p.Tickets)
				.ThenInclude(t => t.History)
			.FirstOrDefaultAsync();
	}

	public async Task<AppUser?> GetProjectManagerAsync(int projectId)
	{
		Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

		if (project is null)
			return null;

		AppUser? projectManager = await _context.Users.FirstOrDefaultAsync(u => u.Id == project.ProjectManagerId);

		return projectManager;
	}

	public async Task<List<AppUser>> GetDevelopersOnProjectAsync(int projectId)
	{
		Project? project = await _context.Projects
			.Include(p => p.Team)
			.FirstOrDefaultAsync(p => p.Id == projectId);

		List<AppUser> developers = new List<AppUser>();

		if (project is null)
			return developers;

		foreach (AppUser projectMember in project.Team)
		{
			AppUser employee = await _context.Users.FirstAsync(u => u.Id == projectMember.Id);

			if (await _userManager.IsInRoleAsync(employee, nameof(Roles.Developer)))
				developers.Add(employee);
		}

		return developers;
	}

    public async Task<List<AppUser>> GetMembersOnProjectAsync(int projectId)
    {
		Project? project = await _context.Projects
			.Include(p => p.Team)
			.FirstOrDefaultAsync(p => p.Id == projectId);

        List<AppUser> members = new List<AppUser>();

		if (project is null)
			return members;

        foreach (AppUser projectMember in project.Team)
        {
			AppUser employee = await _context.Users.FirstAsync(u => u.Id == projectMember.Id);

            if (await _userManager.IsInRoleAsync(employee, nameof(Roles.Member)))
                members.Add(employee);
        }

        return members;
    }

    public async Task<List<Project>> GetProjectsByProjectManagerAsync(string projectManagerId)
	{
		return await _context.Projects.Where(p => p.ProjectManagerId == projectManagerId)
            .Include(p => p.Team)
            .Include(p => p.Tickets)
				.ThenInclude(t => t.Type)
			.Include(p => p.Tickets)
				.ThenInclude(t => t.Status)
			.Include(p => p.Tickets)
				.ThenInclude(t => t.Priority)
			.ToListAsync();
	}

	public async Task<List<Project>> GetUserActiveProjectsAsync(string userId)
	{
		AppUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

		if (user is null || user.CompanyId is null)
			return new List<Project>();

		List<Project> activeCompanyProjects = await GetAllActiveCompanyProjectsAsync(user.CompanyId.Value);

		return activeCompanyProjects.Where(p => p.Team.Any(t => t.Id == user.Id)).ToList();
	}

	public async Task<List<Project>> GetUserArchivedProjectsAsync(string userId)
	{
		AppUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

		if (user is null || user.CompanyId is null)
			return new List<Project>();

		List<Project> archivedCompanyProjects = await GetAllArchivedCompanyProjectsAsync(user.CompanyId.Value);

		return archivedCompanyProjects.Where(p => p.Team.Any(t => t.Id == user.Id)).ToList();
	}

	public async Task<bool> AssignProjectManagerAsync(string projectManagerId, int projectId)
	{
		AppUser? projectManager = await _context.Users.FirstOrDefaultAsync(u => u.Id == projectManagerId);
		Project? project = await _context.Projects
			.Include(p => p.Team)
			.FirstOrDefaultAsync(p => p.Id == projectId);

		if (projectManager is null || project is null)
			return false;

        IEnumerable<string> userRoles = await _userManager.GetRolesAsync(projectManager);

		if (!userRoles.Contains(nameof(Roles.ProjectManager)))
			return false;

        project.ProjectManagerId = projectManager.Id;
        project.Team.Add(projectManager);
		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<bool> RemoveProjectManagerAsync(int projectId)
	{
		Project? project = await _context.Projects
			.Include(p => p.Team)
			.FirstOrDefaultAsync(p => p.Id == projectId);

		if (project is null)
			return false;

		AppUser? projectManager = await _context.Users.FirstOrDefaultAsync(u => u.Id == project.ProjectManagerId);

		if (projectManager is null)
			return false;

		project.ProjectManagerId = null;
		project.Team.Remove(projectManager);
		await _context.SaveChangesAsync();
		return true;
	}

    public async Task<bool> AssignEmployeeToProjectAsync(string employeeId, int projectId)
    {
		Project? project = await _context.Projects
			.Include(p => p.Team)
			.FirstOrDefaultAsync(p => p.Id == projectId);

		AppUser? employee = await _context.Users.FirstOrDefaultAsync(u => u.Id == employeeId);

		if (project is null || employee is null)
			return false;

		project.Team.Add(employee);
		await _context.SaveChangesAsync();
		return true;
    }

	public async Task<bool> RemoveEmployeeFromProjectAsync(string employeeId, int projectId)
	{
		Project? project = await _context.Projects
			.Include(p => p.Team)
			.FirstOrDefaultAsync(p => p.Id == projectId);

		AppUser? employee = await _context.Users.FirstOrDefaultAsync(u => u.Id == employeeId);

		if (project is null || employee is null)
			return false;

		project.Team.Remove(employee);
		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<bool> RemoveEmployeeFromAllActiveProjectsAsync(int companyId, string employeeId)
	{
		AppUser? employee = await _context.Users.FirstOrDefaultAsync(u => u.Id == employeeId);

		if (employee is null)
			return false;

		foreach (Project project in await GetAllActiveCompanyProjectsAsync(companyId))
		{
			if (project.ProjectManagerId == employeeId)
				await RemoveProjectManagerAsync(project.Id);

			if (project.Team.Contains(employee))
				project.Team.Remove(employee);
		}

		return true;
	}

	public async Task UpdateProjectAsync(Project project)
	{
		_context.Update(project);
		await _context.SaveChangesAsync();
	}

	public async Task<bool> ArchiveProjectAsync(int projectId)
	{
		Project? project = await GetProjectByIdAsync(projectId);

		if (project is null)
			return false;

		project.IsArchived = true;

		foreach (Ticket ticket in project.Tickets)
		{
			ticket.IsArchivedByProject = true;
		}

		_context.Projects.Update(project);
		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<bool> UnarchiveProjectAsync(int projectId)
	{
		Project? project = await GetProjectByIdAsync(projectId);

		if (project is null)
			return false;

		project.IsArchived = false;

		foreach (Ticket ticket in project.Tickets)
		{
			ticket.IsArchivedByProject = false;
		}

		_context.Projects.Update(project);
		await _context.SaveChangesAsync();
		return true;
	}
}
