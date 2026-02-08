# Data Model: Multi-Tenant Authentication & User Management

**Feature**: 001-multi-tenant-auth
**Branch**: `001-multi-tenant-auth` | **Date**: 2025-02-03

---

## Overview

This document defines the entities and relationships for Multi-Tenant Authentication & User Management. The core entities (Tenant, User, TenantUser) are already defined in the master data model and implemented in the codebase. This feature adds Invitation, RefreshToken, and AuditLog entities.

**Master Data Model**: [../master/data-model.md](../master/data-model.md)

---

## Existing Entities (Already Implemented)

### 1.1 Tenant

**File**: `backend/src/Execora.Core/Entities/Tenant.cs`

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | Unique tenant identifier |
| Name | string(100) | Required | Organization name |
| Slug | string(50) | Unique, Required | URL-friendly identifier |
| SubscriptionPlan | enum | Required | Core, Professional, Enterprise |
| SubscriptionStatus | enum | Required | Active, Suspended, Trial, PastDue |
| SubscriptionExpiry | datetime | | Plan expiration date |
| MaxProjects | int | | Project limit based on plan |
| MaxUsers | int | | User limit based on plan |
| CreatedAt | datetime | | Tenant creation timestamp |
| UpdatedAt | datetime | | Last modification timestamp |
| IsDeleted | bool | | Soft delete flag |

**Indexes**:
- `IX_Tenants_Slug` (unique)
- `IX_Tenants_SubscriptionStatus`

---

### 1.2 User

**File**: `backend/src/Execora.Core/Entities/User.cs`

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | Unique user identifier |
| Email | string(255) | Unique, Required | User email address |
| EmailConfirmed | bool | Default: false | Email verification status |
| PhoneNumber | string(20) | | Mobile number |
| PhoneNumberConfirmed | bool | Default: false | Phone verification status |
| FirstName | string(50) | Required | |
| LastName | string(50) | Required | |
| PasswordHash | string | Required | Hashed password |
| IsActive | bool | Default: true | Account active flag |
| LastLoginAt | datetime | | Last successful login |
| CreatedAt | datetime | | Account creation timestamp |
| UpdatedAt | datetime | | Last modification timestamp |

**Indexes**:
- `IX_Users_Email` (unique)

---

### 1.3 TenantUser

**File**: `backend/src/Execora.Core/Entities/TenantUser.cs`

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| TenantId | Guid | FK → Tenant, PK | |
| UserId | Guid | FK → User, PK | |
| Role | enum | Required | SystemAdmin, TenantAdmin, ProjectAdmin, ProjectManager, QAQC, SiteEngineer |
| Permissions | json | | Additional permissions (rare) |
| InvitedBy | Guid | FK → User | Who invited this user |
| InvitedAt | datetime | | Invitation timestamp |
| JoinedAt | datetime | | Acceptance timestamp |
| IsActive | bool | Default: true | |

**Indexes**:
- `IX_TenantUsers_TenantId_UserId` (unique)
- `IX_TenantUsers_TenantId_Role`

---

## New Entities (To Be Implemented)

### 2.1 Invitation

Represents a pending invitation for a user to join a tenant.

**File**: `backend/src/Execora.Core/Entities/Invitation.cs` (NEW)

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | Unique invitation identifier |
| TenantId | Guid | FK → Tenant, Required | Tenant user is invited to |
| Email | string(255) | Required | Email address of invited user |
| Role | enum | Required | Role to assign upon acceptance |
| Token | string | Unique, Required | Secure invitation token |
| ExpiresAt | datetime | Required | Token expiration (7 days) |
| Status | enum | Required | Pending, Accepted, Expired, Cancelled |
| InvitedBy | Guid | FK → User | User who sent invitation |
| InvitedAt | datetime | Required | When invitation was sent |
| AcceptedAt | datetime | | When invitation was accepted |
| CreatedAt | datetime | | Record creation timestamp |
| UpdatedAt | datetime | | Last modification timestamp |

**Indexes**:
- `IX_Invitations_TenantId_Status`
- `IX_Invitations_Token` (unique)
- `IX_Invitations_Email_TenantId`

**Enums** (NEW):
```csharp
// backend/src/Execora.Core/Enums/InvitationStatus.cs
public enum InvitationStatus
{
    Pending = 1,
    Accepted = 2,
    Expired = 3,
    Cancelled = 4
}
```

---

### 2.2 RefreshToken

Represents a refresh token used for maintaining user sessions.

**File**: `backend/src/Execora.Core/Entities/RefreshToken.cs` (NEW)

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | Unique token identifier |
| UserId | Guid | FK → User, Required | User who owns this token |
| TokenHash | string | Unique, Required | SHA-256 hash of the token |
| ExpiresAt | datetime | Required | Token expiration datetime |
| DeviceIdentifier | string(50) | | Optional device/browser fingerprint |
| IsRevoked | bool | Default: false | Revocation status |
| RevokedAt | datetime | | When token was revoked |
| RememberMe | bool | Default: false | Extended lifetime flag |
| CreatedAt | datetime | Required | When token was created |
| UpdatedAt | datetime | | Last modification timestamp |

**Indexes**:
- `IX_RefreshTokens_UserId`
- `IX_RefreshTokens_TokenHash` (unique)
- `IX_RefreshTokens_ExpiresAt`

---

### 2.3 AuditLog

Immutable record of security-relevant events.

**File**: `backend/src/Execora.Core/Entities/AuditLog.cs` (NEW)

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | Unique log entry identifier |
| TenantId | Guid | FK → Tenant | Tenant context (nullable for system events) |
| EntityName | string(50) | Required | Name of entity (User, Tenant, Invitation, etc.) |
| EntityId | Guid | | ID of affected entity |
| Action | enum | Required | Type of action performed |
| ActionType | string(50) | | Detailed action type |
| OldValues | json | | Previous values (JSON) |
| NewValues | json | | New values (JSON) |
| ChangedBy | Guid | FK → User | User who made the change |
| ChangedByType | string(50) | | User, System, API |
| ChangedAt | datetime | Required | When the change occurred |
| IpAddress | string(45) | | IP address of requester |
| UserAgent | string(500) | | Client user agent string |
| ProjectId | Guid | FK → Project | Optional project context |

**Indexes**:
- `IX_AuditLogs_TenantId_ChangedAt`
- `IX_AuditLogs_EntityName_EntityId`
- `IX_AuditLogs_ChangedBy`
- `IX_AuditLogs_ProjectId_ChangedAt`

**Enums** (NEW):
```csharp
// backend/src/Execora.Core/Enums/AuditAction.cs
public enum AuditAction
{
    Created = 1,
    Updated = 2,
    Deleted = 3,
    StateChanged = 4,
    Viewed = 5,
    LoggedIn = 6,
    LoggedOut = 7,
    PasswordChanged = 8,
    PasswordReset = 9,
    EmailVerified = 10,
    InvitationSent = 11,
    InvitationAccepted = 12,
    RoleChanged = 13,
    TenantSwitched = 14
}
```

---

## Entity Relationships

```
┌─────────────────────────────────────────────────────────────────────┐
│                    AUTH & USER MANAGEMENT DATA MODEL                 │
├─────────────────────────────────────────────────────────────────────┤
│                                                                       │
│  Tenant (1) ────< (N) TenantUser >──── (N) User                     │
│     │                    │                │                         │
│     │                    │                │                         │
│     V                    V                V                         │
│  (N)               (N) Invitation    (N) RefreshToken              │
│ Invitation                                │                         │
│     │                                   │                         │
│     V                                   V                         │
│  Accepted by                          Stores                     │
│     │                                   │                         │
│     V                                   V                         │
│  Creates TenantUser                  Session                     │
│                                                                       │
│  Tenant (1) ────< (N) AuditLog                                      │
│  User (1) ──────< (N) AuditLog (as ChangedBy)                       │
│                                                                       │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Database Migrations

### Migration: AddAuthenticationEntities

```sql
-- Invitation Table
CREATE TABLE [Invitations] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [Role] INT NOT NULL,
    [Token] NVARCHAR(256) NOT NULL,
    [ExpiresAt] DATETIME2 NOT NULL,
    [Status] INT NOT NULL,
    [InvitedBy] UNIQUEIDENTIFIER NOT NULL,
    [InvitedAt] DATETIME2 NOT NULL,
    [AcceptedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [FK_Invitations_Tenants] FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]),
    CONSTRAINT [FK_Invitations_Users_InvitedBy] FOREIGN KEY ([InvitedBy]) REFERENCES [Users]([Id])
);

CREATE INDEX [IX_Invitations_TenantId_Status] ON [Invitations] ([TenantId], [Status]);
CREATE UNIQUE INDEX [IX_Invitations_Token] ON [Invitations] ([Token]);

-- RefreshToken Table
CREATE TABLE [RefreshTokens] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [TokenHash] NVARCHAR(256) NOT NULL,
    [ExpiresAt] DATETIME2 NOT NULL,
    [DeviceIdentifier] NVARCHAR(50) NULL,
    [IsRevoked] BIT NOT NULL DEFAULT 0,
    [RevokedAt] DATETIME2 NULL,
    [RememberMe] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [FK_RefreshTokens_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
);

CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
CREATE UNIQUE INDEX [IX_RefreshTokens_TokenHash] ON [RefreshTokens] ([TokenHash]);

-- AuditLog Table
CREATE TABLE [AuditLogs] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [TenantId] UNIQUEIDENTIFIER NULL,
    [EntityName] NVARCHAR(50) NOT NULL,
    [EntityId] UNIQUEIDENTIFIER NULL,
    [Action] INT NOT NULL,
    [ActionType] NVARCHAR(50) NULL,
    [OldValues] NVARCHAR(MAX) NULL,
    [NewValues] NVARCHAR(MAX) NULL,
    [ChangedBy] UNIQUEIDENTIFIER NOT NULL,
    [ChangedByType] NVARCHAR(50) NULL,
    [ChangedAt] DATETIME2 NOT NULL,
    [IpAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [ProjectId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [FK_AuditLogs_Tenants] FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]),
    CONSTRAINT [FK_AuditLogs_Users_ChangedBy] FOREIGN KEY ([ChangedBy]) REFERENCES [Users]([Id])
);

CREATE INDEX [IX_AuditLogs_TenantId_ChangedAt] ON [AuditLogs] ([TenantId], [ChangedAt]);
CREATE INDEX [IX_AuditLogs_EntityName_EntityId] ON [AuditLogs] ([EntityName], [EntityId]);
CREATE INDEX [IX_AuditLogs_ChangedBy] ON [AuditLogs] ([ChangedBy]);
```

---

## Row-Level Security (RLS) Configuration

### RLS Predicate for Tenants

```sql
-- Create RLS predicate function
CREATE FUNCTION [dbo].[TenantPredicate](@TenantId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN (
    SELECT 1 AS Result
    WHERE @TenantId = CAST(SESSION_CONTEXT(N'TenantId') AS UNIQUEIDENTIFIER)
);

-- Bind RLS to tables
ALTER SECURITY POLICY [dbo].[TenantIsolationPolicy]
ADD FILTER PREDATE [dbo].[TenantPredicate](@TenantId) ON [Invitations];

ALTER SECURITY POLICY [dbo].[TenantIsolationPolicy]
ADD FILTER PREDATE [dbo].[TenantPredicate](@TenantId) ON [AuditLogs];
```

---

**END OF DATA MODEL**
