using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BugBanisher.Database;
using BugBanisher.Models;
using BugBanisher.Models.Enums;
using BugBanisher.Services.Interfaces;

namespace BugBanisher.Services;

public class CompanyService : ICompanyService
{
    private readonly BugBanisherContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CompanyService(BugBanisherContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Company?> GetCompanyByIdAsync(int id)
    {
        return await _context.Companies.Where(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<AppUser>> GetAllEmployeesAsync(int companyId)
    {
        return await _context.Users
            .Where(u => u.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<bool> RemoveEmployeeAsync(string employeeId)
    {
        AppUser? employeeToRemove = await _context.Users.FirstOrDefaultAsync(u => u.Id == employeeId);

        if (employeeToRemove is null)
            return false;

        employeeToRemove.CompanyId = null;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<AppUser>> GetAllProjectManagersAsync(int companyId)
    {
        List<AppUser> employees = await GetAllEmployeesAsync(companyId);

        List<AppUser> projectManagers = new List<AppUser>();

        foreach (AppUser user in employees)
        {
            if (await _userManager.IsInRoleAsync(user, nameof(Roles.ProjectManager)))
                projectManagers.Add(user);
        }

        return projectManagers;
    }

    public async Task<List<AppUser>> GetAllDevelopersAsync(int companyId)
    {
        List<AppUser> employees = await GetAllEmployeesAsync(companyId);

        List<AppUser> developers = new List<AppUser>();

        foreach (AppUser user in employees)
        {
            if (await _userManager.IsInRoleAsync(user, nameof(Roles.Developer)))
                developers.Add(user);
        }

        return developers;
    }

    public async Task<List<AppUser>> GetAllMembersAsync(int companyId)
    {
        List<AppUser> employees = await GetAllEmployeesAsync(companyId);

        List<AppUser> members = new List<AppUser>();

        foreach (AppUser user in employees)
        {
            if (await _userManager.IsInRoleAsync(user, nameof(Roles.Member)))
                members.Add(user);
        }

        return members;
    }


}
