using BugBanisher.Models;

namespace BugBanisher.Services.Interfaces;

public interface ICompanyService
{
    Task<Company?> GetCompanyByIdAsync(int id);
    Task<List<AppUser>> GetAllEmployeesAsync(int companyId);
    Task<bool> RemoveEmployeeAsync(string employeeId);
    Task<List<AppUser>> GetAllProjectManagersAsync(int companyId);
    Task<List<AppUser>> GetAllDevelopersAsync(int companyId);
    Task<List<AppUser>> GetAllMembersAsync(int companyId);
}
