# EXECORA — Developer Quickstart Guide

**Branch**: `master` | **Last Updated**: 2025-02-03

---

## Overview

This guide helps developers get started with EXECORA development. The platform consists of:

- **Backend API**: .NET 9.0 Web API
- **Frontend SPA**: Angular 19 application
- **Database**: SQL Server 2022 with EF Core 9.0
- **BIM Integration**: Autodesk Platform Services (APS)

---

## Prerequisites

### Required Software

| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | 9.0+ | Backend API |
| Node.js | 20.x+ | Frontend build tooling |
| Angular CLI | 19.x+ | Frontend scaffolding |
| SQL Server | 2022+ | Local development database |
| Azure CLI | Latest | Azure deployment (optional) |
| Git | Latest | Version control |

### Recommended Tools

- Visual Studio 2022 or Visual Studio Code
- Azure Data Studio (for database management)
- Postman or Insomnia (for API testing)
- Devolutions Azure Storage Explorer (for Blob Storage)

---

## Repository Structure

```text
EXECORA/
├── backend/                    # .NET Web API
│   ├── src/
│   │   ├── Execora.Api/        # Main API project
│   │   ├── Execora.Core/       # Domain entities and interfaces
│   │   ├── Execora.Infrastructure/  # EF Core, external services
│   │   ├── Execora.Application/ # Business logic, services
│   │   └── Execora.Auth/       # Authentication & authorization
│   ├── tests/
│   │   ├── Unit/
│   │   └── Integration/
│   └── Execora.sln
│
├── frontend/                   # Angular SPA
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/           # Singleton services, guards
│   │   │   ├── shared/         # Shared components, pipes
│   │   │   ├── features/       # Feature modules
│   │   │   │   ├── sys-admin/ # System admin UI
│   │   │   │   ├── projects/  # Project management
│   │   │   │   ├── activities/ # Activities
│   │   │   │   ├── inspections/ # Quality
│   │   │   │   ├── issues/     # Issues
│   │   │   │   ├── ncr/        # NCR management
│   │   │   │   ├── daily-ops/  # Daily reports
│   │   │   │   ├── bim/        # BIM viewer
│   │   │   │   └── dashboards/ # Analytics
│   │   │   └── auth/           # Authentication
│   │   ├── assets/
│   │   └── environments/
│   └── angular.json
│
├── database/                   # Database scripts
│   ├── migrations/
│   └── seeds/
│
├── docs/                       # Additional documentation
├── specs/                      # This folder
│   └── master/
│       ├── spec.md
│       ├── plan.md
│       ├── research.md
│       ├── data-model.md
│       ├── quickstart.md
│       └── contracts/
│
└── deploy/                     # Deployment configurations
    ├── azure/
    └── docker/
```

---

## Local Development Setup

### 1. Clone Repository

```bash
git clone https://github.com/your-org/execora.git
cd execora
```

### 2. Backend Setup

```bash
cd backend

# Restore dependencies
dotnet restore

# Create appsettings.Development.json (see template below)

# Create database
dotnet ef database update --project src/Execora.Infrastructure --startup-project src/Execora.Api

# Run backend
dotnet run --project src/Execora.Api
```

**Backend will run on**: `https://localhost:5001`

### 3. Frontend Setup

```bash
cd frontend

# Install dependencies
npm install

# Configure environment (see environments/ below)

# Run development server
ng serve
```

**Frontend will run on**: `http://localhost:4200`

---

## Configuration Files

### Backend: appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ExecoraDev;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "your-super-secret-key-at-least-256-bits",
    "Issuer": "execora-dev",
    "Audience": "execora-app",
    "ExpiryMinutes": 1440
  },
  "AzureStorage": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "ContainerName": "execora-files"
  },
  "Autodesk": {
    "ClientId": "your-aps-client-id",
    "ClientSecret": "your-aps-client-secret",
    "Bucket": "execora-bim-dev"
  },
  "Email": {
    "SendGridApiKey": "your-sendgrid-key",
    "FromEmail": "noreply@execora.dev",
    "FromName": "EXECORA Dev"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Frontend: src/environments/environment.ts

```typescript
export const environment = {
  production: false,
  apiBaseUrl: 'https://localhost:5001/api',
  signalrUrl: 'https://localhost:5001/hubs',
  apsClientId: 'your-aps-client-id',
  version: '1.0.0',
  brand: {
    primaryColor: '#B11226',
    secondaryColor: '#1F2937',
    accentColor: '#2563EB'
  }
};
```

---

## Database Management

### Run Migrations

```bash
# Create new migration
dotnet ef migrations add AddInspectionTables --project src/Execora.Infrastructure --startup-project src/Execora.Api

# Apply migrations
dotnet ef database update --project src/Execora.Infrastructure --startup-project src/Execora.Api

# Rollback to specific migration
dotnet ef database update [MigrationName] --project src/Execora.Infrastructure --startup-project src/Execora.Api
```

### Seed Data

```bash
dotnet user-seeds set --project src/Execora.Infrastructure
```

---

## Authentication Flow

### 1. Get Token

```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@execora.com",
    "password": "P@ssw0rd!"
  }'
```

**Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "55007401-...",
  "expiresAt": "2025-02-04T10:30:00Z",
  "user": {
    "id": "...",
    "email": "admin@execora.com",
    "firstName": "System",
    "lastName": "Admin"
  },
  "tenants": [
    {
      "id": "...",
      "name": "Demo Organization",
      "slug": "demo",
      "role": "SystemAdmin"
    }
  ]
}
```

### 2. Use Token

```bash
curl https://localhost:5001/api/app/projects \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..." \
  -H "X-Tenant-ID: your-tenant-id"
```

---

## Key Development Patterns

### Backend: Service Pattern

```csharp
// Interface in Application layer
public interface IProjectService
{
    Task<ProjectDto> GetByIdAsync(Guid id);
    Task<IEnumerable<ProjectDto>> GetByTenantAsync(Guid tenantId);
}

// Implementation
public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<ProjectDto> GetByIdAsync(Guid id)
    {
        var project = await _repository.GetByIdAsync(id);
        return project.ToDto();
    }
}
```

### Backend: Controller Pattern

```csharp
[ApiController]
[Route("api/app/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProjectDto>>> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var tenantId = User.GetTenantId();
        var result = await _projectService.GetPagedAsync(tenantId, pageNumber, pageSize);
        return Ok(result);
    }
}
```

### Frontend: Service Pattern

```typescript
@Injectable({ providedIn: 'root' })
export class ProjectService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;

  getProjects(params: ProjectQueryParams): Observable<PagedResult<Project>> {
    return this.http.get<PagedResult<Project>>(
      `${this.baseUrl}/app/projects`,
      { params: toHttpParams(params) }
    );
  }
}
```

### Frontend: Component Pattern

```typescript
@Component({
  selector: 'ex-project-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="project-grid">
      <ex-project-card
        *ngFor="let project of projects()"
        [project]="project"
        (view)="onViewProject($event)"
      />
    </div>
  `
})
export class ProjectListComponent {
  private projectService = inject(ProjectService);
  private router = inject(Router);

  projects = signal<Project[]>([]);

  ngOnInit() {
    this.loadProjects();
  }

  private loadProjects() {
    this.projectService.getProjects({}).subscribe(
      projects => this.projects.set(projects.items)
    );
  }
}
```

---

## Testing

### Backend Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific project
dotnet test src/Execora.Tests.Unit
```

### Frontend Tests

```bash
# Run unit tests
ng test

# Run e2e tests
ng e2e

# Run with coverage
ng test --code-coverage
```

---

## Common Development Tasks

### Adding a New Feature Module

1. **Backend**: Create feature folder in `src/Execora.Application/Features/`
2. **Frontend**: Create feature module in `frontend/src/app/features/`
3. **API**: Add controller with routes
4. **Tests**: Add unit and integration tests

### Adding a New Entity

1. **Domain**: Add entity to `src/Execora.Core/Entities/`
2. **DbContext**: Add DbSet in `ExecoraDbContext`
3. **Repository**: Add repository interface and implementation
4. **Migration**: Run `dotnet ef migrations add`
5. **DTOs**: Add request/response DTOs

### Adding API Endpoint

1. Add method to service interface
2. Implement in service class
3. Add endpoint in controller
4. Add integration test
5. Update OpenAPI spec in `specs/master/contracts/`

---

## Troubleshooting

### Common Issues

| Issue | Solution |
|-------|----------|
| CORS errors | Configure CORS in `Program.cs` to allow frontend origin |
| Tenant not found | Ensure `X-Tenant-ID` header is sent with requests |
| JWT not validated | Check JWT configuration matches between backend and frontend |
| Migration fails | Ensure connection string is correct and SQL Server is running |
| BIM viewer not loading | Check APS credentials and token generation |

### Debug Tips

```bash
# Backend: Enable verbose logging
dotnet run --project src/Execora.Api --verbose

# Frontend: Debug mode
ng serve --configuration=development --source-map

# Database: Log SQL queries
# In DbContext: optionsBuilder.LogTo(Console.WriteLine)
```

---

## Next Steps

1. Review the full specification: `specs/master/spec.md`
2. Study the data model: `specs/master/data-model.md`
3. Review API contracts: `specs/master/contracts/`
4. Set up your local environment following the steps above
5. Check out the implementation plan: `specs/master/plan.md`

---

## Support

- **Documentation**: See `specs/` directory
- **Issues**: Create issue in GitHub repository
- **Team Contact**: See project README.md

---

**END OF QUICKSTART GUIDE**
