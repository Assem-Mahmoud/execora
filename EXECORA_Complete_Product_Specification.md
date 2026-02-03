# EXECORA — Complete Product Specification (V2.0)
## Construction Execution Operating System (ExOS)
### *Execution, Enforced.*

---

## 1. Executive Overview
**EXECORA** is a workflow-driven construction execution platform designed to **enforce how projects are delivered**. It functions as an **Execution Operating System (ExOS)**, connecting daily site activities, quality, risk, and BIM into a single controlled workflow to provide real-time visibility and enforceable accountability.

---

## 2. Product Vision
Construction projects fail not because of a lack of tools, but because of a lack of enforced execution. EXECORA exists to:
- Eliminate execution ambiguity.
- Enforce company workflows through digital "Gating".
- Surface risk early and turn data into decisions.
- Create audit-ready execution records.

---

## 3. The Central Nervous System: Workflow Engine (Module 07)
The **Workflow Engine** is the core differentiator of EXECORA. It utilizes a **Finite State Machine (FSM)** to govern all modules, ensuring that no data enters the system without meeting quality standards.



**Engine Capabilities:**
- **Hard Gating:** Transitions between states (e.g., "WIP" to "Ready for Inspection") require mandatory data such as photos or GPS timestamps.
- **Conditional Logic:** A failed inspection automatically triggers an **NCR (Module 06)** and locks the parent **Activity (Module 03)**.
- **Role-Based Authority:** Only authorized personnel can trigger specific transitions, creating a digital "Chain of Command".
- **Temporal Compliance (SLAs):** Tracks "Time-in-State" and auto-escalates issues staying "Open" longer than defined thresholds.

---

## 4. Platform Architecture
- **Frontend:** Angular (Enterprise SPA) with Offline-Sync capability and tailwind styles.
- **Backend:** .NET (API-first, modular services).
- **Database:** SQL Server with an Immutable Audit Store.
- **BIM Viewer:** Autodesk Platform Services (APS / Forge).
- **Hosting:** Cloud-ready (Azure compatible).

---

## 5. Module Breakdown

### Module 01 — Project & Organization Management
Establishes structured governance and multi-project organization models.

### Module 02 — Smart Daily Operations
Captures site reality (Workforce, Productivity, Weather) via structured forms with automated API data integration.

### Module 03 — Activities (Execution Layer)
Connects planning to real execution with blocking logic that prevents progress on unverified work.

### Module 04 — Inspections & Quality Management
Enforces quality through mobile inspections with logic-linked checklists and mandatory re-inspection cycles.

### Module 05 — Issues & Risk Management
Treats issues as execution risks with severity levels, ownership enforcement, and automated escalations.

### Module 06 — NCR & Quality Intelligence
Converts defects into learning; users cannot close an NCR without a verified Root Cause Analysis (RCA).

### Module 08 — BIM & 3D Viewer Integration
Provides spatial context where elements are color-coded by **Workflow State** (e.g., Red = NCR, Green = Verified).

---

## 6. Integration Architecture: The Execution Sequence
EXECORA automates the "Discovery to Resolution" path to ensure project integrity:

1. **Trigger:** An Inspection is marked as "FAILED."
2. **Enforcement:** The Workflow Engine intercepts the state change and auto-generates a linked NCR.
3. **Visualization:** The BIM Viewer instantly updates the 3D element to **Red (#B11226)**.
4. **Locking:** The parent Activity is locked; the subcontractor cannot claim 100% progress until the NCR is "Closed."

---

## 7. Feature vs Plan Matrix

| Feature | Core | Professional | Enterprise |
| :--- | :---: | :---: | :---: |
| Workflow Engine | Basic | **Standard Enforcement** | **Custom Logic Builder** |
| Daily Operations | ✅ | ✅ | ✅ |
| Activities & Locking | ✅ | ✅ | ✅ |
| Inspections & NCR | ❌ | ✅ | ✅ |
| BIM & 3D Viewer | ❌ | ✅ | ✅ |
| Audit Trail | Basic | Full | Advanced |

---

## 8. Pricing & Brand
- **Pricing:** Positioned at <0.1% of project value (USD 15K to 80K+ per project).
- **Slogan:** Execution, Enforced.
- **Color Palette:** Crimson (#B11226), Charcoal (#1F2937), Insight Blue (#2563EB).

---
**END OF DOCUMENT**