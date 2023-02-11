using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BugBanisher.Database;
using BugBanisher.Models;
using BugBanisher.Services.Interfaces;

namespace ProjectManager.Services;

public class RoleService : IRoleService
{
    private readonly BugBanisherContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public RoleService(BugBanisherContext context, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
    {
        _context = context;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<bool> AddUserToRoleAsync(AppUser user, string roleName)
    {
        return (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;
    }

    public async Task<string?> GetRoleNameByIdAsync(string roleId)
    {
        IdentityRole? role = _context.Roles.Find(roleId);

        if (role is null)
            return null;

        return await _roleManager.GetRoleNameAsync(role);
    }

    public async Task<List<IdentityRole>> GetRolesAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(AppUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<List<AppUser>> GetUsersInRoleAsync(string roleName, int companyId)
    {
        List<AppUser> companyMembers = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

        List<AppUser> usersInRole = new List<AppUser>();

        foreach (AppUser user in companyMembers)
        {
            if (await _userManager.IsInRoleAsync(user, roleName))
                usersInRole.Add(user);
        }

        return usersInRole;
    }

    public async Task<List<AppUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
    {
        List<AppUser> companyMembers = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

        List<AppUser> usersNotInRole = new List<AppUser>();

        foreach (AppUser user in companyMembers)
        {
            if (await _userManager.IsInRoleAsync(user, roleName))
                continue;
            else
                usersNotInRole.Add(user);
        }

        return usersNotInRole;
    }

    public async Task<bool> IsUserInRoleAsync(AppUser user, string roleName)
    {
        return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<bool> RemoveUserFromRoleAsync(AppUser user, string roleName)
    {
        return (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;
    }

    public async Task<bool> RemoveUserFromRolesAsync(AppUser user, IEnumerable<string> roles)
    {
        return (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;
    }
}
