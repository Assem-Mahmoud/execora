# Tasks: EXECORA Construction Execution Platform

**Branch**: `master` | **Date**: 2025-02-03
**Input**: Design documents from `/specs/master/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Test tasks are included for quality assurance. Constitution indicates TDD will be enforced during implementation.

**Organization**: Tasks are grouped by user story (module) to enable independent implementation and testing of each feature area.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Backend**: `backend/src/` for .NET projects
- **Frontend**: `frontend/src/` for Angular projects
- **Tests**: `backend/tests/` and `frontend/src/` for unit tests

---

## Phase 1: Setup (Shared Infrastructure) ✅ COMPLETE

**Purpose**: Project initialization and basic structure for both backend and frontend

- [X] T001 Create backend solution structure per implementation plan at backend/Execora.sln
- [X] T002 Create .NET 9.0 projects in backend/src/: Execora.Api, Execora.Core, Execora.Application, Execora.Infrastructure, Execora.Auth, Execora.Workflow
- [X] T003 [P] Create Angular 19 project in frontend/ with standalone components configuration
- [X] T004 [P] Initialize backend dependencies: EF Core 9.0, ASP.NET Core Identity, JWT, SignalR, FluentValidation in backend/src/
- [X] T005 [P] Initialize frontend dependencies: Angular 19, RxJS 7, Tailwind CSS 4.x in frontend/
- [X] T006 [P] Configure .editorconfig and backend linting (StyleCop, Analyzers) in backend/
- [X] T007 [P] Configure frontend linting (ESLint, Prettier) and formatting in frontend/
- [X] T008 [P] Create xUnit test projects in backend/tests/: Execora.Tests.Unit, Execora.Tests.Integration
- [X] T009 [P] Configure Jasmine/Karma for unit tests and Playwright for E2E in frontend/
- [X] T010 Create .gitignore per specification for .NET and Angular projects
- [X] T011 [P] Create backend appsettings.json with configuration sections for JWT, ConnectionStrings, AzureStorage, Autodesk, Email, Logging
- [X] T012 [P] Create frontend environment files in frontend/src/environments/: environment.ts, environment.development.ts, environment.production.ts

**Checkpoint**: ✅ Solution structure ready with all projects and configurations

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Backend Foundation

- [ ] T013 Create ExecoraDbContext with DbSet properties for all entities in backend/src/Execora.Infrastructure/Data/ExecoraDbContext.cs
- [ ] T014 [P] Implement core entities in backend/src/Execora.Core/Entities/: Tenant.cs, User.cs, TenantUser.cs
- [ ] T015 [P] Configure Row-Level Security (RLS) for multi-tenant isolation in backend/src/Execora.Infrastructure/Data/ExecoraDbContext.cs
- [ ] T016 Create repository interfaces in backend/src/Execora.Core/Interfaces/: IRepository.cs, ITenantRepository.cs, IUserRepository.cs
- [ ] T017 Implement generic repository pattern in backend/src/Execora.Infrastructure/Repositories/: Repository.cs, TenantRepository.cs, UserRepository.cs
- [ ] T018 Setup EF Core migrations framework with initial migration in backend/src/Execora.Infrastructure/Migrations/
- [ ] T019 [P] Implement JWT token service in backend/src/Execora.Auth/Services/: JwtTokenService.cs, ITokenService.cs
- [ ] T020 [P] Implement refresh token rotation in backend/src/Execora.Auth/Services/: RefreshTokenService.cs
- [ ] T021 Configure ASP.NET Core Identity with custom User and TenantUser in backend/src/Execora.Infrastructure/Identity/
- [ ] T022 Create tenant resolution middleware in backend/src/Execora.Api/Middleware/: TenantResolutionMiddleware.cs
- [ ] T023 [P] Implement authentication schemes and policies in backend/src/Execora.Auth/Services/: AuthorizationService.cs
- [ ] T024 [P] Create global exception handling filter in backend/src/Execora.Api/Filters/: GlobalExceptionFilter.cs
- [ ] T025 [P] Create audit log service for immutable audit trail in backend/src/Execora.Application/Services/: AuditLogService.cs
- [ ] T026 Configure CORS, Swagger/Kestrel in backend/src/Execora.Api/Program.cs
- [ ] T027 Configure dependency injection container in backend/src/Execora.Api/Program.cs

### Frontend Foundation

- [ ] T028 [P] Create core services in frontend/src/app/core/services/: ApiService.ts, AuthService.ts, TenantService.cs
- [ ] T029 [P] Create HTTP interceptors in frontend/src/app/core/interceptors/: AuthInterceptor.cs, TenantInterceptor.cs, ErrorInterceptor.cs
- [ ] T030 [P] Create route guards in frontend/src/app/core/guards/: AuthGuard.cs, TenantGuard.cs, RoleGuard.cs
- [ ] T031 Create shared models in frontend/src/app/core/models/: Tenant.ts, User.ts, Project.ts, Activity.ts
- [ ] T032 [P] Create base UI components in frontend/src/app/shared/components/: ButtonComponent.ts, InputComponent.ts, CardComponent.ts
- [ ] T033 [P] Create layouts in frontend/src/app/shared/layouts/: MainLayoutComponent.ts, AuthLayoutComponent.ts

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Multi-Tenant Authentication & User Management (Priority: P1) 🎯 MVP

**Goal**: Users can register, login, and manage tenant memberships. Multi-tenant isolation enforced.

**Independent Test**: Create a tenant, invite users, login with different roles, verify tenant isolation

### Backend Implementation

- [ ] T034 [P] [US1] Create RegisterRequestDto, LoginRequestDto, TokenResponseDto in backend/src/Execora.Application/DTOs/Auth/
- [ ] T035 [P] [US1] Create invitation DTOs in backend/src/Execora.Application/DTOs/Auth/: InviteUserRequestDto.cs, AcceptInvitationDto.cs
- [ ] T036 [US1] Implement authentication service in backend/src/Execora.Application/Services/: IAuthenticationService.cs, AuthenticationService.cs
- [ ] T037 [US1] Implement user invitation service in backend/src/Execora.Application/Services/: IInvitationService.cs, InvitationService.cs
- [ ] T038 [US1] Create AuthController in backend/src/Execora.Api/Controllers/: AuthController.cs (endpoints: /api/auth/register, /api/auth/login, /api/auth/refresh, /api/auth/logout)
- [ ] T039 [US1] Create UsersController in backend/src/Execora.Api/Controllers/App/: UsersController.cs (endpoints: /api/app/users/invite, /api/app/users, /api/app/users/{id})
- [ ] T040 [US1] Add FluentValidation validators for auth DTOs in backend/src/Execora.Application/Validators/: RegisterRequestValidator.cs, LoginRequestValidator.cs

### Frontend Implementation

- [ ] T041 [P] [US1] Create login component in frontend/src/app/features/auth/: LoginComponent.ts, login.component.html
- [ ] T042 [P] [US1] Create register component in frontend/src/app/features/auth/: RegisterComponent.ts, register.component.html
- [ ] T043 [P] [US1] Create tenant selection component in frontend/src/app/features/auth/: TenantSelectorComponent.ts, tenant-selector.component.html
- [ ] T044 [US1] Implement auth state management with RxJS BehaviorSubject in frontend/src/app/core/services/: AuthStateService.cs
- [ ] T045 [US1] Create user invitation component in frontend/src/app/features/settings/: UserInviteComponent.ts, user-invite.component.html
- [ ] T046 [US1] Configure Angular routing for auth flow in frontend/src/app/app-routing.module.ts

**Checkpoint**: At this point, users can authenticate and tenant membership works

---

## Phase 4: User Story 2 - Project & Organization Management (Priority: P1) 🎯 MVP

**Goal**: Tenant admins can create organizations, projects, and manage project teams

**Independent Test**: Create organization, create project, assign users to project, verify project access control

### Backend Implementation

- [ ] T047 [P] [US2] Create Organization entity in backend/src/Execora.Core/Entities/: Organization.cs
- [ ] T048 [P] [US2] Create Project entity in backend/src/Execora.Core/Entities/: Project.cs
- [ ] T049 [P] [US2] Create ProjectUser entity in backend/src/Execora.Core/Entities/: ProjectUser.cs
- [ ] T050 [P] [US2] Create Trade entity in backend/src/Execora.Core/Entities/: Trade.cs
- [ ] T051 [US2] Add DbSet for Organization, Project, ProjectUser, Trade to ExecoraDbContext in backend/src/Execora.Infrastructure/Data/
- [ ] T052 [US2] Create repository interfaces in backend/src/Execora.Core/Interfaces/: IOrganizationRepository.cs, IProjectRepository.cs, ITradeRepository.cs
- [ ] T053 [US2] Implement repositories in backend/src/Execora.Infrastructure/Repositories/: OrganizationRepository.cs, ProjectRepository.cs, TradeRepository.cs
- [ ] T054 [P] [US2] Create Project DTOs in backend/src/Execora.Application/DTOs/Projects/: ProjectCreateRequestDto.cs, ProjectResponseDto.cs, ProjectUpdateRequestDto.cs
- [ ] T055 [P] [US2] Create Organization DTOs in backend/src/Execora.Application/DTOs/Organizations/: OrganizationCreateRequestDto.cs, OrganizationResponseDto.cs
- [ ] T056 [US2] Create IProjectService in backend/src/Execora.Application/Interfaces/: IProjectService.cs, ProjectService.cs
- [ ] T057 [US2] Create IOrganizationService in backend/src/Execora.Application/Interfaces/: IOrganizationService.cs, OrganizationService.cs
- [ ] T058 [US2] Create ProjectsController in backend/src/Execora.Api/Controllers/App/: ProjectsController.cs (endpoints: GET/POST /api/app/projects, GET/PUT /api/app/projects/{id})
- [ ] T059 [US2] Create OrganizationsController in backend/src/Execora.Api/Controllers/App/: OrganizationsController.cs (endpoints: GET/POST /api/app/organizations, GET/PUT /api/app/organizations/{id})
- [ ] T060 [US2] Add project-based authorization policy in backend/src/Execora.Auth/Policies/: ProjectAccessPolicy.cs
- [ ] T061 [US2] Create migration and update database for project entities in backend/src/Execora.Infrastructure/Migrations/

### Frontend Implementation

- [ ] T062 [P] [US2] Create ProjectService in frontend/src/app/core/services/: ProjectService.ts
- [ ] T063 [P] [US2] Create OrganizationService in frontend/src/app/core/services/: OrganizationService.ts
- [ ] T064 [P] [US2] Create project list component in frontend/src/app/features/projects/: ProjectListComponent.ts, project-list.component.html
- [ ] T065 [P] [US2] Create project form component in frontend/src/app/features/projects/: ProjectFormComponent.ts, project-form.component.html
- [ ] T066 [P] [US2] Create project detail component in frontend/src/app/features/projects/: ProjectDetailComponent.ts, project-detail.component.html
- [ ] T067 [US2] Create organization management component in frontend/src/app/features/settings/: OrganizationComponent.ts, organization.component.html
- [ ] T068 [US2] Create project team management component in frontend/src/app/features/projects/: ProjectTeamComponent.ts, project-team.component.html
- [ ] T069 [US2] Configure routing for projects module in frontend/src/app/app-routing.module.ts

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - Activities & Progress Tracking (Priority: P1) 🎯 MVP

**Goal**: Project managers can create activities, assign them, and track progress

**Independent Test**: Create activities, update progress, view activity tree, verify blocking logic

### Backend Implementation

- [ ] T070 [P] [US3] Create Activity entity in backend/src/Execora.Core/Entities/: Activity.cs
- [ ] T071 [P] [US3] Create ActivityProgress entity in backend/src/Execora.Core/Entities/: ActivityProgress.cs
- [ ] T072 [US3] Add DbSet for Activity, ActivityProgress to ExecoraDbContext in backend/src/Execora.Infrastructure/Data/
- [ ] T073 [US3] Create IActivityRepository interface in backend/src/Execora.Core/Interfaces/: IActivityRepository.cs
- [ ] T074 [US3] Implement ActivityRepository in backend/src/Execora.Infrastructure/Repositories/: ActivityRepository.cs
- [ ] T075 [P] [US3] Create Activity DTOs in backend/src/Execora.Application/DTOs/Activities/: ActivityCreateRequestDto.cs, ActivityResponseDto.cs, ActivityTreeDto.cs
- [ ] T076 [US3] Create IActivityService in backend/src/Execora.Application/Interfaces/: IActivityService.cs, ActivityService.cs
- [ ] T077 [US3] Implement activity hierarchy (parent-child) logic in backend/src/Execora.Application/Services/: ActivityHierarchyService.cs
- [ ] T078 [US3] Implement progress tracking with history in backend/src/Execora.Application/Services/: ActivityProgressService.cs
- [ ] T079 [US3] Create ActivitiesController in backend/src/Execora.Api/Controllers/App/: ActivitiesController.cs (endpoints per contract: GET/POST /api/app/projects/{id}/activities, GET/PUT/DELETE /api/app/activities/{id}, POST /api/app/activities/{id}/progress)
- [ ] T080 [US3] Add activity status enum validation in backend/src/Execora.Application/Validators/: ActivityStatusValidator.cs

### Frontend Implementation

- [ ] T081 [P] [US3] Create ActivityService in frontend/src/app/core/services/: ActivityService.ts
- [ ] T082 [P] [US3] Create activity list/tree component in frontend/src/app/features/activities/: ActivityTreeComponent.ts, activity-tree.component.html
- [ ] T083 [P] [US3] Create activity form component in frontend/src/app/features/activities/: ActivityFormComponent.ts, activity-form.component.html
- [ ] T084 [US3] Create activity detail component in frontend/src/app/features/activities/: ActivityDetailComponent.ts, activity-detail.component.html
- [ ] T085 [US3] Create progress update component in frontend/src/app/features/activities/: ProgressUpdateComponent.ts, progress-update.component.html
- [ ] T086 [US3] Implement activity tree visualization with recursive components in frontend/src/app/features/activities/: ActivityTreeNodeComponent.ts
- [ ] T087 [US3] Configure routing for activities module in frontend/src/app/app-routing.module.ts

**Checkpoint**: All user stories (1-3) should now be independently functional

---

## Phase 6: User Story 4 - Daily Operations (Priority: P2)

**Goal**: Site engineers can submit daily reports with manpower and weather data

**Independent Test**: Create daily report, add manpower entries, view report history

### Backend Implementation

- [ ] T088 [P] [US4] Create DailyReport entity in backend/src/Execora.Core/Entities/: DailyReport.cs
- [ ] T089 [P] [US4] Create ManpowerEntry entity in backend/src/Execora.Core/Entities/: ManpowerEntry.cs
- [ ] T090 [US4] Add DbSet for DailyReport, ManpowerEntry to ExecoraDbContext in backend/src/Execora.Infrastructure/Data/
- [ ] T091 [US4] Create IDailyReportRepository interface in backend/src/Execora.Core/Interfaces/: IDailyReportRepository.cs
- [ ] T092 [US4] Implement DailyReportRepository in backend/src/Execora.Infrastructure/Repositories/: DailyReportRepository.cs
- [ ] T093 [P] [US4] Create DailyReport DTOs in backend/src/Execora.Application/DTOs/DailyOps/: DailyReportCreateRequestDto.cs, DailyReportResponseDto.cs
- [ ] T094 [US4] Create IDailyReportService in backend/src/Execora.Application/Interfaces/: IDailyReportService.cs, DailyReportService.cs
- [ ] T095 [US4] Create DailyOperationsController in backend/src/Execora.Api/Controllers/App/: DailyOperationsController.cs (endpoints: GET/POST /api/app/projects/{id}/daily-reports, GET /api/app/projects/{id}/daily-reports/today)

### Frontend Implementation

- [ ] T096 [P] [US4] Create DailyReportService in frontend/src/app/core/services/: DailyReportService.ts
- [ ] T097 [P] [US4] Create daily report form component in frontend/src/app/features/daily-ops/: DailyReportFormComponent.ts, daily-report-form.component.html
- [ ] T098 [P] [US4] Create daily report list component in frontend/src/app/features/daily-ops/: DailyReportListComponent.ts, daily-report-list.component.html
- [ ] T099 [US4] Create manpower entry component in frontend/src/app/features/daily-ops/: ManpowerEntryComponent.ts, manpower-entry.component.html
- [ ] T100 [US4] Configure routing for daily operations module in frontend/src/app/app-routing.module.ts

**Checkpoint**: Daily operations feature complete

---

## Phase 7: User Story 5 - Issues & Risk Management (Priority: P2)

**Goal**: Users can create, track, and escalate issues with SLA tracking

**Independent Test**: Create issue, assign to user, escalate based on severity, verify SLA tracking

### Backend Implementation

- [ ] T101 [P] [US5] Create Issue entity in backend/src/Execora.Core/Entities/: Issue.cs
- [ ] T102 [P] [US5] Create IssueComment entity in backend/src/Execora.Core/Entities/: IssueComment.cs
- [ ] T103 [US5] Add DbSet for Issue, IssueComment to ExecoraDbContext in backend/src/Execora.Infrastructure/Data/
- [ ] T104 [US5] Create IIssueRepository interface in backend/src/Execora.Core/Interfaces/: IIssueRepository.cs
- [ ] T105 [US5] Implement IssueRepository in backend/src/Execora.Infrastructure/Repositories/: IssueRepository.cs
- [ ] T106 [P] [US5] Create Issue DTOs in backend/src/Execora.Application/DTOs/Issues/: IssueCreateRequestDto.cs, IssueResponseDto.cs, IssueCommentDto.cs
- [ ] T107 [US5] Create IIssueService in backend/src/Execora.Application/Interfaces/: IIssueService.cs, IssueService.cs
- [ ] T108 [US5] Implement SLA escalation service in backend/src/Execora.Application/Services/: IssueEscalationService.cs
- [ ] T109 [US5] Create IssuesController in backend/src/Execora.Api/Controllers/App/: IssuesController.cs (endpoints: GET/POST /api/app/projects/{id}/issues, GET/PUT/DELETE /api/app/issues/{id})

### Frontend Implementation

- [ ] T110 [P] [US5] Create IssueService in frontend/src/app/core/services/: IssueService.ts
- [ ] T111 [P] [US5] Create issue list component with filters in frontend/src/app/features/issues/: IssueListComponent.ts, issue-list.component.html
- [ ] T112 [P] [US5] Create issue form component in frontend/src/app/features/issues/: IssueFormComponent.ts, issue-form.component.html
- [ ] T113 [US5] Create issue detail component with comments in frontend/src/app/features/issues/: IssueDetailComponent.ts, issue-detail.component.html
- [ ] T114 [US5] Create issue dashboard component in frontend/src/app/features/issues/: IssueDashboardComponent.ts, issue-dashboard.component.html
- [ ] T115 [US5] Configure routing for issues module in frontend/src/app/app-routing.module.ts

**Checkpoint**: Issues and risk management feature complete

---

## Phase 8: User Story 6 - Workflow Engine (Priority: P3) - Professional Plan

**Goal**: Finite State Machine enforces state transitions and creates audit trail

**Independent Test**: Define workflow, trigger state transition, verify enforcement, view audit trail

### Backend Implementation

- [ ] T116 [P] [US6] Create WorkflowDefinition entity in backend/src/Execora.Core/Entities/: WorkflowDefinition.cs
- [ ] T117 [P] [US6] Create WorkflowInstance entity in backend/src/Execora.Core/Entities/: WorkflowInstance.cs
- [ ] T118 [P] [US6] Create WorkflowTransition entity in backend/src/Execora.Core/Entities/: WorkflowTransition.cs
- [ ] T119 [US6] Add DbSet for workflow entities to ExecoraDbContext in backend/src/Execora.Infrastructure/Data/
- [ ] T120 [US6] Create IWorkflowRepository interface in backend/src/Execora.Core/Interfaces/: IWorkflowRepository.cs
- [ ] T121 [US6] Implement WorkflowRepository in backend/src/Execora.Infrastructure/Repositories/: WorkflowRepository.cs
- [ ] T122 [P] [US6] Create Workflow DTOs in backend/src/Execora.Application/DTOs/Workflows/: WorkflowDefinitionDto.cs, StateTransitionRequestDto.cs
- [ ] T123 [US6] Create IWorkflowEngine service interface in backend/src/Execora.Workflow/Engine/: IWorkflowEngine.cs
- [ ] T124 [US6] Implement Finite State Machine engine in backend/src/Execora.Workflow/Engine/: WorkflowEngine.cs, StateTransitionValidator.cs
- [ ] T125 [US6] Create default workflow definitions for Activity, Inspection, NCR in backend/src/Execora.Workflow/Definitions/: DefaultWorkflows.cs
- [ ] T126 [US6] Create IWorkflowService in backend/src/Execora.Application/Interfaces/: IWorkflowService.cs, WorkflowService.cs
- [ ] T127 [US6] Create WorkflowsController in backend/src/Execora.Api/Controllers/App/: WorkflowsController.cs (endpoints: GET /api/app/workflows, POST /api/app/workflows/{id}/transition)

### Frontend Implementation

- [ ] T128 [P] [US6] Create WorkflowService in frontend/src/app/core/services/: WorkflowService.ts
- [ ] T129 [P] [US6] Create status indicator component in frontend/src/app/shared/components/: StatusIndicatorComponent.ts, status-indicator.component.html
- [ ] T130 [P] [US6] Create workflow visualization component in frontend/src/app/shared/components/: WorkflowViewerComponent.ts, workflow-viewer.component.html
- [ ] T131 [US6] Create state transition button component in frontend/src/app/shared/components/: StateTransitionButtonComponent.ts, state-transition-button.component.html

**Checkpoint**: Workflow engine operational with FSM enforcement

---

## Phase 9: User Story 7 - Inspections & Quality Management (Priority: P3) - Professional Plan

**Goal**: Inspectors can create inspections, submit checklists, and trigger NCRs on failure

**Independent Test**: Create inspection template, schedule inspection, submit results, verify NCR creation

### Backend Implementation

- [ ] T132 [P] [US7] Create InspectionTemplate entity in backend/src/Execora.Core/Entities/: InspectionTemplate.cs
- [ ] T133 [P] [US7] Create Inspection entity in backend/src/Execora.Core/Entities/: Inspection.cs
- [ ] T134 [P] [US7] Create InspectionAttachment entity in backend/src/Execora.Core/Entities/: InspectionAttachment.cs
- [ ] T135 [US7] Add DbSet for inspection entities to ExecoraDbContext in backend/src/Execora.Infrastructure/Data/
- [ ] T136 [US7] Create IInspectionRepository interface in backend/src/Execora.Core/Interfaces/: IInspectionRepository.cs
- [ ] T137 [US7] Implement InspectionRepository in backend/src/Execora.Infrastructure/Repositories/: InspectionRepository.cs
- [ ] T138 [P] [US7] Create Inspection DTOs in backend/src/Execora.Application/DTOs/Inspections/: InspectionCreateRequestDto.cs, InspectionResponseDto.cs, InspectionSubmitRequestDto.cs
- [ ] T139 [US7] Create IInspectionService in backend/src/Execora.Application/Interfaces/: IInspectionService.cs, InspectionService.cs
- [ ] T140 [US7] Implement template-based checklist handler in backend/src/Execora.Application/Services/: ChecklistService.cs
- [ ] T141 [US7] Implement failed inspection → NCR workflow trigger in backend/src/Execora.Application/Services/: InspectionFailureHandler.cs
- [ ] T142 [US7] Create InspectionsController in backend/src/Execora.Api/Controllers/App/: InspectionsController.cs (endpoints: GET/POST /api/app/projects/{id}/inspections, GET /api/app/inspections/{id}, POST /api/app/inspections/{id}/submit)

### Frontend Implementation

- [ ] T143 [P] [US7] Create InspectionService in frontend/src/app/core/services/: InspectionService.ts
- [ ] T144 [P] [US7] Create inspection template management component in frontend/src/app/features/inspections/: TemplateListComponent.ts, template-list.component.html
- [ ] T145 [P] [US7] Create inspection list component in frontend/src/app/features/inspections/: InspectionListComponent.ts, inspection-list.component.html
- [ ] T146 [P] [US7] Create mobile-friendly inspection form component in frontend/src/app/features/inspections/: InspectionFormComponent.ts, inspection-form.component.html
- [ ] T147 [US7] Create checklist component with dynamic items in frontend/src/app/features/inspections/: ChecklistComponent.ts, checklist.component.html
- [ ] T148 [US7] Configure routing for inspections module in frontend/src/app/app-routing.module.ts

**Checkpoint**: Inspections feature complete with NCR workflow integration

---

## Phase 10: User Story 8 - NCR & Quality Intelligence (Priority: P3) - Professional Plan

**Goal**: NCRs are created, tracked through RCA, verified, and close with activity locking

**Independent Test**: Create NCR, submit RCA, verify corrective action, close NCR, verify activity unlocked

### Backend Implementation

- [ ] T149 [P] [US8] Create NonConformanceReport entity in backend/src/Execora.Core/Entities/: NonConformanceReport.cs
- [ ] T150 [P] [US8] Create NcrAttachment entity in backend/src/Execora.Core/Entities/: NcrAttachment.cs
- [ ] T151 [US8] Add DbSet for NCR entities to ExecoraDbContext in backend/src/Execora.Infrastructure/Data/
- [ ] T152 [US8] Create INcrRepository interface in backend/src/Execora.Core/Interfaces/: INcrRepository.cs
- [ ] T153 [US8] Implement NcrRepository in backend/src/Execora.Infrastructure/Repositories/: NcrRepository.cs
- [ ] T154 [P] [US8] Create NCR DTOs in backend/src/Execora.Application/DTOs/Ncr/: NcrCreateRequestDto.cs, NcrResponseDto.cs, NcrRcaRequestDto.cs
- [ ] T155 [US8] Create INcrService in backend/src/Execora.Application/Interfaces/: INcrService.cs, NcrService.cs
- [ ] T156 [US8] Implement RCA requirement enforcement (cannot close without RCA) in backend/src/Execora.Application/Services/: NcrRcaValidator.cs
- [ ] T157 [US8] Implement activity locking on NCR creation in backend/src/Execora.Application/Services/: ActivityLockService.cs
- [ ] T158 [US8] Create NcrController in backend/src/Execora.Api/Controllers/App/: NcrController.cs (endpoints: GET/POST /api/app/projects/{id}/ncrs, GET /api/app/ncrs/{id}, PUT /api/app/ncrs/{id}/rca, POST /api/app/ncrs/{id}/close)

### Frontend Implementation

- [ ] T159 [P] [US8] Create NcrService in frontend/src/app/core/services/: NcrService.ts
- [ ] T160 [P] [US8] Create NCR list/dashboard component in frontend/src/app/features/ncr/: NcrListComponent.ts, ncr-list.component.html
- [ ] T161 [P] [US8] Create NCR detail component in frontend/src/app/features/ncr/: NcrDetailComponent.ts, ncr-detail.component.html
- [ ] T162 [US8] Create RCA form component in frontend/src/app/features/ncr/: RcaFormComponent.ts, rca-form.component.html
- [ ] T163 [US8] Create NCR verification component in frontend/src/app/features/ncr/: NcrVerificationComponent.ts, ncr-verification.component.html
- [ ] T164 [US8] Configure routing for NCR module in frontend/src/app/app-routing.module.ts

**Checkpoint**: NCR quality intelligence complete with workflow enforcement

---

## Phase 11: User Story 9 - BIM & 3D Viewer Integration (Priority: P3) - Professional Plan

**Goal**: Upload BIM models, view in APS viewer, link elements to execution entities, color-code by state

**Independent Test**: Upload model, view in APS viewer, select element, link to activity, see color coding

### Backend Implementation

- [ ] T165 [P] [US9] Create BimModel entity in backend/src/Execora.Core/Entities/: BimModel.cs
- [ ] T166 [P] [US9] Create BimElementMapping entity in backend/src/Execora.Core/Entities/: BimElementMapping.cs
- [ ] T167 [US9] Add DbSet for BIM entities to ExecoraDbContext in backend/src/Execora.Infrastructure/Data/
- [ ] T168 [US9] Create IBimRepository interface in backend/src/Execora.Core/Interfaces/: IBimRepository.cs
- [ ] T169 [US9] Implement BimRepository in backend/src/Execora.Infrastructure/Repositories/: BimRepository.cs
- [ ] T170 [P] [US9] Create APS client service in backend/src/Execora.Infrastructure/Services/AutodeskAPS/: ApssServiceClient.cs, ApssAuthService.cs
- [ ] T171 [US9] Create IBimService in backend/src/Execora.Application/Interfaces/: IBimService.cs, BimService.cs
- [ ] T172 [US9] Implement model upload and derivative processing in backend/src/Execora.Application/Services/: BimUploadService.cs
- [ ] T173 [US9] Implement viewer token generation in backend/src/Execora.Application/Services/: BimTokenService.cs
- [ ] T174 [US9] Create BimController in backend/src/Execora.Api/Controllers/App/: BimController.cs (endpoints: GET/POST /api/app/projects/{id}/bim-models, GET /api/app/bim-models/{id}/viewer-token)

### Frontend Implementation

- [ ] T175 [P] [US9] Install and configure APS Viewer SDK in frontend/
- [ ] T176 [P] [US9] Create BimService in frontend/src/app/core/services/: BimService.ts
- [ ] T177 [P] [US9] Create BIM viewer component in frontend/src/app/features/bim/: BimViewerComponent.ts, bim-viewer.component.html
- [ ] T178 [US9] Create model selection component in frontend/src/app/features/bim/: BimModelSelectorComponent.ts, bim-model-selector.component.html
- [ ] T179 [US9] Implement element selection and linking in frontend/src/app/features/bim/: BimElementLinkComponent.ts
- [ ] T180 [US9] Implement workflow state color-coding in frontend/src/app/features/bim/: BimColorCodingService.ts
- [ ] T181 [US9] Configure routing for BIM module in frontend/src/app/app-routing.module.ts

**Checkpoint**: BIM integration complete with workflow-aware visualization

---

## Phase 12: User Story 10 - Dashboards & Analytics (Priority: P2)

**Goal**: Project dashboards, executive summaries, risk heatmaps, trend analysis

**Independent Test**: View project dashboard, see progress by trade, view risk heatmap, export report

### Backend Implementation

- [ ] T182 [P] [US10] Create aggregation services in backend/src/Execora.Application/Services/: DashboardAggregationService.cs, ProgressCalculationService.cs, RiskScoringService.cs
- [ ] T183 [P] [US10] Create Dashboard DTOs in backend/src/Execora.Application/DTOs/Dashboards/: ProjectDashboardDto.cs, ProgressByTradeDto.cs, IssuesBySeverityDto.cs
- [ ] T184 [US10] Create IDashboardService in backend/src/Execora.Application/Interfaces/: IDashboardService.cs, DashboardService.cs
- [ ] T185 [US10] Create DashboardsController in backend/src/Execora.Api/Controllers/App/: DashboardsController.cs (endpoints: GET /api/app/projects/{id}/dashboard)
- [ ] T186 [P] [US10] Create report export service (PDF, Excel) in backend/src/Execora.Application/Services/: ReportExportService.cs

### Frontend Implementation

- [ ] T187 [P] [US10] Create DashboardService in frontend/src/app/core/services/: DashboardService.ts
- [ ] T188 [P] [US10] Create project dashboard component in frontend/src/app/features/dashboards/: ProjectDashboardComponent.ts, project-dashboard.component.html
- [ ] T189 [P] [US10] Create progress chart components in frontend/src/app/features/dashboards/: ProgressChartComponent.ts, TradeProgressComponent.ts
- [ ] T190 [P] [US10] Create risk heatmap component in frontend/src/app/features/dashboards/: RiskHeatmapComponent.ts, risk-heatmap.component.html
- [ ] T191 [P] [US10] Create quality trends component in frontend/src/app/features/dashboards/: QualityTrendsComponent.ts, quality-trends.component.html
- [ ] T192 [US10] Configure routing for dashboards module in frontend/src/app/app-routing.module.ts

**Checkpoint**: Dashboards and analytics complete

---

## Phase 13: User Story 11 - System Administration (Priority: P2)

**Goal**: EXECORA internal team can manage tenants, subscriptions, audit logs, and impersonate users

**Independent Test**: Create tenant, update subscription, view audit logs, impersonate tenant user

### Backend Implementation

- [ ] T193 [P] [US11] Create SystemTenantsController in backend/src/Execora.Api/Controllers/Sys/: TenantsController.cs (endpoints: GET/POST /api/sys/tenants, GET/PUT/DELETE /api/sys/tenants/{id}, PUT /api/sys/tenants/{id}/subscription, GET /api/sys/tenants/{id}/usage)
- [ ] T194 [P] [US11] Create ImpersonationController in backend/src/Execora.Api/Controllers/Sys/: ImpersonationController.cs (endpoints: POST /api/sys/impersonation, POST /api/sys/impersonation/{token}/revoke)
- [ ] T195 [P] [US11] Create AuditLogController in backend/src/Execora.Api/Controllers/Sys/: AuditLogController.cs (endpoints: GET /api/sys/audit/logs)
- [ ] T196 [US11] Implement system admin authorization policy in backend/src/Execora.Auth/Policies/: SystemAdminPolicy.cs
- [ ] T197 [US11] Create system admin services in backend/src/Execora.Application/Services/: ITenantManagementService.cs, ISubscriptionService.cs, IImpersonationService.cs
- [ ] T198 [US11] Implement usage tracking service in backend/src/Execora.Application/Services/: TenantUsageService.cs

### Frontend Implementation

- [ ] T199 [P] [US11] Create system admin services in frontend/src/app/core/services/: SysAdminService.ts, SysTenantService.ts
- [ ] T200 [P] [US11] Create tenant management component in frontend/src/app/features/sys-admin/: TenantManagementComponent.ts, tenant-management.component.html
- [ ] T201 [P] [US11] Create subscription assignment component in frontend/src/app/features/sys-admin/: SubscriptionComponent.ts, subscription.component.html
- [ ] T202 [US11] Create audit log viewer component in frontend/src/app/features/sys-admin/: AuditLogViewerComponent.ts, audit-log-viewer.component.html
- [ ] T203 [US11] Create impersonation UI with banner in frontend/src/app/features/sys-admin/: ImpersonationComponent.ts, impersonation-banner.component.html
- [ ] T204 [US11] Create usage dashboard component in frontend/src/app/features/sys-admin/: UsageDashboardComponent.ts, usage-dashboard.component.html
- [ ] T205 [US11] Configure routing for system admin module in frontend/src/app/app-routing.module.ts

**Checkpoint**: System administration complete

---

## Phase 14: User Story 12 - Notifications (Priority: P2)

**Goal**: Users receive in-app and email notifications for relevant events

**Independent Test**: Trigger notification, verify in-app delivery, verify email delivery

### Backend Implementation

- [ ] T206 [P] [US12] Create Notification entity in backend/src/Execora.Core/Entities/: Notification.cs
- [ ] T207 [P] [US12] Create NotificationPreference entity in backend/src/Execora.Core/Entities/: NotificationPreference.cs
- [ ] T208 [US12] Add DbSet for notification entities to ExecoraDbContext in backend/src/Execora.Infrastructure/Data/
- [ ] T209 [P] [US12] Create email service (SendGrid) in backend/src/Execora.Infrastructure/Services/Email/: SendGridEmailService.cs, IEmailService.cs
- [ ] T210 [US12] Create SignalR hub for real-time notifications in backend/src/Execora.Api/Hubs/: NotificationHub.cs
- [ ] T211 [US12] Create notification service in backend/src/Execora.Application/Services/: NotificationService.cs, INotificationService.cs
- [ ] T212 [US12] Implement notification triggers for events in backend/src/Execora.Application/Services/: NotificationTriggerService.cs
- [ ] T213 [US12] Create NotificationsController in backend/src/Execora.Api/Controllers/App/: NotificationsController.cs (endpoints: GET /api/app/notifications, POST /api/app/notifications/mark-all-read, POST /api/app/notifications/{id}/read)

### Frontend Implementation

- [ ] T214 [P] [US12] Create NotificationService in frontend/src/app/core/services/: NotificationService.ts
- [ ] T215 [P] [US12] Create SignalR connection service in frontend/src/app/core/services/: SignalRService.cs
- [ ] T216 [P] [US12] Create notification center component in frontend/src/app/shared/components/: NotificationCenterComponent.ts, notification-center.component.html
- [ ] T217 [P] [US12] Create toast notification component in frontend/src/app/shared/components/: ToastNotificationComponent.ts, toast-notification.component.html
- [ ] T218 [US12] Create notification preferences component in frontend/src/app/features/settings/: NotificationPreferencesComponent.ts, notification-preferences.component.html

**Checkpoint**: Notifications complete with real-time delivery

---

## Phase 15: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

### Documentation

- [ ] T219 [P] Create README.md with project overview and setup instructions in repository root
- [ ] T220 [P] Create API documentation in docs/api/: endpoint-reference.md
- [ ] T221 [P] Create component documentation in docs/frontend/: component-library.md
- [ ] T222 Create deployment guide in docs/deploy/: azure-deployment.md

### Testing

- [ ] T223 [P] Add unit tests for authentication service in backend/tests/Unit/Services/: AuthenticationServiceTests.cs
- [ ] T224 [P] Add unit tests for project service in backend/tests/Unit/Services/: ProjectServiceTests.cs
- [ ] T225 [P] Add unit tests for activity service in backend/tests/Unit/Services/: ActivityServiceTests.cs
- [ ] T226 [P] Add unit tests for workflow engine in backend/tests/Unit/Workflow/: WorkflowEngineTests.cs
- [ ] T227 [P] Add integration tests for auth APIs in backend/tests/Integration/: AuthApiTests.cs
- [ ] T228 [P] Add integration tests for project APIs in backend/tests/Integration/: ProjectApiTests.cs
- [ ] T229 [P] Add frontend component tests in frontend/src/app/shared/components/: ButtonComponent.spec.ts, InputComponent.spec.ts
- [ ] T230 Add E2E tests for critical flows in frontend/src/: login.e2e.ts, project-creation.e2e.ts

### Security & Performance

- [ ] T231 Implement API rate limiting per tenant in backend/src/Execora.Api/Middleware/: RateLimitingMiddleware.cs
- [ ] T232 [P] Add SQL injection prevention validation in backend/src/Execora.Application/Validators/: SqlInjectionValidator.cs
- [ ] T233 [P] Configure HTTPS/TLS 1.3 enforcement in backend/src/Execora.Api/Program.cs
- [ ] T234 [P] Implement database indexes for performance in backend/src/Execora.Infrastructure/Data/: IndexConfiguration.cs
- [ ] T235 [P] Add response caching for static data in backend/src/Execora.Api/Middleware/: CacheControlMiddleware.cs

### Final Integration

- [ ] T236 Create main layout with navigation in frontend/src/app/features/layout/: MainLayoutComponent.ts, main-layout.component.html
- [ ] T237 Create sidebar navigation component in frontend/src/app/shared/layouts/: SidebarComponent.ts, sidebar.component.html
- [ ] T238 Create header with user menu component in frontend/src/app/shared/layouts/: HeaderComponent.ts, header.component.html
- [ ] T239 Create breadcrumb navigation component in frontend/src/app/shared/components/: BreadcrumbComponent.ts, breadcrumb.component.html
- [ ] T240 Configure brand colors in frontend/src/styles/: Crimson #B11226, Charcoal #1F2937, Insight Blue #2563EB
- [ ] T241 Run all tests and verify 80%+ code coverage
- [ ] T242 Run quickstart.md validation to verify developer onboarding

**Checkpoint**: Production-ready platform

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-14)**: All depend on Foundational phase completion
  - US1 (Auth) must be complete before US2-14 (requires authenticated user)
  - US2 (Projects) must be complete before US3 (Activities depend on Projects)
  - US6 (Workflow) must be complete before US7, US8 (Inspections, NCR depend on workflow)
  - US7 (Inspections) integrates with US8 (NCR) - can be developed in parallel
  - US8 (NCR) integrates with US3 (Activity locking)
  - US9 (BIM), US10 (Dashboards), US11 (Sys Admin), US12 (Notifications) are independent after foundational
- **Polish (Phase 15)**: Depends on all desired user stories being complete

### User Story Dependencies

```
Phase 1 (Setup) → Phase 2 (Foundation) → US1 (Auth) → US2 (Projects) → US3 (Activities)
                                                                   ↓
                                                            US4 (Daily Ops)
                                                            US5 (Issues)
                                                                   ↓
                                                            US6 (Workflow) → US7 (Inspections) → US8 (NCR) ┐
                                                                          ↑                           │
                                                                          └─────────── Activity Locking ─┘
                                                            US9 (BIM) - Independent after US2
                                                            US10 (Dashboards) - Independent after US2, US3
                                                            US11 (Sys Admin) - Independent after US1
                                                            US12 (Notifications) - Independent after US1
```

### Parallel Opportunities

Within each user story phase, tasks marked [P] can run in parallel:

- Phase 1: T003, T004, T005, T006, T007, T008, T009, T011, T012 can all run in parallel
- Phase 2: T014-T015, T019-T020, T023-T025 can run in parallel (backend foundation)
- Phase 2: T028-T033 can run in parallel (frontend foundation)
- Within each user story: Entity creation, DTO creation, and service creation can often run in parallel
- Different user stories can be worked on in parallel by different team members AFTER foundational phase

---

## Parallel Example: User Story 2 (Projects)

```bash
# Launch all entity creation tasks together:
Task: "Create Organization entity in backend/src/Execora.Core/Entities/: Organization.cs"
Task: "Create Project entity in backend/src/Execora.Core/Entities/: Project.cs"
Task: "Create ProjectUser entity in backend/src/Execora.Core/Entities/: ProjectUser.cs"
Task: "Create Trade entity in backend/src/Execora.Core/Entities/: Trade.cs"

# Launch all DTO creation tasks together:
Task: "Create Project DTOs in backend/src/Execora.Application/DTOs/Projects/"
Task: "Create Organization DTOs in backend/src/Execora.Application/DTOs/Organizations/"

# Launch all frontend service creations together:
Task: "Create ProjectService in frontend/src/app/core/services/: ProjectService.ts"
Task: "Create OrganizationService in frontend/src/app/core/services/: OrganizationService.ts"
```

---

## Implementation Strategy

### MVP First (User Stories 1-3 Only)

1. Complete Phase 1: Setup (T001-T012)
2. Complete Phase 2: Foundational (T013-T033)
3. Complete Phase 3: User Story 1 - Auth (T034-T046)
4. Complete Phase 4: User Story 2 - Projects (T047-T069)
5. Complete Phase 5: User Story 3 - Activities (T070-T087)
6. **STOP and VALIDATE**: Test Core plan functionality independently
7. Deploy/demo MVP

### Professional Plan (MVP + Quality & Workflow)

1. Complete MVP (Phases 1-5)
2. Complete Phase 6: User Story 6 - Workflow Engine (T116-T131)
3. Complete Phase 7: User Story 7 - Inspections (T132-T148)
4. Complete Phase 8: User Story 8 - NCR (T149-T164)
5. **STOP and VALIDATE**: Test Professional plan features
6. Deploy/demo Professional tier

### Enterprise Plan (All Features)

1. Complete Professional Plan (Phases 1-8)
2. Complete Phase 9: User Story 9 - BIM (T165-T181)
3. Complete Phase 10: User Story 10 - Dashboards (T182-T192)
4. Complete Phase 11: User Story 11 - System Admin (T193-T205)
5. Complete Phase 12: User Story 12 - Notifications (T206-T218)
6. Complete Phase 13: Polish (T219-T242)
7. **FINAL VALIDATION**: Full system testing
8. Production deployment

### Parallel Team Strategy

With multiple developers after foundational phase:

- **Developer A**: User Story 4 (Daily Ops) + User Story 5 (Issues)
- **Developer B**: User Story 6 (Workflow) + User Story 7 (Inspections)
- **Developer C**: User Story 8 (NCR) + User Story 12 (Notifications)
- **Developer D**: User Story 9 (BIM) + User Story 10 (Dashboards)
- **Developer E**: User Story 11 (System Admin) + Frontend polish

---

## Notes

- [P] tasks = different files, no dependencies, can run in parallel
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Verify tests fail before implementing (TDD approach per constitution)
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Total tasks: 242
- MVP tasks (Phases 1-5): 87 tasks
- Professional plan tasks (Phases 1-8): 164 tasks
- Enterprise plan tasks (Phases 1-15): 242 tasks

---

**END OF TASKS**
