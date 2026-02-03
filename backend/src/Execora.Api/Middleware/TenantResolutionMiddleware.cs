using System.Net;
using System.Text.Json;

namespace Execora.Api.Middleware;

/// <summary>
/// Middleware to resolve the current tenant from HTTP requests.
/// Tenant can be identified via:
/// 1. Header: X-Tenant-Id or X-Tenant-Slug
/// 2. JWT Claim: tenant_id
/// 3. Query parameter: tenant_id (for system operations)
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    private static readonly string[] TenantHeaders = new[]
    {
        "X-Tenant-Id",
        "X-Tenant-Slug"
    };

    public TenantResolutionMiddleware(
        RequestDelegate next,
        ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip tenant resolution for system admin endpoints
        if (IsSystemAdminEndpoint(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Skip tenant resolution for public endpoints (login, register, etc.)
        if (IsPublicEndpoint(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Try to resolve tenant from various sources
        var tenantId = ResolveTenantId(context);

        if (tenantId.HasValue)
        {
            // Store tenant ID in HttpContext for use in downstream services
            context.Items["TenantId"] = tenantId.Value.ToString();
            await _next(context);
        }
        else
        {
            _logger.LogWarning("Tenant could not be resolved for request: {Path}", context.Request.Path);
            await WriteTenantNotFoundError(context);
        }
    }

    private Guid? ResolveTenantId(HttpContext context)
    {
        // 1. Try from JWT claims (if authenticated)
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("tenant_id");
            if (tenantClaim != null && Guid.TryParse(tenantClaim.Value, out var tenantIdFromClaim))
            {
                return tenantIdFromClaim;
            }
        }

        // 2. Try from headers
        foreach (var header in TenantHeaders)
        {
            if (context.Request.Headers.TryGetValue(header, out var headerValue))
            {
                if (Guid.TryParse(headerValue.ToString(), out var tenantIdFromHeader))
                {
                    return tenantIdFromHeader;
                }
            }
        }

        // 3. Try from query string (less preferred, for system operations)
        if (context.Request.Query.TryGetValue("tenant_id", out var queryValue))
        {
            if (Guid.TryParse(queryValue.ToString(), out var tenantIdFromQuery))
            {
                return tenantIdFromQuery;
            }
        }

        return null;
    }

    private static bool IsSystemAdminEndpoint(string path)
    {
        return path.StartsWith("/api/sys", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPublicEndpoint(string path)
    {
        var publicPaths = new[]
        {
            "/api/auth",
            "/health",
            "/.well-known"
        };

        return publicPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }

    private static async Task WriteTenantNotFoundError(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        var error = new
        {
            Type = "TenantResolutionError",
            Message = "Tenant could not be resolved from the request. Please provide X-Tenant-Id header or authenticate with a valid token.",
            Code = "TENANT_NOT_RESOLVED"
        };

        var json = JsonSerializer.Serialize(error, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Extension methods for TenantResolutionMiddleware
/// </summary>
public static class TenantResolutionMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantResolutionMiddleware>();
    }
}
