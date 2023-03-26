using BugBanisher.Models;

namespace BugBanisher.Services.Interfaces;

public interface IProjectService
{
	Task<int> CreateNewProjectAsync(Project project);
	Task<bool> DeleteProjectAsync(int projectId);
	Task<List<Project>> GetAllActiveCompanyProjectsAsync(int companyId);
	Task<List<Project>> GetAllArchivedCompanyProjectsAsync(int companyId);
	Task<List<Project>> GetUserActiveProjectsAsync(string userId);
	Task<List<Project>> GetUserArchivedProjectsAsync(string Id);
	Task<List<Project>> GetProjectsByProjectManagerAsync(string projectManagerId);
	Task<Project?> GetProjectByIdAsync(int projectId);
	Task<AppUser?> GetProjectManagerAsync(int projectId);
	Task<List<AppUser>> GetDevelopersOnProjectAsync(int projectId);
	Task<List<AppUser>> GetMembersOnProjectAsync(int projectId);
	Task<bool> AssignProjectManagerAsync(string projectManagerId, int projectId);
	Task<bool> RemoveProjectManagerAsync(int projectId);
	Task<bool> AssignEmployeeToProjectAsync(string employeeId, int projectId);
	Task<bool> RemoveEmployeeFromProjectAsync(string employeeId, int projectId);
	Task<bool> RemoveEmployeeFromAllProjectsAsync(int companyId, string employeeId);
	Task UpdateProjectAsync(Project project);
	Task<bool> ArchiveProjectAsync(int projectId);
	Task<bool> UnarchiveProjectAsync(int projectId);
}
