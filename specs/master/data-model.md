# EXECORA — Data Model
## Phase 1: Entity & Relationship Design

**Branch**: `master` | **Date**: 2025-02-03

---

## 1. Multi-Tenancy Core Entities

### 1.1 Tenant

Represents an organization/customer in the system.

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

Represents a user in the system. Users can belong to multiple tenants.

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

Junction table for user-tenant relationships with role information.

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

## 2. Organization & Project Management (Module 01)

### 2.1 Organization

Represents a company within a tenant (for large multi-org tenants).

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| TenantId | Guid | FK → Tenant, Required | |
| Name | string(100) | Required | Organization/unit name |
| Code | string(20) | | Short code |
| ParentOrganizationId | Guid | FK → Organization | Self-reference for hierarchy |
| Address | string(500) | | |
| LogoUrl | string | | |
| CreatedAt | datetime | | |
| UpdatedAt | datetime | | |
| IsDeleted | bool | | |

**Indexes**:
- `IX_Organizations_TenantId`

---

### 2.2 Project

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| TenantId | Guid | FK → Tenant, Required | |
| OrganizationId | Guid | FK → Organization | |
| Name | string(100) | Required | Project name |
| Code | string(20) | | Project code |
| Description | string(1000) | | |
| ProjectStatus | enum | Required | Draft, Active, OnHold, Completed, Archived |
| StartDate | date | | |
| EndDate | date | | |
| Location | string(500) | | Site location |
| Latitude | decimal(9,6) | | GPS latitude |
| Longitude | decimal(9,6) | | GPS longitude |
| BimModelId | string | | APS model URN |
| BimModelVersion | int | | Current model version |
| ProjectManagerId | Guid | FK → User | |
| CreatedBy | Guid | FK → User | |
| CreatedAt | datetime | | |
| UpdatedAt | datetime | | |
| IsDeleted | bool | | |

**Indexes**:
- `IX_Projects_TenantId`
- `IX_Projects_TenantId_OrganizationId`
- `IX_Projects_TenantId_Status`

---

### 2.3 ProjectUser (Project Membership)

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| ProjectId | Guid | FK → Project, PK | |
| UserId | Guid | FK → User, PK | |
| ProjectRole | enum | Required | ProjectAdmin, ProjectManager, QAQC, SiteEngineer, Viewer |
| AssignedAt | datetime | | |
| AssignedBy | Guid | FK → User | |

**Indexes**:
- `IX_ProjectUsers_ProjectId_UserId` (unique)
- `IX_ProjectUsers_UserId`

---

### 2.4 Trade

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| TenantId | Guid | FK → Tenant, Required | |
| Name | string(100) | Required | Trade name (e.g., "Electrical") |
| Code | string(20) | | Short code |
| Description | string(500) | | |
| Color | string(7) | | Hex color code |
| IsActive | bool | Default: true | |
| CreatedAt | datetime | | |

---

## 3. Activities (Module 03)

### 3.1 Activity

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| ProjectId | Guid | FK → Project, Required | |
| ParentActivityId | Guid | FK → Activity | Self-reference for WBS |
| TradeId | Guid | FK → Trade | |
| ActivityCode | string(50) | | WBS code |
| Name | string(200) | Required | Activity name |
| Description | string(1000) | | |
| PlannedStartDate | date | | |
| PlannedEndDate | date | | |
| ActualStartDate | date | | |
| ActualEndDate | date | | |
| ProgressPercentage | decimal(5,2) | 0-100 | Current progress |
| Status | enum | Required | NotStarted, InProgress, ReadyForInspection, Approved, Rework, Completed, OnHold, Locked |
| IsBlocked | bool | Default: false | Locked by workflow |
| BlockedById | Guid | FK → Issue/NCR | Reference to blocking entity |
| PlannedQuantity | decimal(12,2) | | |
| Unit | string(20) | | Unit of measure |
| Location | string(500) | | Location in project |
| BimElementIds | json | | Array of linked BIM element IDs |
| AssignedTo | Guid | FK → User | |
| CreatedAt | datetime | | |
| UpdatedAt | datetime | | |
| IsDeleted | bool | | |

**Indexes**:
- `IX_Activities_ProjectId`
- `IX_Activities_ProjectId_Status`
- `IX_Activities_ProjectId_ParentActivityId`
- `IX_Activities_AssignedTo`

---

### 3.2 ActivityProgress

Progress snapshots for trend analysis.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| ActivityId | Guid | FK → Activity, Required | |
| ProgressPercentage | decimal(5,2) | Required | |
| ReportedBy | Guid | FK → User, Required | |
| ReportedAt | datetime | Required | |
| Notes | string(1000) | | |

**Indexes**:
- `IX_ActivityProgress_ActivityId_ReportedAt`

---

## 4. Inspections (Module 04)

### 4.1 InspectionTemplate

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| TenantId | Guid | FK → Tenant, Required | |
| Name | string(100) | Required | |
| Description | string(500) | | |
| TradeId | Guid | FK → Trade | |
| Version | int | Required, Default: 1 | Template version |
| IsActive | bool | Default: true | |
| Checklists | json | Required | Array of checklist items |
| CreatedBy | Guid | FK → User | |
| CreatedAt | datetime | | |
| UpdatedAt | datetime | | |

**Checklist Item Structure**:
```json
{
  "id": "guid",
  "title": "Check concrete reinforcement",
  "type": "PassFail | Rating | Text | Photo",
  "required": true,
  "order": 1,
  "items": []  // nested items
}
```

---

### 4.2 Inspection

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| ProjectId | Guid | FK → Project, Required | |
| ActivityId | Guid | FK → Activity | |
| InspectionTemplateId | Guid | FK → InspectionTemplate, Required | |
| InspectionNumber | string(20) | Auto-generated | IFC-2024-0001 |
| Title | string(200) | Required | |
| Description | string(1000) | | |
| ScheduledDate | date | Required | |
| InspectionDate | datetime | | Actual inspection time |
| Status | enum | Required | Scheduled, InProgress, Passed, Failed, Cancelled |
| InspectorId | Guid | FK → User, Required | |
| ContractorId | Guid | FK → User | |
| Location | string(500) | | |
| BimElementIds | json | | Linked BIM elements |
| PassedAt | datetime | | |
| FailedAt | datetime | | |
| FailedReason | string(1000) | | |
| ChecklistResults | json | | Filled checklist |
| RequiresReinspection | bool | Default: false | |
| ReinspectionId | Guid | FK → Inspection | Self-reference |
| IsReinspectionOf | Guid | FK → Inspection | Self-reference |
| CreatedBy | Guid | FK → User | |
| CreatedAt | datetime | | |
| UpdatedAt | datetime | | |

**Indexes**:
- `IX_Inspections_ProjectId`
- `IX_Inspections_ProjectId_Status`
- `IX_Inspections_ActivityId`
- `IX_Inspections_InspectorId`
- `IX_Inspections_ScheduledDate`

---

### 4.3 InspectionAttachment

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| InspectionId | Guid | FK → Inspection, Required | |
| FileName | string(255) | Required | |
| FileUrl | string | Required | Azure Blob URL |
| FileType | string(50) | | image/jpeg, application/pdf |
| FileSizeBytes | long | | |
| UploadedBy | Guid | FK → User | |
| UploadedAt | datetime | | |
| Caption | string(500) | | |
| Metadata | json | | GPS, timestamp, etc. |

---

## 5. Issues & Risk Management (Module 05)

### 5.1 Issue

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| ProjectId | Guid | FK → Project, Required | |
| IssueNumber | string(20) | Auto-generated | ISS-2024-0001 |
| Title | string(200) | Required | |
| Description | string(2000) | Required | |
| Category | enum | Required | Safety, Quality, Schedule, Cost, Resource, Technical |
| Severity | enum | Required | Low, Medium, High, Critical |
| Priority | enum | Required | Low, Medium, High, Critical |
| Status | enum | Required | Open, InProgress, UnderReview, Resolved, Closed, OnHold |
| Source | enum | Required | Site, Office, Inspection, NCR, Manual |
| SourceId | string | | Reference to source entity |
| ActivityId | Guid | FK → Activity | |
| AssignedTo | Guid | FK → User, Required | |
| RaisedBy | Guid | FK → User, Required | |
| RaisedAt | datetime | Required | |
| DueDate | datetime | | SLA due date |
| ResolvedAt | datetime | | |
| Resolution | string(2000) | | |
| BimElementIds | json | | |
| Location | string(500) | | |
| IsEscalated | bool | Default: false | |
| EscalatedAt | datetime | | |
| EscalatedTo | Guid | FK → User | |
| CreatedAt | datetime | | |
| UpdatedAt | datetime | | |

**Indexes**:
- `IX_Issues_ProjectId`
- `IX_Issues_ProjectId_Status`
- `IX_Issues_ProjectId_Severity`
- `IX_Issues_AssignedTo`
- `IX_Issues_DueDate`

---

### 5.2 IssueComment

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| IssueId | Guid | FK → Issue, Required | |
| Comment | string(2000) | Required | |
| CreatedBy | Guid | FK → User, Required | |
| CreatedAt | datetime | Required | |
| Attachments | json | | Array of attachment URLs |

---

## 6. NCR & Quality Intelligence (Module 06)

### 6.1 NonConformanceReport (NCR)

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| ProjectId | Guid | FK → Project, Required | |
| NcrNumber | string(20) | Auto-generated | NCR-2024-0001 |
| Title | string(200) | Required | |
| Description | string(2000) | Required | |
| Category | enum | Required | Material, Workmanship, Design, Documentation |
| Severity | enum | Required | Minor, Major, Critical |
| Status | enum | Required | Open, UnderReview, CorrectiveAction, Verification, Closed, Rejected |
| SourceInspectionId | Guid | FK → Inspection | |
| SourceIssueId | Guid | FK → Issue | |
| ActivityId | Guid | FK → Activity | |
| RaisedBy | Guid | FK → User, Required | |
| AssignedTo | Guid | FK → User, Required | |
| RaisedAt | datetime | Required | |
| DueDate | datetime | | |
| RootCauseAnalysis | string(2000) | | RCA - required for closure |
| CorrectiveAction | string(2000) | | |
| CorrectiveActionApprovedBy | Guid | FK → User | |
| CorrectiveActionApprovedAt | datetime | | |
| PreventiveAction | string(2000) | | |
| VerifiedBy | Guid | FK → User | |
| VerifiedAt | datetime | | |
| ClosedBy | Guid | FK → User | |
| ClosedAt | datetime | | |
| BimElementIds | json | | |
| Location | string(500) | | |
| CreatedAt | datetime | | |
| UpdatedAt | datetime | | |

**Indexes**:
- `IX_NonConformanceReports_ProjectId`
- `IX_NonConformanceReports_ProjectId_Status`
- `IX_NonConformanceReports_ActivityId`
- `IX_NonConformanceReports_AssignedTo`

---

### 6.2 NcrAttachment

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| NcrId | Guid | FK → NonConformanceReport, Required | |
| FileName | string(255) | Required | |
| FileUrl | string | Required | |
| FileType | string(50) | | |
| FileSizeBytes | long | | |
| UploadedBy | Guid | FK → User | |
| UploadedAt | datetime | | |
| AttachmentType | enum | Required | Evidence, RCA, CorrectiveAction, Other |

---

## 7. Workflow Engine (Module 07)

### 7.1 WorkflowDefinition

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| TenantId | Guid | FK → Tenant, Required | |
| Name | string(100) | Required | |
| Description | string(500) | | |
| EntityType | enum | Required | Activity, Inspection, NCR, Issue |
| Version | int | Required, Default: 1 | |
| IsActive | bool | Default: true | |
| States | json | Required | State definitions |
| Transitions | json | Required | Transition rules |
| CreatedBy | Guid | FK → User | |
| CreatedAt | datetime | | |
| UpdatedAt | datetime | | |

**State Definition Example**:
```json
{
  "states": [
    {
      "id": "draft",
      "name": "Draft",
      "color": "#9CA3AF",
      "isInitial": true
    },
    {
      "id": "inProgress",
      "name": "In Progress",
      "color": "#3B82F6"
    }
  ],
  "transitions": [
    {
      "from": "draft",
      "to": "inProgress",
      "required": ["assignedTo", "plannedStartDate"],
      "permissions": ["ProjectManager"]
    }
  ]
}
```

---

### 7.2 WorkflowInstance

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| WorkflowDefinitionId | Guid | FK → WorkflowDefinition, Required | |
| EntityId | Guid | Required | Reference to entity instance |
| EntityType | enum | Required | Activity, Inspection, NCR, Issue |
| CurrentState | string(50) | Required | |
| PreviousState | string(50) | | |
| EnteredCurrentStateAt | datetime | Required | |
| StateHistory | json | | Array of state transitions |
| IsLocked | bool | Default: false | |
| LockedBy | Guid | FK → User | |
| LockedAt | datetime | | |
| LockedReason | string(500) | | |

**Indexes**:
- `IX_WorkflowInstances_EntityId_EntityType` (unique)

---

### 7.3 WorkflowTransition

Audit log for state transitions.

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| WorkflowInstanceId | Guid | FK → WorkflowInstance, Required | |
| FromState | string(50) | | |
| ToState | string(50) | Required | |
| TriggeredBy | Guid | FK → User, Required | |
| TriggeredAt | datetime | Required | |
| Comments | string(1000) | | |
| Metadata | json | | Additional transition data |

---

## 8. Daily Operations (Module 02)

### 8.1 DailyReport

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| ProjectId | Guid | FK → Project, Required | |
| ReportDate | date | Required | |
| WeatherCondition | enum | | Sunny, Cloudy, Rainy, Windy, Stormy |
| TemperatureMin | decimal(4,1) | | Celsius |
| TemperatureMax | decimal(4,1) | | Celsius |
| TotalManpower | int | | |
| WorkDescription | string(2000) | | |
| Constraints | string(1000) | | |
| TomorrowPlan | string(1000) | | |
| SubmittedBy | Guid | FK → User, Required | |
| SubmittedAt | datetime | Required | |
| ApprovedBy | Guid | FK → User | |
| ApprovedAt | datetime | | |

**Indexes**:
- `IX_DailyReports_ProjectId_ReportDate` (unique)

---

### 8.2 ManpowerEntry

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| DailyReportId | Guid | FK → DailyReport, Required | |
| TradeId | Guid | FK → Trade | |
| Description | string(100) | | |
| Headcount | int | Required | |
| WorkHours | decimal(4,1) | | |

---

## 9. Notifications (Module 10)

### 9.1 Notification

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| TenantId | Guid | FK → Tenant, Required | |
| UserId | Guid | FK → User, Required | |
| Type | enum | Required | InspectionAssigned, InspectionFailed, NcrRaised, IssueEscalated, ActivityLocked |
| Title | string(200) | Required | |
| Message | string(1000) | Required | |
| ActionUrl | string | | Deep link |
| IsRead | bool | Default: false | |
| ReadAt | datetime | | |
| SentAt | datetime | Required | |
| EntityType | string(50) | | |
| EntityId | Guid | | |
| Metadata | json | | |

**Indexes**:
- `IX_Notifications_UserId_IsRead`
- `IX_Notifications_TenantId_SentAt`

---

### 9.2 NotificationPreference

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| UserId | Guid | FK → User, Required | |
| TenantId | Guid | FK → Tenant, Required | |
| NotificationType | enum | Required | |
| InAppEnabled | bool | Default: true | |
| EmailEnabled | bool | | |
| SmsEnabled | bool | Default: false | |

---

## 10. Audit Trail

### 10.1 AuditLog

Immutable audit log (append-only table).

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| TenantId | Guid | FK → Tenant, Required | |
| EntityName | string(50) | Required | |
| EntityId | Guid | Required | |
| Action | enum | Required | Created, Updated, Deleted, StateChanged, Viewed |
| ActionType | string(50) | | Detailed action |
| OldValues | json | | |
| NewValues | json | | |
| ChangedBy | Guid | FK → User | |
| ChangedByType | string(50) | | User, System, API |
| ChangedAt | datetime | Required | |
| IpAddress | string(45) | | IPv4 or IPv6 |
| UserAgent | string(500) | | |
| ProjectId | Guid | FK → Project | | For filtering |

**Indexes**:
- `IX_AuditLogs_TenantId_ChangedAt`
- `IX_AuditLogs_EntityName_EntityId`
- `IX_AuditLogs_ProjectId_ChangedAt`

**Table Optimization**: Consider partitioning by month for large scale.

---

## 11. BIM Integration (Module 08)

### 11.1 BimModel

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| ProjectId | Guid | FK → Project, Required | |
| ModelName | string(100) | Required | |
| FileName | string(255) | Required | |
| FileSizeBytes | long | | |
| FileUrl | string | Required | Azure Blob URL |
| Urn | string | | APS URN after processing |
| Status | enum | Required | Uploading, Processing, Ready, Failed |
| Version | int | Required, Default: 1 | |
| UploadedBy | Guid | FK → User | |
| UploadedAt | datetime | | |
| ProcessedAt | datetime | | |

**Indexes**:
- `IX_BimModels_ProjectId_Version`

---

### 11.2 BimElementMapping

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | Guid | PK, Identity | |
| BimModelId | Guid | FK → BimModel, Required | |
| ElementId | string(100) | Required | APS element ID |
| EntityType | enum | Required | Activity, Inspection, NCR, Issue |
| EntityId | Guid | Required | |
| LinkedAt | datetime | Required | |
| LinkedBy | Guid | FK → User | |

**Indexes**:
- `IX_BimElementMappings_ElementId`
- `IX_BimElementMappings_EntityType_EntityId`

---

## 12. Entity Relationship Diagram Summary

```
Tenant (1) ----< (N) TenantUser >---- (N) User
Tenant (1) ----< (N) Organization
Tenant (1) ----< (N) Project
Tenant (1) ----< (N) WorkflowDefinition

Organization (1) ----< (N) Project

Project (1) ----< (N) ProjectUser >---- (N) User
Project (1) ----< (N) Activity
Project (1) ----< (N) Inspection
Project (1) ----< (N) Issue
Project (1) ----< (N) NonConformanceReport
Project (1) ----< (N) DailyReport
Project (1) ----< (N) BimModel

Activity (1) ----< (N) Inspection
Activity (1) ----< (N) NonConformanceReport
Activity (1) ----< (N) Issue
Activity (1) ----< (N) ActivityProgress

Inspection (1) ----< (N) InspectionAttachment
Inspection (1) ----< (N) NonConformanceReport (via failed inspection)

NonConformanceReport (1) ----< (N) NcrAttachment
Issue (1) ----< (N) IssueComment

WorkflowDefinition (1) ----< (N) WorkflowInstance
WorkflowInstance (1) ----< (N) WorkflowTransition
```

---

**END OF DATA MODEL**
