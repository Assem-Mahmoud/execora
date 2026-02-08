# Developer Quickstart: Multi-Tenant Authentication & User Management

**Feature**: 001-multi-tenant-auth
**Last Updated**: 2025-02-03

This guide helps you set up a local development environment for implementing the Multi-Tenant Authentication & User Management feature.

---

## Prerequisites

### Required Software

| Software | Version | Purpose |
|----------|---------|---------|
| .NET SDK | 9.0+ | Backend API development |
| SQL Server | 2022+ | Local database |
| Node.js | 20+ | Frontend build tools |
| Angular CLI | 19+ | Frontend development |
| Git | Latest | Version control |

### Verification

```bash
# Verify .NET SDK
dotnet --version

# Verify Node.js
node --version

# Verify Angular CLI
ng version

# Verify SQL Server (running)
# Check SQL Server Configuration Manager or Services
```

---

## Repository Setup

### 1. Clone and Configure

```bash
# Clone the repository (if not already done)
git clone https://github.com/Assem-Mahmold/execora.git
cd execora

# Checkout the feature branch
git checkout 001-multi-tenant-auth
```

### 2. Backend Configuration

```bash
# Navigate to backend directory
cd backend

# Restore NuGet packages
dotnet restore

# Set up user secrets for configuration
dotnet user-secrets init --project src/Execora.Api
```

#### Configuration (appsettings.json)

Create `src/Execora.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ExecoraDev;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Issuer": "https://api-dev.execora.com",
    "Audience": "https://api-dev.execora.com",
    "SecretKey": "YOUR_DEV_SECRET_KEY_MIN_32_CHARS",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "Email": {
    "SmtpServer": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@example.com",
    "SmtpPassword": "your-password",
    "FromEmail": "noreply@execora.com",
    "FromName": "EXECORA"
  },
  "RateLimit": {
    "LoginAttemptsPer15Minutes": 5,
    "RegistrationAttemptsPerHour": 3,
    "PasswordResetAttemptsPerHour": 3
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 3. Database Setup

```bash
# Run EF migrations to create database schema
cd src/Execora.Infrastructure
dotnet ef database update --startup-project ../Execora.Api

# Or create initial migration if needed
dotnet ef migrations add InitialCreate --startup-project ../Execora.Api
dotnet ef database update --startup-project ../Execora.Api
```

#### Verify Database

```sql
-- Check tables exist
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Should see: Tenants, Users, TenantUsers, Invitations, RefreshTokens, AuditLogs
```

### 4. Frontend Configuration

```bash
# Navigate to frontend directory
cd ../../frontend

# Install dependencies
npm install

# Create environment file
cp src/environments/environment.example.ts src/environments/environment.ts
```

#### Environment Configuration

Edit `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api',
  authUrl: 'https://localhost:5001/api/auth',
  appUrl: 'https://localhost:5001/api/app',
  clientId: 'execora-web'
};
```

---

## Running the Application

### Backend

```bash
# From backend directory
cd backend

# Run the API
dotnet run --project src/Execora.Api

# API will be available at:
# https://localhost:5001 (HTTP)
# https://localhost:5001 (HTTPS - dev certificate required)
```

#### Trust Dev Certificate (first time only)

```bash
# Trust the ASP.NET Core HTTPS development certificate
dotnet dev-certs https --trust
```

### Frontend

```bash
# From frontend directory
cd frontend

# Run Angular development server
ng serve

# Application will be available at:
# http://localhost:4200
```

---

## Development Workflow

### 1. Creating New Entities

When adding entities (like `Invitation`, `RefreshToken`, `AuditLog`):

1. **Add entity to `Execora.Core`**:
   ```csharp
   // backend/src/Execora.Core/Entities/Invitation.cs
   namespace Execora.Core.Entities;
   public class Invitation : BaseEntity
   {
       // Properties...
   }
   ```

2. **Add enum if needed**:
   ```csharp
   // backend/src/Execora.Core/Enums/InvitationStatus.cs
   namespace Execora.Core.Enums;
   public enum InvitationStatus { Pending, Accepted, Expired, Cancelled }
   ```

3. **Add repository interface**:
   ```csharp
   // backend/src/Execora.Core/Interfaces/IInvitationRepository.cs
   ```

4. **Update `AppDbContext`**:
   ```csharp
   // backend/src/Execora.Infrastructure/Data/AppDbContext.cs
   public DbSet<Invitation> Invitations { get; set; }
   ```

5. **Create migration**:
   ```bash
   dotnet ef migrations add AddInvitationEntity --startup-project src/Execora.Api
   dotnet ef database update --startup-project src/Execora.Api
   ```

### 2. Creating API Endpoints

1. **Create DTOs in `Execora.Application`**:
   ```csharp
   // backend/src/Execora.Application/DTOs/InvitationCreateRequest.cs
   ```

2. **Create service in `Execora.Application`**:
   ```csharp
   // backend/src/Execora.Application/Services/IInvitationService.cs
   // backend/src/Execora.Application/Services/InvitationService.cs
   ```

3. **Create controller in `Execora.Api`**:
   ```csharp
   // backend/src/Execora.Api/Controllers/User/InvitationController.cs
   [ApiController]
   [Route("api/app/users")]
   public class InvitationController : ControllerBase
   {
       // Endpoints...
   }
   ```

4. **Update OpenAPI/Swagger** (annotations or XML comments)

### 3. Creating Frontend Features

1. **Generate feature module**:
   ```bash
   ng generate module features/invitations --module app.module
   ```

2. **Generate components**:
   ```bash
   ng generate component features/invitations/invite-user
   ng generate component features/invitations/invitation-list
   ```

3. **Generate service**:
   ```bash
   ng generate service features/invitations/invitation
   ```

4. **Add routing**:
   ```typescript
   // frontend/src/app/features/invitations/invitations.routes.ts
   export const INVITATION_ROUTES: Routes = [
     { path: 'users/invite', component: InviteUserComponent, canActivate: [AuthGuard] },
     { path: 'users/invitations', component: InvitationListComponent, canActivate: [AuthGuard] }
   ];
   ```

---

## Testing

### Backend Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/Execora.Tests.Unit
dotnet test tests/Execora.Tests.Integration
```

### Frontend Tests

```bash
# Unit tests
ng test

# E2E tests
ng e2e

# Component tests (if using standalone)
ng test --watch
```

---

## Troubleshooting

### Port Already in Use

```bash
# Windows: Find process using port 5001
netstat -ano | findstr :5001

# Kill the process
taskkill /PID <pid> /F

# Or use a different port in launchSettings.json
```

### Database Connection Issues

1. Verify SQL Server is running
2. Check connection string format
3. Ensure firewall allows local connections
4. Try `TrustServerCertificate=True` for dev

### CORS Errors

Add `cors` middleware configuration in `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

app.UseCors();
```

### Email Not Sending (Development)

For local development, emails may not send. Options:
1. Use a real SMTP server (SendGrid, Mailgun)
2. Use a test SMTP server (MailHog, Papercut)
3. Log emails to console instead of sending

---

## Useful Commands

### Backend

```bash
# Clean and rebuild
dotnet clean && dotnet build

# Watch for changes and hot reload
dotnet watch --project src/Execora.Api

# Format code
dotnet format

# Analyze for code quality
dotnet analyze
```

### Frontend

```bash
# Clean build cache
rm -rf node_modules/.cache
npm cache clean --force

# Format code
npx prettier --write "src/**/*.ts"

# Lint code
ng lint

# Build for production
ng build --configuration production
```

---

## API Documentation

Once running, Swagger UI is available at:
```
https://localhost:5001/swagger
```

OpenAPI specifications are in:
- `specs/001-multi-tenant-auth/contracts/auth-api.yaml`
- `specs/001-multi-tenant-auth/contracts/invitation-api.yaml`

---

## Next Steps

1. Review the implementation plan: `specs/001-multi-tenant-auth/plan.md`
2. Review the feature specification: `specs/001-multi-tenant-auth/spec.md`
3. Run `/speckit.tasks` to generate implementation tasks
4. Start with Phase 2.1: Core Authentication

---

**END OF QUICKSTART**
