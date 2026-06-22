# PropertyOS — Agile Sprint Plan

## Overview

This document defines the Agile development structure for PropertyOS, a full-stack Property Management System.

The project is managed using Agile methodology with iterative 2-week sprints, enabling incremental delivery of features across web, mobile, and backend systems.

This structure supports:
- MVP development
- PLAR academic evidence
- Professional software engineering portfolio standards

---

# 🧱 Agile Structure

## Workflow Columns (Kanban)

Tasks will move through the following stages:

- 📥 Backlog (planned but not started)
- 🟡 Ready for Sprint (prioritised for upcoming sprint)
- 🔵 In Progress (actively being developed)
- 🧪 Testing / Review (QA and validation)
- ✅ Done (completed and deployed)

---

# 🧩 EPICS (Major System Areas)

The project is divided into the following epics:

- Epic 1: Project Foundation
- Epic 2: Authentication & User Management
- Epic 3: Property & Unit Management
- Epic 4: Maintenance Ticket System
- Epic 5: Rent Tracking System
- Epic 6: Dashboard & UI Layer
- Epic 7: Mobile Application (React Native)
- Epic 8: Backend Business Logic (.NET API)
- Epic 9: Deployment & Production Readiness

---

# 🚀 SPRINT STRUCTURE

Each sprint is 2 weeks long and delivers a working increment of the system.

---

## 🟢 Sprint 1 — Foundation & System Setup

### Goal:
Establish project structure, tooling, and database foundation.

### Tasks:
- [✅] Create GitHub repository (PropertyOS)
- [ ] Initialize Next.js frontend (TypeScript)
- [ ] Setup Supabase project (database + auth)
- [ ] Define initial database schema (draft)
- [ ] Create system-design.md
- [✅] Create requirements.md
- [✅] Setup monorepo folder structure
- [ ] Configure environment variables

### Deliverable:
- Running frontend + connected Supabase project
- Initial architecture and documentation complete

---

## 🔐 Sprint 2 — Authentication & Roles

### Goal:
Implement secure login system with role-based access.

### Tasks:
- [ ] Implement Supabase authentication
- [ ] Create user roles (tenant, landlord, admin)
- [ ] Build protected routes in Next.js
- [ ] Create user profile table
- [ ] Role-based dashboard routing

### Deliverable:
- Functional login system
- Role-based access control working

---

## 🏢 Sprint 3 — Property Management Core

### Goal:
Enable landlords to manage properties and units.

### Tasks:
- [ ] Create properties table and UI
- [ ] Create units table and relationships
- [ ] Assign tenants to units
- [ ] Implement CRUD operations for properties
- [ ] Build property dashboard UI

### Deliverable:
- Functional property + unit management system

---

## 🧰 Sprint 4 — Maintenance System

### Goal:
Implement full maintenance ticket workflow.

### Tasks:
- [ ] Create maintenance_tickets table
- [ ] Build ticket submission form (tenant)
- [ ] Enable image uploads (Supabase Storage)
- [ ] Build landlord ticket dashboard
- [ ] Implement ticket status updates

### Deliverable:
- End-to-end maintenance request system

---

## 💸 Sprint 5 — Rent Tracking System

### Goal:
Implement rent tracking and payment status system.

### Tasks:
- [ ] Create rent tracking tables
- [ ] Assign rent per unit
- [ ] Track monthly payments
- [ ] Implement paid/unpaid/overdue status
- [ ] Build rent dashboard UI

### Deliverable:
- Functional rent tracking system

---

## 📊 Sprint 6 — Dashboard & UI Enhancements

### Goal:
Improve usability and data visibility.

### Tasks:
- [ ] Build landlord dashboard
- [ ] Build tenant dashboard
- [ ] Add KPI summary cards
- [ ] Implement filtering and search
- [ ] Improve UI styling (Tailwind CSS)

### Deliverable:
- Polished and usable UI

---

## 📱 Sprint 7 — Mobile Application (React Native)

### Goal:
Build tenant mobile experience.

### Tasks:
- [ ] Setup Expo project
- [ ] Implement mobile authentication
- [ ] Build maintenance request form
- [ ] Enable camera/image upload
- [ ] Display ticket status updates

### Deliverable:
- Functional tenant mobile app

---

## 🧠 Sprint 8 — Backend Business Logic (.NET)

### Goal:
Add business logic layer for advanced processing.

### Tasks:
- [ ] Create ASP.NET Core API
- [ ] Implement rent calculation logic
- [ ] Build maintenance prioritisation system
- [ ] Create reporting endpoints
- [ ] Connect API to Supabase database

### Deliverable:
- Functional backend API layer

---

## 🚀 Sprint 9 — Deployment & Production Readiness

### Goal:
Prepare system for deployment and portfolio showcase.

### Tasks:
- [ ] Deploy Next.js frontend (Vercel)
- [ ] Deploy backend API
- [ ] Configure environment variables
- [ ] Validate security rules (RLS)
- [ ] Final UI/UX polish
- [ ] Complete README documentation

### Deliverable:
- Fully deployed MVP system

---

# 🧠 Notes

- Sprints may overlap depending on development speed
- Each task should be tracked in GitHub Projects
- Commits should follow feature-based structure
- Documentation must be updated continuously for PLAR evidence

---

# End of Document