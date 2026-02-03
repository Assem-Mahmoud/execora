using Execora.Core.Entities;
using Execora.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Execora.Infrastructure.Identity;

/// <summary>
/// Custom user claims principal factory that adds tenant-specific claims
/// </summary>
public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser>
{
    private readonly UserManager<IdentityUser> _userManager;

    public AppUserClaimsPrincipalFactory(
        UserManager<IdentityUser> userManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
    {
        _userManager = userManager;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Note: Tenant-specific claims are added during login via JWT generation
        // This factory is for the default IdentityUser used by ASP.NET Core Identity

        return identity;
    }
}
