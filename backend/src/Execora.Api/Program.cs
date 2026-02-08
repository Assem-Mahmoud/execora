using Execora.Api.Filters;
using Execora.Api.Middleware;
using Execora.Application.Services;
using Execora.Auth.Services;
using Execora.Core.Interfaces;
using Execora.Infrastructure.Data;
using Execora.Infrastructure.Identity;
using Execora.Infrastructure.Repositories;
using Execora.Infrastructure.Services.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

var app = builder.Build();

// Configure the HTTP request pipeline
ConfigurePipeline(app, app.Environment);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
{
    // Database
    services.AddDbContext<ExecoraDbContext>(options =>
    {
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

        // Enable sensitive data logging in development
        if (environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    });

    // ASP.NET Core Identity
    services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.RequireUniqueEmail = true;

        // Sign-in settings
        options.SignIn.RequireConfirmedEmail = false; // Can be enabled for production
        options.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddEntityFrameworkStores<ExecoraDbContext>()
    .AddDefaultTokenProviders();

    // JWT Authentication
    var jwtSection = configuration.GetSection("Jwt");
    var secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"] ?? "Execora",
            ValidAudience = jwtSection["Audience"] ?? "ExecoraApp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

    // Authorization
    services.AddExecoraAuthorization();
    services.AddExecoraAuthorizationHandlers();

    // CORS
    services.AddCors(options =>
    {
        options.AddPolicy("DefaultCors", policy =>
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? new[] { "http://localhost:4200" };

            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    // API Controllers with Global Exception Filter
    services.AddGlobalExceptionFilter();

    // API Explorer and Swagger
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "EXECORA API",
            Version = "v1",
            Description = "Construction Execution Operating System API",
            Contact = new OpenApiContact
            {
                Name = "EXECORA",
                Email = "support@execora.com"
            }
        });

        // Include XML comments if available
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }

        // JWT Authentication for Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // Register Application Services
    services.AddScoped<ITokenService, JwtTokenService>();
    services.AddScoped<IRefreshTokenService, RefreshTokenService>();
    services.AddScoped<IAuditLogService, AuditLogService>();

    // Register Auth Services (Phase 2)
    services.AddScoped<IPasswordHasher, PasswordHasher>();

    // Register Repositories
    services.AddScoped<ITenantRepository, TenantRepository>();
    services.AddScoped<IUserRepository, UserRepository>();

    // Register Additional Repositories (Phase 2)
    services.AddScoped<IInvitationRepository, InvitationRepository>();
    services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    services.AddScoped<IAuditLogRepository, AuditLogRepository>();
    services.AddScoped<IPasswordHistoryRepository, PasswordHistoryRepository>();

    // Register Email Service (Phase 2)
    services.AddScoped<IEmailService, SmtpEmailService>();

    // Health Checks
    services.AddHealthChecks()
        .AddDbContextCheck<ExecoraDbContext>("database");
}

void ConfigurePipeline(WebApplication app, IHostEnvironment environment)
{
    // Global exception handling should be first
    // Note: Handled by filter, not middleware

    // Use CORS
    app.UseCors("DefaultCors");

    // Use HTTPS redirection in production
    if (!environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
        app.UseHsts();
    }

    // Rate limiting middleware (Phase 2)
    app.UseMiddleware<RateLimitMiddleware>();

    // Tenant resolution middleware (must be before authentication)
    app.UseTenantResolution();

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Swagger/OpenAPI
    if (environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "EXECORA API v1");
            options.RoutePrefix = "swagger";
        });
    }

    // Health check endpoint
    app.MapHealthChecks("/health");

    // Map controllers
    app.MapControllers();

    // Root endpoint
    app.MapGet("/", () => new
    {
        Name = "EXECORA API",
        Version = "1.0.0",
        Status = "Running",
        Timestamp = DateTime.UtcNow
    });
}
