# PropertyOS — Requirements Document

## 1. Overview

PropertyOS is a full-stack property management system designed to streamline communication, maintenance tracking, rent management, and property administration between landlords and tenants.

The system is designed as a multi-platform application consisting of:
- Web application (landlord/admin dashboard)
- Mobile application (tenant experience)
- Backend API (business logic layer)
- Cloud-hosted database and storage layer

The system will serve as:
- A Minimum Viable Product (MVP)
- A portfolio project demonstrating full-stack engineering capability
- A PLAR evidence project mapping to academic software development outcomes

---

## 2. Problem Statement

Property management is often fragmented across spreadsheets, emails, messaging apps, and manual tracking systems. This results in:

- Delayed maintenance resolution
- Lack of transparency between landlords and tenants
- Inefficient rent tracking and payment status monitoring
- Poor document organization (leases, receipts, agreements)
- Limited real-time communication and accountability

PropertyOS aims to centralize these workflows into a single digital system.

---

## 3. Project Objectives

The primary objectives of PropertyOS are:

- Provide a centralized platform for property management
- Enable tenants to submit and track maintenance requests
- Allow landlords to manage properties, units, and tenants efficiently
- Automate rent tracking and overdue detection
- Ensure secure role-based access control
- Support both web and mobile access

---

## 4. System Users

### 4.1 Tenant
- Submits maintenance requests
- Views rent status
- Receives updates on requests
- Uploads supporting images/documents

### 4.2 Landlord
- Manages properties and units
- Reviews maintenance requests
- Tracks rent payments
- Manages tenant assignments

### 4.3 Admin (System-level role)
- Oversees platform operations
- Has full access for monitoring and support purposes

---

## 5. Functional Requirements

### 5.1 Authentication & Authorization
- Users must be able to register and log in securely
- Role-based access control must be enforced (tenant, landlord, admin)
- Users can only access data relevant to their role

---

### 5.2 Property Management
- Landlords can create, update, and delete properties
- Each property can contain multiple units
- Units must be assigned to tenants

---

### 5.3 Maintenance System
- Tenants can create maintenance tickets
- Each ticket must include:
  - Title
  - Description
  - Priority (low, medium, high, urgent)
  - Status (reported, in_progress, resolved)
  - Optional file attachments (images/videos)
- Landlords can update ticket status
- System must track ticket history

---

### 5.4 Rent Management System
- Each unit has a defined monthly rent value
- System tracks rent payments per month
- Payment statuses include:
  - paid
  - unpaid
  - overdue
- System must support overdue detection logic (future backend extension)

---

### 5.5 Document Management
- Users can upload and store documents
- Documents must be linked to:
  - property
  - unit
  - or tenancy agreement
- Supported file types include PDF, images, and text documents

---

### 5.6 Dashboard & Reporting
- Landlords must have a dashboard showing:
  - active properties
  - open maintenance tickets
  - rent status summary
- System should present aggregated data clearly

---

## 6. Non-Functional Requirements

### 6.1 Performance
- System should support fast load times (<2 seconds for core pages under normal load)
- Database queries must be optimized with indexing where necessary

---

### 6.2 Security
- All sensitive data must be protected using authentication
- Row Level Security (RLS) must be enforced at the database level
- Passwords must not be stored in plain text (handled by auth provider)

---

### 6.3 Scalability
- System must support multiple properties per landlord
- System must support multiple tenants per property over time
- Architecture should allow future expansion (analytics, AI, automation)

---

### 6.4 Reliability
- System should handle failed requests gracefully
- Data integrity must be preserved using relational constraints

---

### 6.5 Usability
- Interfaces must be responsive (desktop + mobile)
- System must provide clear navigation for both tenants and landlords

---

## 7. Technical Constraints

The system must be built using the following technologies:

### Frontend
- Next.js (React)
- TypeScript
- Tailwind CSS

### Mobile
- React Native (Expo)
- TypeScript

### Backend
- ASP.NET Core Web API (C#)

### Database & Storage
- Supabase (PostgreSQL)
- Supabase Auth
- Supabase Storage

---

## 8. Assumptions

- Users have access to internet-enabled devices
- Landlords may manage multiple properties
- Tenants are assigned to a single unit at a time
- Payments are tracked but not processed directly by the system (future Stripe integration)

---

## 9. Future Enhancements

The following features are outside MVP scope but planned for expansion:

- Automated rent payment integration (Stripe)
- AI-based maintenance categorization
- Push notifications for tenants
- Analytics dashboard for property performance
- Audit logging system for administrative tracking

---

## 10. Summary

This document defines the functional and non-functional requirements for the PropertyOS system. It serves as the foundation for system architecture design, database modeling, and implementation planning.