﻿using System.Security.Claims;
using System.Security.Principal;

namespace BugBanisher.Extensions;

public static class IdentityExtensions
{
    public static int GetCompanyId(this IIdentity identity)
    {
        Claim? claim = ((ClaimsIdentity)identity).FindFirst("Company");

        if (claim is null)
            return -1;

        return int.Parse(claim.Value);
    }
}
