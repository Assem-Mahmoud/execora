# EXECORA — Feature Specification
## Construction Execution Operating System (ExOS)

---

## 1. Executive Overview

**EXECORA** is a multi-tenant, workflow-driven construction execution platform designed to enforce how projects are delivered. The platform supports multiple organizations (tenants) with strict data isolation, independent users, projects, workflows, and subscriptions.

---

## 2. Multi-Tenant Architecture

- **Tenant** = one organization/customer
- Full data isolation per tenant
- Independent projects, portfolios, users, and workflows
- Pricing and subscriptions applied per tenant

---

## 3. Admin Scopes, Roles & Navigation

### 3.1 System Admin (EXECORA Platform Admin)
- **Scope**: Global
- **Audience**: EXECORA internal team
- **Access**: Organization Management, Subscription & Plan Assignment, Platform Configuration, System Audit Logs, Tenant Impersonation (audited)
- **No Access To**: Projects, Execution data, BIM models, Dashboards
- **Default Landing**: `/sys/organizations`

### 3.2 Tenant Admin (Customer Admin)
- **Scope**: Single Tenant
- **Access**: Organization settings, Projects management, Users & roles, Workflow configuration
- **Default Landing**: `/app/projects`

### 3.3 Project Users
- **Scope**: Assigned projects only
- **Examples**: Project Manager, QA/QC Engineer, Site Engineer

### 3.4 Role Hierarchy

| Role | Scope | Access |
|-----|-------|--------|
| SystemAdmin | Global | Organizations only |
| TenantAdmin | Tenant | Org + Projects |
| ProjectAdmin | Project | Configuration |
| ProjectManager | Project | Execution |
| QAQC | Project | Quality |
| SiteEngineer | Project | Site Ops |

### 3.5 API Structure
- **System APIs**: `/api/sys/*`
- **Tenant APIs**: `/api/app/*`
- **TenantId** required on all tenant requests
- Cross-tenant access technically blocked

---

## 4. Module Breakdown

### Module 01 — Project & Organization Management
**Purpose**: Establish structured governance across projects.

**Features**:
- Project creation & configuration
- Role-based access control
- Trade & team structure
- Multi-project organization model

**Plans**: Core, Professional, Enterprise

---

### Module 02 — Smart Daily Operations
**Purpose**: Capture daily site reality in structured form.

**Features**:
- Daily reports
- Workforce & productivity tracking
- Weather & constraints (automated API data integration)
- Attachments & evidence

**Plans**: Core, Professional, Enterprise

---

### Module 03 — Activities (Execution Layer)
**Purpose**: Connect planning to real execution.

**Features**:
- Activity definitions
- Progress tracking
- **Blocking logic** (prevents progress on unverified work)
- Execution status

**Plans**: Core, Professional, Enterprise

---

### Module 04 — Inspections & Quality Management
**Purpose**: Enforce quality through workflows.

**Features**:
- Inspection templates
- Mobile inspections
- Mandatory evidence (photos, GPS timestamps)
- Pass / Fail logic
- Re-inspections

**Plans**: Professional, Enterprise

---

### Module 05 — Issues & Risk Management
**Purpose**: Treat issues as execution risks.

**Features**:
- Issue categorization
- Severity & SLA tracking
- Ownership enforcement
- Automated escalations

**Plans**: Core (Basic), Professional, Enterprise

---

### Module 06 — NCR & Quality Intelligence
**Purpose**: Convert defects into learning.

**Features**:
- NCR lifecycle management
- Root cause analysis (RCA) required for closure
- Corrective & preventive actions
- Re-inspections

**Plans**: Professional, Enterprise

---

### Module 07 — Workflow Engine
**Purpose**: Enforce company-specific execution logic.

**Features**:
- **Finite State Machine (FSM)** for state transitions
- **Hard Gating**: Transitions require mandatory data (photos, GPS, etc.)
- **Conditional Logic**: Failed inspection → auto-generates NCR → locks Activity
- **Role-Based Authority**: Only authorized personnel can trigger transitions
- **Temporal Compliance (SLAs)**: Time-in-State tracking with auto-escalation
- Workflow versioning

**Plans**: Professional, Enterprise

---

### Module 08 — BIM & 3D Viewer Integration
**Purpose**: Provide spatial context to execution.

**Features**:
- APS (Autodesk Platform Services) Viewer integration
- Model versioning
- Element-level linking
- Color-coded by workflow state (Red = NCR, Green = Verified)
- Viewer snapshots

**Plans**: Professional, Enterprise

---

### Module 09 — Dashboards & Decision Intelligence
**Purpose**: Turn data into decisions.

**Features**:
- Project dashboards
- Risk & quality trends
- Portfolio views
- Power BI export

**Plans**: Core (Basic), Professional, Enterprise

---

### Module 10 — Notifications & Audit Trail
**Purpose**: Ensure accountability and compliance.

**Features**:
- In-app notifications
- Email notifications (Professional+)
- Action-based triggers
- Immutable audit logs
- Timeline views

**Plans**: Core (In-App only), Professional, Enterprise

---

## 5. Integration Architecture: The Execution Sequence

EXECORA automates the "Discovery to Resolution" path:

1. **Trigger**: Inspection marked as "FAILED"
2. **Enforcement**: Workflow Engine auto-generates linked NCR
3. **Visualization**: BIM Viewer updates element to **Red (#B11226)**
4. **Locking**: Parent Activity locked; subcontractor cannot claim 100% progress until NCR is "Closed"

---

## 6. Subscription Plans

| Feature | Core | Professional | Enterprise |
|---------|:----:|:------------:|:----------:|
| Project Management | ✅ | ✅ | ✅ |
| Activities | ✅ | ✅ | ✅ |
| Daily Operations | ✅ | ✅ | ✅ |
| Issues & Risk | Basic | ✅ | ✅ |
| Workflow Engine | ❌ | ✅ | ✅ |
| Inspections | ❌ | ✅ | ✅ |
| NCR Management | ❌ | ✅ | ✅ |
| BIM & 3D Viewer | ❌ | ✅ | ✅ |
| Dashboards | Basic | Advanced | Portfolio |
| Notifications | In-App | In-App + Email | Full |
| Audit Trail | Basic | Full | Advanced |
| Custom Workflows | ❌ | ❌ | ✅ |
| Dedicated Support | ❌ | ❌ | ✅ |

**Pricing (Annual/Project)**:
- **CORE**: USD 15K–20K
- **PROFESSIONAL**: USD 30K–40K
- **ENTERPRISE**: USD 60K–80K or USD 250K+ (portfolio license)

---

## 7. Brand Identity

- **Name**: EXECORA
- **Slogan**: Execution, Enforced.
- **Tone**: Authoritative, calm, enterprise-grade
- **Primary Color**: Crimson (#B11226)
- **Secondary**: Charcoal (#1F2937)
- **Accent**: Insight Blue (#2563EB)

---

## 8. Technical Requirements Summary

- **Frontend**: Angular (Enterprise SPA) with Tailwind CSS, Offline-Sync capability
- **Backend**: .NET (latest) API-first, modular services
- **Database**: SQL Server with Immutable Audit Store
- **BIM Viewer**: Autodesk Platform Services (APS / Forge)
- **Hosting**: Cloud-ready (Azure compatible)
- **Auth**: Role-based access control with multi-tenant isolation

---

**END OF SPECIFICATION**
