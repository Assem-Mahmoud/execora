# Multi-Tenant Authentication & User Management — Technical Research

**Feature**: 001-multi-tenant-auth
**Branch**: `001-multi-tenant-auth` | **Date**: 2025-02-03

---

## Overview

This document summarizes technology decisions relevant to implementing Multi-Tenant Authentication & User Management. The comprehensive research is documented in the master specification.

---

## Backend Technology Stack

### Decision: .NET 9.0 (Latest LTS)

**Rationale**:
- Enterprise-grade performance and reliability
- Built-in dependency injection and middleware pipeline
- Strong typing with C# 13 for complex business logic
- Superior async/await patterns for high-concurrency operations
- First-class Azure integration
- Comprehensive EF Core 9.0 for complex data modeling
- Native support for OpenAPI/Swagger documentation

**Documentation**: [../master/research.md](../master/research.md#1-backend-technology-stack)

---

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

**Documentation**: [../master/research.md](../master/research.md#7-authentication--authorization)

---

### Decision: BCrypt.Net-Next for Password Hashing

**Rationale**:
- Proven, widely adopted algorithm
- Automatic work factor adjustment
- Built-in salt generation
- Resistant to rainbow table attacks
- .NET ecosystem standard

**Alternatives Considered**:
- **Argon2**: Stronger theoretically, but less ecosystem maturity
- **PBKDF2**: Built-in option, but BCrypt has better resistance to GPU attacks

---

## Frontend Technology Stack

### Decision: Angular 19 (Latest)

**Rationale**:
- Enterprise-first architecture with strict typing (TypeScript 5.6)
- Comprehensive CLI for code generation and consistency
- Built-in routing, forms, and HTTP modules
- Standalone components for better tree-shaking
- Ivy compiler for optimal runtime performance
- Strong Angular Material ecosystem
- Offline capability via Angular Service Workers

**Documentation**: [../master/research.md](../master/research.md#2-frontend-technology-stack)

---

### Decision: Tailwind CSS 4.x

**Rationale**:
- Utility-first approach for rapid UI development
- Excellent support for EXECORA brand colors (Crimson #B11226, Charcoal #1F2937, Insight Blue #2563EB)
- Built-in responsive design utilities
- Dark mode support for enterprise environments
- Small bundle sizes with JIT compilation
- Strong Angular integration via @angular/+tailwind (v19+)

**Documentation**: [../master/research.md](../master/research.md#3-uistyling-framework)

---

## Database Architecture

### Decision: SQL Server 2022 with EF Core 9.0

**Rationale**:
- Relational data model fits authentication domain (Users, Tenants, Roles)
- Strong transaction support for workflow state transitions
- Full-text search for inspection templates and issues
- Temporal tables for audit trail (native SQL Server feature)
- JSON column support for flexible workflow configuration storage
- Excellent Azure SQL Database compatibility
- **Row-Level Security (RLS)** for multi-tenant isolation

**Documentation**: [../master/research.md](../master/research.md#4-database-architecture)

---

### Decision: Shared Database, Row-Level Security (RLS)

**Rationale**:
- Cost-effective for SaaS model (single database instance)
- SQL Server RLS enforces isolation at database level
- TenantId column on all tenant-specific tables
- Automatic tenant filtering via EF Core global query filters
- Easier maintenance and backup strategy
- Scales to thousands of tenants

**Documentation**: [../master/research.md](../master/research.md#5-multi-tenancy-strategy)

---

## Security Implementation

### Password Requirements

| Requirement | Value |
|-------------|-------|
| Minimum Length | 12 characters |
| Required Characters | Uppercase, lowercase, number, special |
| History | Prevent reuse of last 5 passwords |
| Hashing Algorithm | BCrypt (work factor 12) |
| Reset Token Lifetime | 1 hour |
| Account Lockout | 5 failed attempts, 30 minute lockout |

### Token Security

| Setting | Value |
|---------|-------|
| Access Token Lifetime | 15 minutes |
| Refresh Token Lifetime | 7 days (standard), 30 days (remember me) |
| Token Storage | HttpOnly cookies (frontend) + Database (backend) |
| Refresh Strategy | Rotate on every use |
| Revocation | Immediate on logout/password change |

### Rate Limiting

| Endpoint | Limit |
|----------|-------|
| Login | 5 attempts per 15 minutes per IP |
| Registration | 3 attempts per hour per IP |
| Password Reset | 3 attempts per hour per email |
| Invitation | 10 per hour per tenant |

---

## Testing Strategy

### Backend Testing
- **Unit Tests**: xUnit + Moq (service layer)
- **Integration Tests**: WebApplicationFactory (API endpoints)
- **Coverage Target**: 80%+

### Frontend Testing
- **Unit Tests**: Jasmine/Karma (components, services)
- **E2E Tests**: Playwright (critical user flows)

**Documentation**: [../master/research.md](../master/research.md#11-testing-strategy)

---

## Deployment Architecture

### Decision: Azure App Service + Azure SQL Database

**Components**:
- **Frontend**: Azure Static Web Apps (Angular SPA)
- **Backend API**: Azure App Service (ASP.NET Core)
- **Database**: Azure SQL Database (Single database + elastic pool options)
- **Email**: SendGrid or Azure Communication Services
- **Cache**: Azure Cache for Redis (optional, for session/distributed locks)

**CI/CD**: GitHub Actions

**Documentation**: [../master/research.md](../master/research.md#12-deployment-architecture)

---

## Performance Considerations

### Target Metrics
- API Response: <200ms p95 for standard CRUD
- Login: <2s end-to-end
- Token Refresh: <500ms
- Concurrent Users: 1,000+ per tenant
- Database Connections: Connection pooling with max 100 connections

### Optimization Strategies
- EF Core query optimization (NoTracking, split queries)
- Response caching for static data (lookup tables)
- CDN for static assets (Angular bundles, images)
- Database indexes on TenantId, ProjectId, WorkflowState

**Documentation**: [../master/research.md](../master/research.md#13-performance-considerations)

---

## Security Measures

- HTTPS/TLS 1.3 enforced
- SQL injection prevention (EF Core parameterization)
- XSS prevention (Angular DOM sanitizer)
- CSRF tokens for state-changing operations
- API rate limiting (per tenant)
- Audit logging for all state transitions
- PII encryption at rest (SQL Server Always Encrypted)

**Documentation**: [../master/research.md](../master/research.md#14-security-measures)

---

## External Dependencies

| Dependency | Purpose | Azure Alternative |
|------------|---------|------------------|
| BCrypt.Net-Next | Password hashing | Built-in |
| SendGrid | Email delivery | Azure Communication Services |
| FluentValidation | Request validation | Data Annotations |
| System.IdentityModel.Tokens.Jwt | JWT tokens | Built-in |

---

**END OF RESEARCH**
