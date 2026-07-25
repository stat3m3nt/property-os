# PropertyOS — Business Requirements Document (BRD)

Document Version: 1.0  
Status: Draft  
Author: Andrew Evboifo  
Last Updated: July 2026  

---

# 1. Introduction

## 1.1 Purpose

This Business Requirements Document (BRD) defines the business needs, capabilities, rules, and constraints for PropertyOS.

The purpose of this document is to establish what the business requires from the system before technical design and implementation begins.

This document serves as the foundation for:

- User stories
- Use cases
- Domain modeling
- Database design
- System architecture
- Software development
- Testing strategy

---

# 2. Product Overview

PropertyOS is a cloud-based property management platform designed to centralize and simplify property operations for landlords, property managers, and property management organizations.

The system enables organizations to manage core property operations including:

- Property portfolios
- Units and occupancy
- Tenant relationships
- Lease agreements
- Rent tracking
- Maintenance workflows
- Vendor coordination
- Documentation
- Operational reporting

PropertyOS aims to provide a unified operating system for managing rental property operations.

---

# 3. Business Objectives (BO)

The following business objectives define the outcomes PropertyOS aims to achieve.

---

## BO-001 — Centralized Property Operations

The system shall provide a centralized platform where property-related information and workflows can be managed in one location.

---

## BO-002 — Reduce Administrative Work

The system shall reduce dependence on manual processes including:

- Spreadsheets
- Paper documentation
- Manual reminders
- Disconnected communication tools

---

## BO-003 — Improve Operational Visibility

The system shall provide users with accurate information about:

- Property performance
- Occupancy
- Rent collection
- Maintenance activity
- Business operations

---

## BO-004 — Improve Communication

The system shall improve communication between:

- Property managers
- Landlords
- Tenants
- Vendors

---

## BO-005 — Support Business Growth

The system shall support organizations as their property portfolios grow.

---

# 4. Stakeholders

## 4.1 Primary Stakeholders

---

## Property Management Companies

Responsibilities:

- Manage multiple properties
- Coordinate tenants
- Manage maintenance operations
- Monitor financial performance

Goals:

- Increase operational efficiency
- Reduce administrative workload
- Improve portfolio visibility

---

## Independent Landlords

Responsibilities:

- Manage owned properties
- Maintain tenant relationships
- Track rental income

Goals:

- Simplify property management
- Reduce reliance on spreadsheets
- Improve organization

---

## Property Managers

Responsibilities:

- Handle daily operations
- Coordinate maintenance
- Communicate with tenants

Goals:

- Complete tasks efficiently
- Access accurate property information

---

# 4.2 Secondary Stakeholders

---

## Tenants

Responsibilities:

- Maintain rental agreements
- Submit maintenance requests
- Communicate with management

---

## Vendors / Contractors

Responsibilities:

- Complete assigned maintenance work
- Provide service updates

---

## System Administrators

Responsibilities:

- Manage system access
- Maintain platform configuration
- Support users

---

# 5. Business Capabilities

This section defines the capabilities required by the business.

---

# 5.1 User and Organization Management

## Business Need

Organizations require secure access management and separation of business data.

---

## Requirements

### BR-ORG-001

The system shall allow organizations to create and manage their own workspace.

---

### BR-ORG-002

The system shall allow multiple users to operate within an organization.

---

### BR-ORG-003

The system shall support different user roles with different permissions.

---

### BR-ORG-004

The system shall maintain separation between organizations.

---

# 5.2 Property Portfolio Management

## Business Need

Property managers require a centralized method for managing properties and rentable spaces.

---

## Requirements

### BR-PROP-001

The system shall allow organizations to maintain property records.

---

### BR-PROP-002

The system shall allow properties to contain multiple units.

---

### BR-PROP-003

The system shall maintain property information including:

- Address
- Property type
- Ownership information
- Status
- Related units

---

### BR-PROP-004

The system shall allow users to track property availability and occupancy.

---

# 5.3 Tenant Relationship Management

## Business Need

Organizations require accurate tenant records and communication history.

---

## Requirements

### BR-TEN-001

The system shall allow organizations to maintain tenant profiles.

---

### BR-TEN-002

The system shall associate tenants with rental agreements.

---

### BR-TEN-003

The system shall maintain tenant history.

---

### BR-TEN-004

The system shall support tenant communication records.

---

# 5.4 Lease Management

## Business Need

Organizations require a reliable method to manage rental agreements.

---

## Requirements

### BR-LEASE-001

The system shall allow users to create and manage lease agreements.

---

### BR-LEASE-002

The system shall associate leases with:

- Properties
- Units
- Tenants

---

### BR-LEASE-003

The system shall track lease lifecycle information.

Including:

- Start date
- End date
- Rental amount
- Status

---

### BR-LEASE-004

The system shall support lease renewals.

---

# 5.5 Maintenance Operations

## Business Need

Organizations require an organized workflow for managing repair requests.

---

## Requirements

### BR-MAINT-001

The system shall allow maintenance issues to be reported.

---

### BR-MAINT-002

The system shall allow maintenance requests to be reviewed and assigned.

---

### BR-MAINT-003

The system shall track maintenance progress.

---

### BR-MAINT-004

The system shall maintain maintenance history.

---

# 5.6 Financial Operations

## Business Need

Organizations require visibility into rental income and payment activity.

---

## Requirements

### BR-FIN-001

The system shall allow users to record rental payments.

---

### BR-FIN-002

The system shall associate payments with leases.

---

### BR-FIN-003

The system shall allow users to track payment status.

---

### BR-FIN-004

The system shall provide financial summaries.

---

# 5.7 Document Management

## Business Need

Organizations require secure storage of property-related documents.

---

## Requirements

### BR-DOC-001

The system shall allow users to store property-related documents.

---

### BR-DOC-002

The system shall associate documents with relevant business records.

Examples:

- Properties
- Tenants
- Leases
- Maintenance requests

---

### BR-DOC-003

The system shall restrict document access based on permissions.

---

# 5.8 Reporting and Analytics

## Business Need

Organizations require insight into property operations.

---

## Requirements

### BR-REPORT-001

The system shall provide operational dashboards.

---

### BR-REPORT-002

The system shall provide information about:

- Occupancy
- Revenue
- Maintenance activity
- Outstanding payments

---

### BR-REPORT-003

The system shall support report generation.

---

# 6. Business Rules

Business rules define restrictions and logic that must always be enforced.

---

# Property Rules

### BRULE-PROP-001

A property must belong to an organization.

---

### BRULE-PROP-002

A property may contain multiple units.

---

# Unit Rules

### BRULE-UNIT-001

A unit must belong to a property.

---

### BRULE-UNIT-002

A unit cannot have multiple active leases simultaneously.

---

# Lease Rules

### BRULE-LEASE-001

A lease must be associated with at least one tenant.

---

### BRULE-LEASE-002

A lease start date must occur before the lease end date.

---

### BRULE-LEASE-003

A lease cannot become active unless the associated unit exists.

---

# Payment Rules

### BRULE-PAY-001

A payment must be associated with an active lease.

---

### BRULE-PAY-002

Payment status must accurately represent payment condition.

Examples:

- Paid
- Pending
- Late
- Failed

---

# Maintenance Rules

### BRULE-MAINT-001

A maintenance request must belong to a property or unit.

---

### BRULE-MAINT-002

Maintenance requests must maintain status history.

---

### BRULE-MAINT-003

Only authorized users may update maintenance status.

---

# 7. Non-Functional Requirements

Non-functional requirements define quality expectations of the system.

---

# Security Requirements

### NFR-SEC-001

The system shall protect sensitive user and business information.

---

### NFR-SEC-002

The system shall enforce access permissions.

---

### NFR-SEC-003

The system shall maintain security activity records.

---

# Performance Requirements

### NFR-PERF-001

The system should provide responsive user interactions.

---

### NFR-PERF-002

The system should support increasing amounts of users and business data.

---

# Scalability Requirements

### NFR-SCALE-001

The system shall support organizations with different portfolio sizes.

---

### NFR-SCALE-002

The system architecture should support future expansion.

---

# Maintainability Requirements

### NFR-MAIN-001

The system shall be organized using modular design principles.

---

### NFR-MAIN-002

The system documentation shall be maintained throughout development.

---

### NFR-MAIN-003

The system should support automated testing.

---

# 8. Business Constraints

The following constraints apply to the initial development phase.

---

## Initial Scope Constraints

The first version will not include:

- AI automation
- Banking integrations
- Tax filing
- Accounting integrations
- IoT integrations
- Utility billing
- Advanced CRM functionality

---

## Operational Constraints

The system must:

- Support modern web environments
- Protect user data
- Support multiple organizations
- Maintain reliable business records

---

# 9. Assumptions

The following assumptions apply:

- Users have internet access.
- Organizations manage their own property information.
- Users have appropriate permissions for accessed information.
- External payment and communication integrations may be added in future releases.

---

# 10. Requirement Traceability

Each business requirement will later map to:

- User Stories
- Use Cases
- Domain Entities
- Database Tables
- API Endpoints
- Automated Tests

Example:

```
BR-MAINT-001

↓

US-MAINT-001

↓

MaintenanceRequest Entity

↓

Maintenance API

↓

Maintenance Tests
```

---

# End of Document
