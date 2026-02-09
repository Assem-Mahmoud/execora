# Tasks: Multi-Tenant Authentication & User Management

**Input**: Design documents from `/specs/001-multi-tenant-auth/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Test-First Development (TDD) will be enforced - unit tests for all services, integration tests for API endpoints, E2E tests for critical flows.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Backend**: `backend/src/` for .NET projects
- **Frontend**: `frontend/src/` for Angular projects
- **Tests**: `backend/tests/` for backend tests, `frontend/tests/` for frontend tests

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and dependency setup for authentication feature

- [X] T001 Add BCrypt.Net-Next NuGet package to backend/src/Execora.Auth/Execora.Auth.csproj
- [X] T002 [P] Add FluentValidation NuGet package to backend/src/Execora.Application/Execora.Application.csproj (Already exists)
- [X] T003 [P] Add System.IdentityModel.Tokens.Jwt NuGet package to backend/src/Execora.Auth/Execora.Auth.csproj
- [X] T004 Create backend/src/Execora.Application/DTOs directory for request/response DTOs
- [X] T005 Create backend/src/Execora.Application/Validators directory for FluentValidation validators
- [X] T006 [P] Create backend/tests/Execora.Tests.Unit/Services/Authentication directory for service tests
- [X] T007 [P] Create backend/tests/Execora.Tests.Integration/Controllers/Auth directory for API tests
- [X] T008 [P] Create frontend/src/app/core/services directory for core Angular services (Already exists)
- [X] T009 [P] Create frontend/src/app/core/guards directory for Angular route guards (Already exists)
- [X] T010 [P] Create frontend/src/app/core/interceptors directory for HTTP interceptors (Already exists)
- [X] T011 [P] Create frontend/src/app/features/auth directory for authentication feature module (Already exists)
- [X] T012 Create frontend/tests/e2e directory for Playwright E2E tests

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T013 Create InvitationStatus enum in backend/src/Execora.Core/Enums/InvitationStatus.cs
- [X] T014 [P] Create AuditAction enum in backend/src/Execora.Core/Enums/AuditAction.cs
- [X] T015 Create Invitation entity in backend/src/Execora.Core/Entities/Invitation.cs
- [X] T016 [P] Create RefreshToken entity in backend/src/Execora.Core/Entities/RefreshToken.cs
- [X] T017 [P] Create AuditLog entity in backend/src/Execora.Core/Entities/AuditLog.cs
- [X] T018 Create IInvitationRepository interface in backend/src/Execora.Core/Interfaces/IInvitationRepository.cs
- [X] T019 [P] Create IRefreshTokenRepository interface in backend/src/Execora.Core/Interfaces/IRefreshTokenRepository.cs
- [X] T020 Add new entities to AppDbContext in backend/src/Execora.Infrastructure/Data/AppDbContext.cs
- [X] T021 Create EF Core migration for new entities in backend/src/Execora.Infrastructure/Migrations
- [X] T022 Create IEmailService interface in backend/src/Execora.Infrastructure/Services/Email/IEmailService.cs
- [X] T023 Implement SmtpEmailService in backend/src/Execora.Infrastructure/Services/Email/SmtpEmailService.cs
- [X] T024 [P] Implement InvitationRepository in backend/src/Execora.Infrastructure/Repositories/InvitationRepository.cs
- [X] T025 [P] Implement RefreshTokenRepository in backend/src/Execora.Infrastructure/Repositories/RefreshTokenRepository.cs
- [X] T026 Extend UserRepository with new auth methods in backend/src/Execora.Infrastructure/Repositories/UserRepository.cs (Added PasswordHistory support)
- [X] T027 Create IPasswordHasher interface in backend/src/Execora.Auth/Services/IPasswordHasher.cs
- [X] T028 Implement PasswordHasher using BCrypt in backend/src/Execora.Auth/Services/PasswordHasher.cs
- [X] T029 Extend TokenService with tenant/user claims in backend/src/Execora.Auth/Services/TokenService.cs (Already implemented)
- [X] T030 Update RefreshTokenService to use database in backend/src/Execora.Auth/Services/RefreshTokenService.cs
- [X] T031 Create RateLimitMiddleware in backend/src/Execora.Api/Middleware/RateLimitMiddleware.cs
- [X] T032 Register all services in Program.cs in backend/src/Execora.Api/Program.cs
- [X] T033 [P] Create auth models in frontend/src/app/core/models/auth.models.ts
- [X] T034 [P] Create user models in frontend/src/app/core/models/user.models.ts (Already exists)
- [X] T035 Create base AuthService in frontend/src/app/core/services/auth.service.ts (Already exists)
- [X] T036 Create TokenService for token management in frontend/src/app/core/services/token.service.ts
- [X] T037 Create AuthInterceptor for token injection in frontend/src/app/core/interceptors/auth.interceptor.ts (Already exists)
- [X] T038 [P] Create TenantInterceptor for tenant context in frontend/src/app/core/interceptors/tenant.interceptor.ts (Already exists)
- [X] T039 Create AuthGuard for protected routes in frontend/src/app/core/guards/auth.guard.ts (Already exists)
- [X] T040 [P] Create EmailService interface in backend/src/Execora.Application/Services/IEmailService.cs (Already exists as IAuditLogService in Infrastructure)

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - User Registration & Tenant Creation (Priority: P1) 🎯 MVP

**Goal**: Enable new organizations to register and create their tenant

**Independent Test**: Navigate to registration page, create new tenant, verify tenant admin can log in and access tenant dashboard

### Unit Tests for User Story 1

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T041 [P] [US1] Create RegistrationServiceTests in backend/tests/Execora.Tests.Unit/Services/RegistrationServiceTests.cs
- [ ] T042 [P] [US1] Create RegistrationValidatorTests in backend/tests/Execora.Tests.Unit/Validators/RegistrationValidatorTests.cs

### Integration Tests for User Story 1

- [ ] T043 [P] [US1] Create RegisterTests endpoint tests in backend/tests/Execora.Tests.Integration/Controllers/Auth/RegisterTests.cs
- [ ] T044 [US1] Create registration E2E test in frontend/tests/e2e/registration.spec.ts

### Backend Implementation for User Story 1

- [ ] T045 [P] [US1] Create RegisterRequest DTO in backend/src/Execora.Application/DTOs/RegisterRequest.cs
- [ ] T046 [P] [US1] Create RegisterResponse DTO in backend/src/Execora.Application/DTOs/RegisterResponse.cs
- [ ] T047 [P] [US1] Create RegistrationValidator in backend/src/Execora.Application/Validators/RegistrationValidator.cs
- [ ] T048 [US1] Create IRegistrationService interface in backend/src/Execora.Application/Services/IRegistrationService.cs
- [ ] T049 [US1] Implement RegistrationService in backend/src/Execora.Application/Services/RegistrationService.cs (depends on T048)
- [ ] T050 [US1] Create RegisterController in backend/src/Execora.Api/Controllers/Auth/RegisterController.cs (depends on T049)
- [ ] T051 [US1] Add registration audit logging in backend/src/Execora.Application/Services/RegistrationService.cs (depends on T049)
- [ ] T052 [US1] Add email verification token generation in backend/src/Execora.Application/Services/RegistrationService.cs (depends on T049)

### Frontend Implementation for User Story 1

- [ ] T053 [P] [US1] Create register component in frontend/src/app/features/auth/register/register.component.ts
- [ ] T054 [P] [US1] Create register template in frontend/src/app/features/auth/register/register.component.html
- [ ] T055 [P] [US1] Create register styles in frontend/src/app/features/auth/register/register.component.scss
- [ ] T056 [US1] Implement registration form validation in frontend/src/app/features/auth/register/register.component.ts (depends on T053)
- [ ] T057 [US1] Wire registration API call in frontend/src/app/core/services/auth.service.ts (depends on T035)
- [ ] T058 [US1] Add registration routing in frontend/src/app/features/auth/auth.routes.ts
- [ ] T059 [US1] Create password-strength component in frontend/src/app/shared/components/password-strength/password-strength.component.ts

**Checkpoint**: User Story 1 complete - users can register and create tenants

---

## Phase 4: User Story 2 - Email Verification & Account Activation (Priority: P1)

**Goal**: Verify user email addresses before allowing login

**Independent Test**: Register new account, receive verification email, click link, confirm login works

### Unit Tests for User Story 2

- [ ] T060 [P] [US2] Create EmailVerificationServiceTests in backend/tests/Execora.Tests.Unit/Services/EmailVerificationServiceTests.cs

### Integration Tests for User Story 2

- [ ] T061 [P] [US2] Create VerifyEmailTests endpoint tests in backend/tests/Execora.Tests.Integration/Controllers/Auth/VerifyEmailTests.cs
- [ ] T062 [US2] Create email verification E2E test in frontend/tests/e2e/email-verification.spec.ts

### Backend Implementation for User Story 2

- [ ] T063 [P] [US2] Create VerifyEmailRequest DTO in backend/src/Execora.Application/DTOs/VerifyEmailRequest.cs
- [ ] T064 [P] [US2] Create VerifyEmailResponse DTO in backend/src/Execora.Application/DTOs/VerifyEmailResponse.cs
- [ ] T065 [P] [US2] Create ResendVerificationRequest DTO in backend/src/Execora.Application/DTOs/ResendVerificationRequest.cs
- [ ] T066 [US2] Implement email verification in backend/src/Execora.Application/Services/RegistrationService.cs (extends T049)
- [ ] T067 [US2] Add verify-email endpoint to backend/src/Execora.Api/Controllers/Auth/RegisterController.cs (depends on T066)
- [ ] T068 [US2] Add resend-verification endpoint to backend/src/Execora.Api/Controllers/Auth/RegisterController.cs (depends on T066)
- [ ] T069 [US2] Add email verification to login check in backend/src/Execora.Application/Services/AuthenticationService.cs

### Frontend Implementation for User Story 2

- [ ] T070 [P] [US2] Create verify-email component in frontend/src/app/features/auth/verify-email/verify-email.component.ts
- [ ] T071 [P] [US2] Create verify-email template in frontend/src/app/features/auth/verify-email/verify-email.component.html
- [ ] T072 [US2] Implement verification token handling in frontend/src/app/features/auth/verify-email/verify-email.component.ts (depends on T070)
- [ ] T073 [US2] Add resend verification functionality in frontend/src/app/features/auth/register/register.component.ts (extends T056)
- [ ] T074 [US2] Add verification routing in frontend/src/app/features/auth/auth.routes.ts

**Checkpoint**: User Stories 1 AND 2 complete - users can register, verify email, and prepare for login

---

## Phase 5: User Story 3 - User Login & Authentication (Priority: P1)

**Goal**: Authenticate users with email/password and issue JWT tokens

**Independent Test**: Enter valid credentials on login form, verify authentication and redirect to dashboard

### Unit Tests for User Story 3

- [ ] T075 [P] [US3] Create AuthenticationServiceTests in backend/tests/Execora.Tests.Unit/Services/AuthenticationServiceTests.cs
- [ ] T076 [P] [US3] Create PasswordHasherTests in backend/tests/Execora.Tests.Unit/Services/PasswordHasherTests.cs

### Integration Tests for User Story 3

- [ ] T077 [P] [US3] Create LoginTests endpoint tests in backend/tests/Execora.Tests.Integration/Controllers/Auth/LoginTests.cs
- [ ] T078 [US3] Create login E2E test in frontend/tests/e2e/login.spec.ts

### Backend Implementation for User Story 3

- [ ] T079 [P] [US3] Create LoginRequest DTO in backend/src/Execora.Application/DTOs/LoginRequest.cs
- [ ] T080 [P] [US3] Create LoginResponse DTO in backend/src/Execora.Application/DTOs/LoginResponse.cs
- [ ] T081 [US3] Create IAuthenticationService interface in backend/src/Execora.Application/Services/IAuthenticationService.cs
- [ ] T082 [US3] Implement AuthenticationService in backend/src/Execora.Application/Services/AuthenticationService.cs (depends on T081, T028)
- [ ] T083 [US3] Add account lockout logic in backend/src/Execora.Application/Services/AuthenticationService.cs (depends on T082)
- [ ] T084 [US3] Create LoginController in backend/src/Execora.Api/Controllers/Auth/LoginController.cs (depends on T082)
- [ ] T085 [US3] Add login audit logging in backend/src/Execora.Application/Services/AuthenticationService.cs (depends on T082)
- [ ] T086 [US3] Configure JWT Bearer authentication in backend/src/Execora.Api/Program.cs (depends on T029)

### Frontend Implementation for User Story 3

- [ ] T087 [P] [US3] Create login component in frontend/src/app/features/auth/login/login.component.ts
- [ ] T088 [P] [US3] Create login template in frontend/src/app/features/auth/login/login.component.html
- [ ] T089 [P] [US3] Create login styles in frontend/src/app/features/auth/login/login.component.scss
- [ ] T090 [US3] Implement login form validation in frontend/src/app/features/auth/login/login.component.ts (depends on T087)
- [ ] T091 [US3] Wire login API call in frontend/src/app/core/services/auth.service.ts (depends on T035)
- [ ] T092 [US3] Store tokens on successful login in frontend/src/app/core/services/token.service.ts (depends on T036)
- [ ] T093 [US3] Add login routing in frontend/src/app/features/auth/auth.routes.ts
- [ ] T094 [US3] Create auth-layout component in frontend/src/app/shared/layouts/auth-layout/auth-layout.component.ts

**Checkpoint**: User Story 3 complete - users can log in and receive JWT tokens

---

## Phase 6: User Story 7 - Token Refresh & Session Management (Priority: P1)

**Goal**: Maintain user sessions with refresh token rotation

**Independent Test**: Log in, wait for access token to near expiration, verify token refresh without re-authentication

### Unit Tests for User Story 7

- [ ] T095 [P] [US7] Create RefreshTokenServiceTests in backend/tests/Execora.Tests.Unit/Services/RefreshTokenServiceTests.cs

### Integration Tests for User Story 7

- [ ] T096 [P] [US7] Create TokenRefreshTests endpoint tests in backend/tests/Execora.Tests.Integration/Controllers/Auth/TokenRefreshTests.cs
- [ ] T097 [US7] Create token refresh E2E test in frontend/tests/e2e/token-refresh.spec.ts

### Backend Implementation for User Story 7

- [ ] T098 [P] [US7] Create RefreshTokenRequest DTO in backend/src/Execora.Application/DTOs/RefreshTokenRequest.cs
- [ ] T099 [P] [US7] Create RefreshTokenResponse DTO in backend/src/Execora.Application/DTOs/RefreshTokenResponse.cs
- [ ] T100 [US7] Implement refresh token rotation in backend/src/Execora.Auth/Services/RefreshTokenService.cs (extends T030)
- [ ] T101 [US7] Create TokenController in backend/src/Execora.Api/Controllers/Auth/TokenController.cs (depends on T100)
- [ ] T102 [US7] Add /refresh endpoint in backend/src/Execora.Api/Controllers/Auth/TokenController.cs (depends on T100)
- [ ] T103 [US7] Add /logout endpoint with token revocation in backend/src/Execora.Api/Controllers/Auth/TokenController.cs (depends on T100)
- [ ] T104 [US7] Invalidate all tokens on password reset in backend/src/Execora.Application/Services/PasswordResetService.cs

### Frontend Implementation for User Story 7

- [ ] T105 [US7] Implement token refresh logic in frontend/src/app/core/services/token.service.ts (depends on T036)
- [ ] T106 [US7] Add automatic token refresh in AuthInterceptor in frontend/src/app/core/interceptors/auth.interceptor.ts (depends on T037)
- [ ] T107 [US7] Handle token expiry and redirect to login in frontend/src/app/core/interceptors/auth.interceptor.ts (depends on T037)
- [ ] T108 [US7] Implement logout with token cleanup in frontend/src/app/core/services/auth.service.ts (depends on T035)
- [ ] T109 [US7] Add remember-me functionality in frontend/src/app/features/auth/login/login.component.ts (extends T090)

**Checkpoint**: User Story 7 complete - session management with secure token rotation

---

## Phase 7: User Story 4 - Password Reset (Priority: P1)

**Goal**: Allow users to reset forgotten passwords

**Independent Test**: Request password reset, receive email, use link to set new password, log in with new password

### Unit Tests for User Story 4

- [X] T110 [P] [US4] Create PasswordResetServiceTests in backend/tests/Execora.Tests.Unit/Services/PasswordResetServiceTests.cs
- [X] T111 [P] [US4] Create PasswordResetTests endpoint tests in backend/tests/Execora.Tests.Integration/Controllers/Auth/PasswordResetTests.cs
- [ ] T112 [US4] Create password reset E2E test in frontend/tests/e2e/password-reset.spec.ts

### Backend Implementation for User Story 4

- [X] T113 [P] [US4] Create ForgotPasswordRequest DTO in backend/src/Execora.Application/DTOs/ForgotPasswordRequest.cs
- [X] T114 [P] [US4] Create ResetPasswordRequest DTO in backend/src/Execora.Application/DTOs/ResetPasswordRequest.cs
- [X] T115 [P] [US4] Create ChangePasswordRequest DTO in backend/src/Execora.Application/DTOs/ChangePasswordRequest.cs
- [X] T116 [US4] Create IPasswordResetService interface in backend/src/Execora.Application/Services/IPasswordResetService.cs
- [X] T117 [US4] Implement PasswordResetService in backend/src/Execora.Application/Services/PasswordResetService.cs (depends on T116)
- [X] T118 [US4] Add password history tracking in backend/src/Execora.Infrastructure/Repositories/UserRepository.cs (extends T026)
- [X] T119 [US4] Create PasswordController in backend/src/Execora.Api/Controllers/Auth/PasswordController.cs (depends on T117)
- [X] T120 [US4] Add /forgot-password endpoint in backend/src/Execora.Api/Controllers/Auth/PasswordController.cs (depends on T117)
- [X] T121 [US4] Add /reset-password endpoint in backend/src/Execora.Api/Controllers/Auth/PasswordController.cs (depends on T117)
- [X] T122 [US4] Add /change-password endpoint in backend/src/Execora.Api/Controllers/Auth/PasswordController.cs (depends on T117)

### Frontend Implementation for User Story 4

- [X] T123 [P] [US4] Create forgot-password component in frontend/src/app/features/auth/forgot-password/forgot-password.component.ts
- [X] T124 [P] [US4] Create forgot-password template in frontend/src/app/features/auth/forgot-password/forgot-password.component.html
- [X] T125 [P] [US4] Create reset-password component in frontend/src/app/features/auth/reset-password/reset-password.component.ts
- [X] T126 [P] [US4] Create reset-password template in frontend/src/app/features/auth/reset-password/reset-password.component.html
- [X] T127 [US4] Implement forgot-password flow in frontend/src/app/features/auth/forgot-password/forgot-password.component.ts (depends on T123)
- [X] T128 [US4] Implement reset-password flow in frontend/src/app/features/auth/reset-password/reset-password.component.ts (depends on T125)
- [X] T129 [US4] Add password change form to user profile in frontend/src/app/features/users/profile/profile.component.ts
- [X] T130 [US4] Add password reset routing in frontend/src/app/features/auth/auth.routes.ts

**Status**: ✅ User Story 4 Backend - Complete (All endpoints implemented and functional)
**Status**: ✅ User Story 4 Frontend - Complete (All components implemented with 14/14 tests passing)
**Status**: ⏳ User Story 4 E2E Tests - Pending

---

## Phase 8: User Story 5 - User Invitation to Tenant (Priority: P1)

**Goal**: Enable Tenant Admins to invite users to their tenant

**Independent Test**: Tenant Admin sends invitation, invited user receives email, accepts invitation, accesses tenant

### Unit Tests for User Story 5

- [ ] T131 [P] [US5] Create InvitationServiceTests in backend/tests/Execora.Tests.Unit/Services/InvitationServiceTests.cs

### Integration Tests for User Story 5

- [ ] T132 [P] [US5] Create InvitationTests endpoint tests in backend/tests/Execora.Tests.Integration/Controllers/User/InvitationTests.cs
- [ ] T133 [US5] Create invitation E2E test in frontend/tests/e2e/invitation.spec.ts

### Backend Implementation for User Story 5

- [ ] T134 [P] [US5] Create InvitationCreateRequest DTO in backend/src/Execora.Application/DTOs/InvitationCreateRequest.cs
- [ ] T135 [P] [US5] Create InvitationResponse DTO in backend/src/Execora.Application/DTOs/InvitationResponse.cs
- [ ] T136 [US5] Create IInvitationService interface in backend/src/Execora.Application/Services/IInvitationService.cs
- [ ] T137 [US5] Implement InvitationService in backend/src/Execora.Application/Services/InvitationService.cs (depends on T136)
- [ ] T138 [US5] Create InvitationController in backend/src/Execora.Api/Controllers/User/InvitationController.cs (depends on T137)
- [ ] T139 [US5] Add /users/invite endpoint in backend/src/Execora.Api/Controllers/User/InvitationController.cs (depends on T137)
- [ ] T140 [US5] Add /users/invitations GET endpoint in backend/src/Execora.Api/Controllers/User/InvitationController.cs (depends on T137)
- [ ] T141 [US5] Add /users/invitations/{id}/cancel endpoint in backend/src/Execora.Api/Controllers/User/InvitationController.cs (depends on T137)
- [ ] T142 [US5] Add /users/invitations/{id}/resend endpoint in backend/src/Execora.Api/Controllers/User/InvitationController.cs (depends on T137)
- [ ] T143 [US5] Implement invitation acceptance flow in backend/src/Execora.Application/Services/InvitationService.cs (depends on T137)
- [ ] T144 [US5] Add invitation acceptance endpoint in backend/src/Execora.Api/Controllers/Auth/RegisterController.cs (extends T050)

### Frontend Implementation for User Story 5

- [ ] T145 [P] [US5] Create invite-user component in frontend/src/app/features/users/invite-user/invite-user.component.ts
- [ ] T146 [P] [US5] Create invite-user template in frontend/src/app/features/users/invite-user/invite-user.component.html
- [ ] T147 [P] [US5] Create invitation-list component in frontend/src/app/features/users/invitation-list/invitation-list.component.ts
- [ ] T148 [P] [US5] Create invitation-list template in frontend/src/app/features/users/invitation-list/invitation-list.component.html
- [ ] T149 [US5] Implement invitation form in frontend/src/app/features/users/invite-user/invite-user.component.ts (depends on T145)
- [ ] T150 [US5] Implement invitations list display in frontend/src/app/features/users/invitation-list/invitation-list.component.ts (depends on T147)
- [ ] T151 [US5] Create accept-invitation component in frontend/src/app/features/auth/accept-invitation/accept-invitation.component.ts
- [ ] T152 [US5] Create accept-invitation template in frontend/src/app/features/auth/accept-invitation/accept-invitation.component.html
- [ ] T153 [US5] Implement invitation acceptance flow in frontend/src/app/features/auth/accept-invitation/accept-invitation.component.ts (depends on T151)
- [ ] T154 [US5] Add user management routing in frontend/src/app/features/users/users.routes.ts

**Checkpoint**: User Story 5 complete - Tenant Admins can invite and manage users

---

## Phase 9: User Story 6 - Multi-Tenant User Access (Priority: P2)

**Goal**: Enable users with multiple tenant memberships to switch between tenants

**Independent Test**: Create user with access to two tenants, log in, switch between tenants, verify correct data/permissions

### Unit Tests for User Story 6

- [ ] T155 [P] [US6] Create TenantServiceTests in backend/tests/Execora.Tests.Unit/Services/TenantServiceTests.cs

### Integration Tests for User Story 6

- [ ] T156 [P] [US6] Create TenantSwitchTests endpoint tests in backend/tests/Execora.Tests.Integration/Controllers/User/TenantTests.cs
- [ ] T157 [US6] Create tenant switching E2E test in frontend/tests/e2e/tenant-switching.spec.ts

### Backend Implementation for User Story 6

- [ ] T158 [P] [US6] Create TenantResponse DTO in backend/src/Execora.Application/DTOs/TenantResponse.cs
- [ ] T159 [P] [US6] Create TenantsListResponse DTO in backend/src/Execora.Application/DTOs/TenantsListResponse.cs
- [ ] T160 [P] [US6] Create SwitchTenantRequest DTO in backend/src/Execora.Application/DTOs/SwitchTenantRequest.cs
- [ ] T161 [P] [US6] Create SwitchTenantResponse DTO in backend/src/Execora.Application/DTOs/SwitchTenantResponse.cs
- [ ] T162 [US6] Create ITenantService interface in backend/src/Execora.Application/Services/ITenantService.cs
- [ ] T163 [US6] Implement TenantService in backend/src/Execora.Application/Services/TenantService.cs (depends on T162)
- [ ] T164 [US6] Create TenantController in backend/src/Execora.Api/Controllers/User/TenantController.cs (depends on T163)
- [ ] T165 [US6] Add /tenants GET endpoint in backend/src/Execora.Api/Controllers/User/TenantController.cs (depends on T163)
- [ ] T166 [US6] Add /tenants/{id}/switch POST endpoint in backend/src/Execora.Api/Controllers/User/TenantController.cs (depends on T163)
- [ ] T167 [US6] Update token with new tenant claims on switch in backend/src/Execora.Auth/Services/TokenService.cs (extends T029)
- [ ] T168 [US6] Add tenant switch audit logging in backend/src/Execora.Application/Services/TenantService.cs (depends on T163)

### Frontend Implementation for User Story 6

- [ ] T169 [P] [US6] Create tenant-switcher component in frontend/src/app/shared/components/tenant-switcher/tenant-switcher.component.ts
- [ ] T170 [P] [US6] Create tenant-switcher template in frontend/src/app/shared/components/tenant-switcher/tenant-switcher.component.html
- [ ] T171 [US6] Implement tenant switching in frontend/src/app/core/services/auth.service.ts (depends on T035)
- [ ] T172 [US6] Store last active tenant in preferences in frontend/src/app/core/services/token.service.ts (depends on T036)
- [ ] T173 [US6] Display available tenants in switcher in frontend/src/app/shared/components/tenant-switcher/tenant-switcher.component.ts (depends on T169)
- [ ] T174 [US6] Update UI on tenant switch in frontend/src/app/shared/components/tenant-switcher/tenant-switcher.component.ts (depends on T169)
- [ ] T175 [US6] Redirect to last active tenant on login in frontend/src/app/features/auth/login/login.component.ts (extends T090)

**Checkpoint**: User Story 6 complete - Multi-tenant users can switch between tenants

---

## Phase 10: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T176 [P] Create UserProfileResponse DTO in backend/src/Execora.Application/DTOs/UserProfileResponse.cs
- [ ] T177 [P] Create UpdateProfileRequest DTO in backend/src/Execora.Application/DTOs/UpdateProfileRequest.cs
- [ ] T178 [P] Create UpdateUserRoleRequest DTO in backend/src/Execora.Application/DTOs/UpdateUserRoleRequest.cs
- [ ] T179 Create ProfileController in backend/src/Execora.Api/Controllers/User/ProfileController.cs
- [ ] T180 Add /users/me GET endpoint in backend/src/Execora.Api/Controllers/User/ProfileController.cs
- [ ] T181 Add /users/me PUT endpoint in backend/src/Execora.Api/Controllers/User/ProfileController.cs
- [ ] T182 Add /users GET endpoint for tenant user listing in backend/src/Execora.Api/Controllers/User/ProfileController.cs
- [ ] T183 Add /users/{id} PUT endpoint for role updates in backend/src/Execora.Api/Controllers/User/ProfileController.cs
- [ ] T184 Add /users/{id} DELETE endpoint in backend/src/Execora.Api/Controllers/User/ProfileController.cs
- [ ] T185 [P] Create user-list component in frontend/src/app/features/users/user-list/user-list.component.ts
- [ ] T186 [P] Create user-list template in frontend/src/app/features/users/user-list/user-list.component.html
- [ ] T187 [P] Create profile component in frontend/src/app/features/users/profile/profile.component.ts
- [ ] T188 [P] Create profile template in frontend/src/app/features/users/profile/profile.component.html
- [ ] T189 Implement user management in frontend/src/app/features/users/user-list/user-list.component.ts (depends on T185)
- [ ] T190 Implement profile management in frontend/src/app/features/users/profile/profile.component.ts (depends on T187)
- [ ] T191 Add error handling for auth errors in frontend/src/app/core/interceptors/error.interceptor.ts
- [ ] T192 Add loading states for auth flows in frontend/src/app/core/services/auth.service.ts
- [ ] T193 [P] Add unit tests for AuthService in frontend/tests/unit/services/auth.service.spec.ts
- [ ] T194 [P] Add unit tests for TokenService in frontend/tests/unit/services/token.service.spec.ts
- [ ] T195 [P] Add unit tests for AuthGuard in frontend/tests/unit/guards/auth.guard.spec.ts
- [ ] T196 Update OpenAPI/Swagger documentation for all auth endpoints in backend/src/Execora.Api/Program.cs
- [ ] T197 Configure CORS for Angular frontend in backend/src/Execora.Api/Program.cs
- [ ] T198 Add cookie configuration for HttpOnly tokens in backend/src/Execora.Api/Program.cs
- [ ] T199 Configure rate limiting per endpoint in backend/src/Execora.Api/Middleware/RateLimitMiddleware.cs
- [ ] T200 Add internationalization for auth strings in frontend/src/assets/i18n/en.json
- [ ] T201 Run all backend unit tests and verify 80%+ coverage in backend/tests/Execora.Tests.Unit
- [ ] T202 Run all backend integration tests in backend/tests/Execora.Tests.Integration
- [ ] T203 Run all frontend unit tests in frontend/tests/unit
- [ ] T204 Run all E2E tests in frontend/tests/e2e
- [ ] T205 Validate quickstart.md setup instructions work correctly
- [ ] T206 Security review: Verify SQL injection protection in backend/src/Execora.Infrastructure/Repositories
- [ ] T207 Security review: Verify XSS protection in frontend components
- [ ] T208 Security review: Verify CSRF protection in backend/src/Execora.Api/Program.cs
- [ ] T209 Performance test: Verify <2s login time
- [ ] T210 Performance test: Verify <500ms token refresh time
- [ ] T211 Verify Row-Level Security configuration in database
- [ ] T212 Verify audit logging for all security events
- [ ] T213 Update API documentation with examples from contracts/auth-api.yaml
- [ ] T214 Update API documentation with examples from contracts/invitation-api.yaml

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phases 3-9)**: All depend on Foundational phase completion
  - US1 (Registration) must complete before US2 (Email Verification)
  - US7 (Token Refresh) should follow US3 (Login)
  - US5 (Invitations) is independent after US3
  - US6 (Multi-Tenant) is independent after US3
  - US4 (Password Reset) is independent after US3
- **Polish (Phase 10)**: Depends on all desired user stories being complete

### User Story Dependencies

```
Foundational (Phase 2)
    │
    ├─── US1: User Registration (Phase 3)
    │       │
    │       └─── US2: Email Verification (Phase 4)
    │
    ├─── US3: User Login (Phase 5)
    │       │
    │       ├─── US7: Token Refresh (Phase 6)
    │       │
    │       ├─── US4: Password Reset (Phase 7) [can run parallel with US7]
    │       │
    │       ├─── US5: User Invitation (Phase 8) [can run parallel with US7]
    │       │
    │       └─── US6: Multi-Tenant Access (Phase 9) [can run parallel with US7]
    │
    └─── Polish (Phase 10)
```

### Within Each User Story

1. Unit tests written FIRST (must fail)
2. Integration tests written SECOND (must fail)
3. Models/DTOs created
4. Services implemented (tests now pass)
5. Controllers/endpoints implemented
6. Frontend components implemented
7. E2E tests written and pass
8. Story complete and independently testable

### Parallel Opportunities

**Setup Phase (Phase 1)**:
- T002-T012: All 11 setup tasks can run in parallel

**Foundational Phase (Phase 2)**:
- T014, T016, T017: Enums and entities can run in parallel
- T024, T025: Repository implementations can run in parallel
- T033, T034: Frontend models can run in parallel
- T038, T039: Interceptors/guard can run in parallel

**After Foundational Phase**:
- US1 (Phase 3), US3 (Phase 5), US5 (Phase 8), US6 (Phase 9) can all start in parallel by different developers
- US7 (Phase 6) and US4 (Phase 7) can start in parallel once US3 is complete

**Within Each User Story**:
- All unit tests (T0xx[P]) can be written in parallel
- All integration tests (T0xx[P]) can be written in parallel
- All DTOs (T0xx[P]) can be created in parallel
- All frontend components (T0xx[P]) can be created in parallel

---

## Parallel Example: User Story 1

```bash
# Launch all unit tests for User Story 1 together:
Task T041: "Create RegistrationServiceTests in backend/tests/Execora.Tests.Unit/Services/"
Task T042: "Create RegistrationValidatorTests in backend/tests/Execora.Tests.Unit/Validators/"

# Launch all integration tests for User Story 1 together:
Task T043: "Create RegisterTests endpoint tests in backend/tests/Execora.Tests.Integration/"
Task T044: "Create registration E2E test in frontend/tests/e2e/"

# Launch all DTOs for User Story 1 together:
Task T045: "Create RegisterRequest DTO in backend/src/Execora.Application/DTOs/"
Task T046: "Create RegisterResponse DTO in backend/src/Execora.Application/DTOs/"

# Launch all frontend components for User Story 1 together:
Task T053: "Create register component in frontend/src/app/features/auth/register/"
Task T054: "Create register template in frontend/src/app/features/auth/register/"
Task T055: "Create register styles in frontend/src/app/features/auth/register/"
```

---

## Parallel Example: User Story 3 (after Foundational complete)

```bash
# Developer A can work on User Story 3 while Developer B works on User Story 1:
Developer A Tasks (US3): T075-T094
Developer B Tasks (US1): T041-T059
```

---

## Implementation Strategy

### MVP First (User Stories 1, 2, 3, 7)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1 (Registration)
4. Complete Phase 4: User Story 2 (Email Verification)
5. Complete Phase 5: User Story 3 (Login)
6. Complete Phase 6: User Story 7 (Token Refresh)
7. **STOP and VALIDATE**: Test core auth flow independently (register → verify → login → refresh)
8. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add US1 + US2 → Test independently → Registration flow works
3. Add US3 + US7 → Test independently → Full login flow works (MVP!)
4. Add US4 → Test independently → Password reset works
5. Add US5 → Test independently → Team collaboration works
6. Add US6 → Test independently → Multi-tenant access works
7. Each story adds value without breaking previous stories

### Parallel Team Strategy

With 4 developers after Foundational phase:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: US1 (Registration) + US2 (Email Verification)
   - Developer B: US3 (Login) + US7 (Token Refresh)
   - Developer C: US4 (Password Reset) [can start after US3]
   - Developer D: US5 (Invitations) [can start after US3]
3. Developer A or B adds US6 (Multi-Tenant) after US3 complete
4. All developers converge on Polish phase

---

## Summary

| Metric | Count |
|--------|-------|
| **Total Tasks** | 214 |
| **Setup Tasks** | 12 |
| **Foundational Tasks** | 28 |
| **User Story 1 (Registration)** | 19 |
| **User Story 2 (Email Verification)** | 15 |
| **User Story 3 (Login)** | 20 |
| **User Story 7 (Token Refresh)** | 15 |
| **User Story 4 (Password Reset)** | 20 |
| **User Story 5 (Invitation)** | 24 |
| **User Story 6 (Multi-Tenant)** | 21 |
| **Polish Tasks** | 39 |
| **Parallelizable Tasks** | ~80 (marked with [P]) |

### Independent Test Criteria per Story

- **US1**: Register new tenant, verify tenant created, admin can log in
- **US2**: Register, receive email, click link, verify login enabled
- **US3**: Verified user can log in with valid credentials
- **US7**: Logged-in user maintains session after access token expires
- **US4**: User can reset password via email and log in with new password
- **US5**: Tenant Admin can invite user, invited user accepts and accesses tenant
- **US6**: Multi-tenant user can switch between tenants and see correct data

### MVP Scope

**Recommended MVP**: User Stories 1, 2, 3, 7
- Enables user registration with email verification
- Enables secure login and session management
- Foundational for all other features
- Estimated: ~70 backend tasks, ~40 frontend tasks

---

**END OF TASKS**
