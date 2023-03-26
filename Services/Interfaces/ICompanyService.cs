using BugBanisher.Models;

namespace BugBanisher.Services.Interfaces;

public interface ICompanyService
{
    Task<Company?> GetCompanyByIdAsync(int id);
    Task<List<AppUser>> GetAllEmployeesAsync(int companyId);
    Task<bool> RemoveEmployeeAsync(string employeeId);
    Task<List<AppUser>> GetAllAdminsAsync(int companyId);
    Task<List<AppUser>> GetAllProjectManagersAsync(int companyId);
    Task<List<AppUser>> GetAllDevelopersAsync(int companyId);
    Task<List<AppUser>> GetAllMembersAsync(int companyId);
    Task<AppUser?> UpdateEmployeeProfileAsync(AppUser updatedUser);
    Task DeleteProfilePictureAsync(string userId);
    Task<bool> AddEmployeeAsync(AppUser appUser, int companyId);
}
