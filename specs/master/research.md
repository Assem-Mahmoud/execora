# EXECORA — Technical Research & Decisions
## Phase 0: Research & Technology Selection

**Branch**: `master` | **Date**: 2025-02-03

---

## 1. Backend Technology Stack

### Decision: .NET 9.0 (Latest LTS)

**Rationale**:
- Enterprise-grade performance and reliability
- Excellent support for microservices architecture
- Built-in dependency injection and middleware pipeline
- Strong typing with C# 13 for complex business logic (Workflow Engine FSM)
- Superior async/await patterns for high-concurrency operations
- First-class Azure integration (App Service, Functions, Cosmos DB, SQL Database)
- Comprehensive EF Core 9.0 for complex data modeling
- Native support for OpenAPI/Swagger documentation

**Alternatives Considered**:
- **Node.js/NestJS**: Good for rapid prototyping, but less suitable for complex FSM workflow enforcement
- **Java/Spring**: Mature ecosystem, but .NET offers better Azure integration and cleaner async patterns
- **GoLang**: Excellent performance, but ecosystem maturity for enterprise CRUD applications is lower

---

## 2. Frontend Technology Stack

### Decision: Angular 19 (Latest)

**Rationale**:
- Enterprise-first architecture with strict typing (TypeScript 5.6)
- Comprehensive CLI for code generation and consistency
- Built-in routing, forms, and HTTP modules
- Standalone components for better tree-shaking
- Ivy compiler for optimal runtime performance
- Excellent state management patterns (RxJS, Signals)
- Mature testing infrastructure (Jasmine, Karma)
- Strong Angular Material ecosystem
- Offline capability via Angular Service Workers

**Alternatives Considered**:
- **React**: More flexible, but requires more architectural decisions for enterprise apps
- **Vue**: Simpler learning curve, but less opinionated for large teams
- **Blazor**: Promising, but ecosystem maturity and SPA patterns still evolving

---

## 3. UI/Styling Framework

### Decision: Tailwind CSS 4.x

**Rationale**:
- Utility-first approach for rapid UI development
- Excellent support for EXECORA brand colors (Crimson #B11226, Charcoal #1F2937, Insight Blue #2563EB)
- Built-in responsive design utilities
- Dark mode support for enterprise environments
- Small bundle sizes with JIT compilation
- Strong Angular integration via @angular/+tailwind (v19+)

**Alternatives Considered**:
- **Angular Material**: Robust component library, but harder to customize for brand consistency
- **Bootstrap**: Good starting point, but less flexible for custom enterprise designs
- **Sass/SCSS**: Powerful but requires writing more custom CSS

---

## 4. Database Architecture

### Decision: SQL Server 2022 with EF Core 9.0

**Rationale**:
- Relational data model fits construction domain (Projects, Activities, Inspections, NCRs)
- Strong transaction support for workflow state transitions
- Full-text search for inspection templates and issues
- Temporal tables for audit trail (native SQL Server feature)
- JSON column support for flexible workflow configuration storage
- Excellent Azure SQL Database compatibility
- Row-level security for multi-tenant isolation

**Alternatives Considered**:
- **PostgreSQL**: Excellent open-source option, but SQL Server has better Azure integration
- **MongoDB**: Flexible schema, but relationships are core to EXECORA's data model
- **Cosmos DB**: Great for global distribution, but overkill for initial MVP and costly

---

## 5. Multi-Tenancy Strategy

### Decision: Shared Database, Row-Level Security (RLS)

**Rationale**:
- Cost-effective for SaaS model (single database instance)
- SQL Server RLS enforces isolation at database level
- TenantId column on all tenant-specific tables
- Automatic tenant filtering via EF Core global query filters
- Easier maintenance and backup strategy
- Scales to thousands of tenants

**Alternatives Considered**:
- **Database per Tenant**: Complete isolation, but expensive and complex to manage
- **Schema per Tenant**: Middle ground, but still complex migrations

---

## 6. BIM Integration

### Decision: Autodesk Platform Services (APS) / Forge

**Rationale**:
- Industry standard for construction BIM viewers
- Supports IFC, Revit, and other common formats
- Model derivative API for web-ready formats
- Webhook support for model processing completion
- Element-level properties and color coding
- Authenticates via OAuth 2.0

**Key Integration Points**:
- Model upload and derivative processing
- Viewer initialization with token management
- Element selection and property extraction
- Snapshot generation for audit trail

---

## 7. Authentication & Authorization

### Decision: ASP.NET Core Identity + JWT + Role-Based Claims

**Rationale**:
- Built-in Identity framework for user management
- JWT tokens for stateless API authentication
- Claims-based authorization for granular permissions
- Multi-tenant claims (TenantId, Role, Permissions)
- Refresh token rotation for security
- OpenID Connect compatible (future SSO support)

**Role/Permission Structure**:
```
SystemAdmin: sys.*
TenantAdmin: app.org.*, app.project.*, app.user.*
ProjectAdmin: app.project.{id}.*
ProjectManager: app.project.{id}.execution.*
QAQC: app.project.{id}.quality.*
SiteEngineer: app.project.{id}.site.*
```

---

## 8. Workflow Engine Implementation

### Decision: Stateless FSM with Event Sourcing

**Rationale**:
- Finite State Machine pattern for clear state transitions
- Event sourcing for immutable audit trail
- Stateless design for scalability
- Workflow definition stored as JSON (versioned)
- Transition rules evaluated at runtime
- Saga pattern for multi-entity workflows (Inspection → NCR → Activity Lock)

**State Machine Example**:
```
Activity States: Draft → InProgress → ReadyForInspection → Approved | Rework → Complete
Inspection States: Scheduled → InProgress → Passed | Failed → Closed
NCR States: Open → UnderReview → CorrectiveAction → Verification → Closed
```

---

## 9. File Storage Strategy

### Decision: Azure Blob Storage

**Rationale**:
- Scalable object storage for photos, documents, BIM models
- Hot/Cool tier for cost optimization
- SAS tokens for secure temporary access
- CDN integration for faster delivery
- Immutable blob storage for audit attachments

---

## 10. Real-time Notifications

### Decision: Azure SignalR Service

**Rationale**:
- Real-time push notifications for web clients
- Automatic scaling built-in
- WebSocket fallback to Server-Sent Events
- Integrates with ASP.NET Core authentication
- Supports notification filtering by tenant/user

**Email Notifications**: SendGrid (Azure integration)

---

## 11. Testing Strategy

### Backend Testing
- **Unit Tests**: xUnit + Moq (service layer)
- **Integration Tests**: WebApplicationFactory (API endpoints)
- **Workflow Tests**: SpecFlow (BDD for FSM transitions)

### Frontend Testing
- **Unit Tests**: Jasmine/Karma (components, services)
- **E2E Tests**: Playwright (critical user flows)

---

## 12. Deployment Architecture

### Decision: Azure App Service + Azure SQL Database

**Components**:
- **Frontend**: Azure Static Web Apps (Angular SPA)
- **Backend API**: Azure App Service (ASP.NET Core)
- **Database**: Azure SQL Database (Single database + elastic pool options)
- **BIM Models**: Azure Blob Storage
- **Cache**: Azure Cache for Redis (session, distributed locks)
- **Search**: Azure Cognitive Search (full-text search on issues/inspections)

**CI/CD**: GitHub Actions

---

## 13. Performance Considerations

### Target Metrics
- API Response: <200ms p95 for standard CRUD
- Page Load: <2s initial load, <500ms subsequent navigation
- Concurrent Users: 1000+ simultaneous users per tenant
- Database Connections: Connection pooling with max 100 connections

### Optimization Strategies
- EF Core query optimization (NoTracking, split queries)
- Response caching for static data (lookup tables)
- CDN for static assets (Angular bundles, images)
- Database indexes on TenantId, ProjectId, WorkflowState

---

## 14. Security Measures

- HTTPS/TLS 1.3 enforced
- SQL injection prevention (EF Core parameterization)
- XSS prevention (Angular DOM sanitizer)
- CSRF tokens for state-changing operations
- API rate limiting (per tenant)
- Audit logging for all state transitions
- PII encryption at rest (SQL Server Always Encrypted)

---

## 15. Development Workflow

- **Source Control**: Git with feature branch workflow
- **Code Review**: Pull requests required
- **CI Pipeline**: Automated builds, tests, and linting
- **CD Pipeline**: Automatic deployment to staging, manual approval for production
- **Environment Strategy**: Dev → Staging → Production

---

**END OF RESEARCH**
