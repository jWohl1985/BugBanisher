using Microsoft.AspNetCore.Identity;
using BugBanisher.Models;

namespace BugBanisher.Services.Interfaces;

public interface IRoleService
{
    Task<bool> IsUserInRoleAsync(AppUser user, string roleName);

    Task<List<IdentityRole>> GetRolesAsync();

    Task<IEnumerable<string>> GetUserRolesAsync(AppUser user);

    Task<bool> AddUserToRoleAsync(AppUser user, string roleName);

    Task<bool> RemoveUserFromRoleAsync(AppUser user, string roleName);

    Task<bool> RemoveUserFromRolesAsync(AppUser user, IEnumerable<string> roles);

    Task<List<AppUser>> GetUsersInRoleAsync(string roleName, int companyId);

    Task<List<AppUser>> GetUsersNotInRoleAsync(string roleName, int companyId);

    Task<string?> GetRoleNameByIdAsync(string roleId);
}
