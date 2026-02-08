using Execora.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Execora.Auth.Services;

/// <summary>
/// Authorization requirement for tenant roles
/// </summary>
public class TenantRoleRequirement : IAuthorizationRequirement
{
    public TenantRole[] RequiredRoles { get; }

    public TenantRoleRequirement(params TenantRole[] roles)
    {
        RequiredRoles = roles;
    }
}

/// <summary>
/// Authorization handler for tenant roles
/// </summary>
public class TenantRoleAuthorizationHandler : AuthorizationHandler<TenantRoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantRoleRequirement requirement)
    {
        var roleClaim = context.User.FindFirst("tenant_role");

        if (roleClaim != null && Enum.TryParse<TenantRole>(roleClaim.Value, out var userRole))
        {
            if (requirement.RequiredRoles.Contains(userRole))
            {
                context.Succeed(requirement);
            }
            else
            {
                // Check if user has higher privilege role
                if (IsHigherPrivilegeRole(userRole, requirement.RequiredRoles))
                {
                    context.Succeed(requirement);
                }
            }
        }

        return Task.CompletedTask;
    }

    private static bool IsHigherPrivilegeRole(TenantRole userRole, TenantRole[] requiredRoles)
    {
        // Define role hierarchy (lower number = higher privilege)
        var roleHierarchy = new Dictionary<TenantRole, int>
        {
            { TenantRole.SystemAdmin, 1 },
            { TenantRole.TenantAdmin, 2 },
            { TenantRole.ProjectAdmin, 3 },
            { TenantRole.ProjectManager, 4 },
            { TenantRole.QAQC, 5 },
            { TenantRole.SiteEngineer, 6 }
        };

        var userLevel = roleHierarchy.GetValueOrDefault(userRole, int.MaxValue);
        return requiredRoles.Any(required => roleHierarchy.GetValueOrDefault(required, int.MaxValue) >= userLevel);
    }
}

/// <summary>
/// Authorization requirement for system admin access
/// </summary>
public class SystemAdminRequirement : IAuthorizationRequirement
{
}

/// <summary>
/// Authorization handler for system admin
/// </summary>
public class SystemAdminAuthorizationHandler : AuthorizationHandler<SystemAdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SystemAdminRequirement requirement)
    {
        var roleClaim = context.User.FindFirst("tenant_role");

        if (roleClaim != null && Enum.TryParse<TenantRole>(roleClaim.Value, out var userRole))
        {
            if (userRole == TenantRole.SystemAdmin)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Extension methods for configuring EXECORA authorization
/// </summary>
public static class AuthorizationServiceExtensions
{
    public static IServiceCollection AddExecoraAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // System admin policy - only for platform administrators
            options.AddPolicy("SystemAdmin", policy =>
                policy.Requirements.Add(new SystemAdminRequirement()));

            // Tenant admin policy - can manage tenant settings
            options.AddPolicy("TenantAdmin", policy =>
                policy.Requirements.Add(new TenantRoleRequirement(
                    TenantRole.TenantAdmin, TenantRole.SystemAdmin)));

            // Project management policy
            options.AddPolicy("ProjectManagement", policy =>
                policy.Requirements.Add(new TenantRoleRequirement(
                    TenantRole.ProjectAdmin, TenantRole.ProjectManager,
                    TenantRole.TenantAdmin, TenantRole.SystemAdmin)));

            // Project execution policy
            options.AddPolicy("ProjectExecution", policy =>
                policy.Requirements.Add(new TenantRoleRequirement(
                    TenantRole.ProjectManager, TenantRole.SiteEngineer,
                    TenantRole.ProjectAdmin, TenantRole.TenantAdmin, TenantRole.SystemAdmin)));

            // Quality management policy
            options.AddPolicy("QualityManagement", policy =>
                policy.Requirements.Add(new TenantRoleRequirement(
                    TenantRole.QAQC, TenantRole.ProjectManager,
                    TenantRole.ProjectAdmin, TenantRole.TenantAdmin, TenantRole.SystemAdmin)));
        });

        return services;
    }

    public static IServiceCollection AddExecoraAuthorizationHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, TenantRoleAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, SystemAdminAuthorizationHandler>();

        return services;
    }
}
