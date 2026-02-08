using System.Collections.Concurrent;
using System.Net;

namespace Execora.Api.Middleware;

/// <summary>
/// Middleware for rate limiting requests to prevent brute force attacks
/// </summary>
public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitMiddleware> _logger;
    private readonly ConcurrentDictionary<string, RateLimitCounter> _counters = new();
    private readonly RateLimitOptions _options;

    public RateLimitMiddleware(
        RequestDelegate next,
        ILogger<RateLimitMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _options = configuration.GetSection("RateLimit").Get<RateLimitOptions>() ?? new RateLimitOptions();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var path = context.Request.Path.Value ?? string.Empty;

        // Determine which rate limit to apply based on the endpoint
        var limit = GetRateLimitForPath(path);

        if (limit == null)
        {
            await _next(context);
            return;
        }

        var identifier = GetClientIdentifier(context);
        var counter = _counters.GetOrAdd(identifier, _ => new RateLimitCounter());

        lock (counter)
        {
            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-limit.WindowMinutes);

            // Clean up old entries
            var validRequests = counter.Requests
                .Where(r => r > windowStart)
                .ToList();

            // Clear and add valid requests
            counter.Requests.Clear();
            foreach (var req in validRequests)
            {
                counter.Requests.Add(req);
            }

            // Check if limit exceeded
            if (counter.Requests.Count >= limit.MaxRequests)
            {
                _logger.LogWarning("Rate limit exceeded for {Identifier} on {Path}. Count: {Count}, Limit: {Limit}",
                    identifier, path, counter.Requests.Count, limit.MaxRequests);

                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.Headers["Retry-After"] = Math.Ceiling((double)limit.WindowMinutes).ToString();
                return;
            }

            // Add current request
            counter.Requests.Add(now);
        }

        await _next(context);
    }

    private RateLimitEntry? GetRateLimitForPath(string path)
    {
        // Define rate limits for specific endpoints
        if (path.Contains("/auth/login", StringComparison.OrdinalIgnoreCase))
        {
            return new RateLimitEntry(_options.LoginAttemptsPer15Minutes, 15);
        }

        if (path.Contains("/auth/register", StringComparison.OrdinalIgnoreCase))
        {
            return new RateLimitEntry(_options.RegistrationAttemptsPerHour, 60);
        }

        if (path.Contains("/auth/forgot-password", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("/auth/reset-password", StringComparison.OrdinalIgnoreCase))
        {
            return new RateLimitEntry(_options.PasswordResetAttemptsPerHour, 60);
        }

        if (path.Contains("/users/invite", StringComparison.OrdinalIgnoreCase))
        {
            return new RateLimitEntry(10, 60); // 10 per hour per tenant
        }

        return null;
    }

    private static string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID from claims (for authenticated requests)
        var userIdClaim = context.User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrEmpty(userIdClaim))
        {
            return $"user:{userIdClaim}";
        }

        // Fall back to IP address
        var remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp != null)
        {
            return $"ip:{remoteIp}";
        }

        return "unknown";
    }

    private class RateLimitCounter
    {
        public List<DateTime> Requests { get; } = new();
    }

    private record RateLimitEntry(int MaxRequests, int WindowMinutes);
}

/// <summary>
/// Configuration options for rate limiting
/// </summary>
public class RateLimitOptions
{
    public int LoginAttemptsPer15Minutes { get; set; } = 5;
    public int RegistrationAttemptsPerHour { get; set; } = 3;
    public int PasswordResetAttemptsPerHour { get; set; } = 3;
}
