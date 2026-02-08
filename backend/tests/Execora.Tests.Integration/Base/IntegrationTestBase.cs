using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Execora.Tests.Integration.Base;

/// <summary>
/// Base class for integration tests
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> _factory;
    protected readonly HttpClient _client;

    public IntegrationTestBase(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = CreateClient();
    }

    protected HttpClient CreateClient()
    {
        return _factory.WithWebHostBuilder(builder =>
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
        }).CreateClient();
    }

    protected T GetRequiredService<T>()
    {
        using var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }
}