
# EXECORA — Complete Product Specification (Multi-Tenant)
## Construction Execution Operating System
### *Execution, Enforced.*

---

## 1. Executive Overview

**EXECORA** is a **multi-tenant, workflow-driven construction execution platform** designed to enforce how projects are delivered.

The platform supports **multiple organizations (tenants)** with strict data isolation, independent users, projects, workflows, and subscriptions.

---

## 2. Multi-Tenant Foundation

- Tenant = one organization
- Full data isolation per tenant
- Independent projects, portfolios, users, and workflows
- Pricing and subscriptions applied per tenant

---

## 3. Admin Scopes, Roles & Navigation Model

EXECORA enforces a strict separation between **platform administration** and **tenant usage**.

---

### 3.1 System Admin (EXECORA Platform Admin)

**Scope:** Global  
**Audience:** EXECORA internal team  

**Access:**
- Organization (Tenant) Management
- Subscription & Plan Assignment
- Platform Configuration
- System Audit Logs
- Tenant Impersonation (audited)

❌ No access to:
- Projects
- Execution data
- BIM models
- Dashboards

**Default Landing Page:**
```
/sys/organizations
```

---

### 3.2 Tenant Admin (Customer Admin)

**Scope:** Single Tenant  

**Access:**
- Organization settings
- Projects management
- Users & roles
- Workflow configuration (plan-based)

**Default Landing Page:**
```
/app/projects
```

---

### 3.3 Project Users

**Scope:** Assigned projects only  

Examples:
- Project Manager
- QA/QC Engineer
- Site Engineer

---

### 3.4 Role Hierarchy Summary

| Role | Scope | Access |
|----|------|--------|
| SystemAdmin | Global | Organizations only |
| TenantAdmin | Tenant | Org + Projects |
| ProjectAdmin | Project | Configuration |
| ProjectManager | Project | Execution |
| QAQC | Project | Quality |
| SiteEngineer | Project | Site Ops |

---

### 3.5 Navigation Rules

**System Admin UI**
- Organizations
- Subscriptions
- Platform Settings
- Audit Logs

**Tenant / Project UI**
- Projects
- Dashboards
- Activities
- Issues
- Inspections
- NCRs
- BIM Viewer
- Reports

---

### 3.6 API & Authorization Enforcement

- System APIs:
```
/api/sys/*
```

- Tenant APIs:
```
/api/app/*
```

- TenantId required on all tenant requests
- Cross-tenant access technically blocked

---

### 3.7 Impersonation (Optional)

- Allowed for System Admin
- Fully audited
- Time-limited
- Banner indication in UI

---

## 4. Closing Statement

This model ensures:
- Enterprise-grade security
- Strong tenant trust
- SaaS scalability
- Compliance readiness

---

**END OF DOCUMENT**
