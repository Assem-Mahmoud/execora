using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Execora.Infrastructure.Data;

namespace Execora.Api.Tests.Helpers;

/// <summary>
/// Helper class for integration tests
/// </summary>
public static class TestingHelper
{
    /// <summary>
    /// Configure the test application with in-memory database
    /// </summary>
    public static WebApplicationFactory<Program> CreateWithInMemoryDb()
    {
        var factory = new WebApplicationFactory<Program>();

        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ExecoraDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add test database
                services.AddDbContext<ExecoraDbContext>(options =>
                    options.UseInMemoryDatabase("ExecoraTestDb"));
            });
        });
    }
}