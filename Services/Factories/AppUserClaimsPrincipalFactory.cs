using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using BugBanisher.Models;

namespace BugBanisher.Services.Factories;

public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, IdentityRole>
{
    public AppUserClaimsPrincipalFactory(UserManager<AppUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
    {

    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
    {
        if (!user.CompanyId.HasValue)
        {
            return await base.GenerateClaimsAsync(user);
        }

        ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
        Claim companyClaim = new Claim("Company", user.CompanyId.Value.ToString());
        identity.AddClaim(companyClaim);

        return identity;
    }
}
