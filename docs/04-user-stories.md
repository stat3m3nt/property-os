# PropertyOS — User Stories

**Document Version:** 1.0  
**Status:** Approved  
**Author:** Andrew Evboifo  
**Last Updated:** July 2026

---

# 1. Purpose

This document defines the functional requirements of PropertyOS from the perspective of its users.

Each user story describes a desired capability that delivers business value to a specific user.

These user stories provide the foundation for:

- Sprint Planning
- Task Breakdown
- GitHub Issues
- Development
- Testing
- Acceptance Validation

Each story is traceable to:

- Business Requirements
- Business Rules
- Use Cases

---

# 2. Story Format

Every story follows this structure.

"As a <persona>,
I want <goal>,
so that <business value>."

Each story also contains acceptance criteria that define when the work is considered complete.

---

# 3. User Stories

---

# Epic: Authentication

---

## US-AUTH-001 — Register Account

**Priority:** High

**Persona**

User

**Story**

As a new user,

I want to create an account,

so that I can securely access PropertyOS.

### Acceptance Criteria

- User enters required information.
- Email address must be unique.
- Password satisfies security requirements.
- Verification email is sent.
- Account is created successfully.

### Related Requirements

- BR-ORG-001

### Related Use Case

- UC-AUTH-001

---

## US-AUTH-002 — Login

**Priority:** High

**Persona**

User

**Story**

As a registered user,

I want to log into PropertyOS,

so that I can access my organization.

### Acceptance Criteria

- Credentials are validated.
- Invalid credentials display an error.
- Successful login redirects to dashboard.
- Session is created securely.

### Related Requirements

- BR-ORG-002

### Related Use Case

- UC-AUTH-002

---

# Epic: Organization Management

---

## US-ORG-001 — Create Organization

**Priority:** High

**Persona**

Administrator

**Story**

As an administrator,

I want to create an organization,

so that I can manage properties within a dedicated workspace.

### Acceptance Criteria

- Organization name is required.
- Organization is created.
- User becomes Organization Administrator.

### Related Requirements

- BR-ORG-001

### Related Use Case

- UC-ORG-001

---

## US-ORG-002 — Invite Team Members

**Priority:** Medium

**Persona**

Organization Administrator

**Story**

As an organization administrator,

I want to invite users,

so that my team can collaborate.

### Acceptance Criteria

- Email invitation is sent.
- User selects a role.
- User joins organization after accepting invitation.

### Related Requirements

- BR-ORG-003

### Related Use Case

- UC-ORG-002

---

# Epic: Property Management

---

## US-PROP-001 — Create Property

**Priority:** High

**Persona**

Property Manager

**Story**

As a property manager,

I want to create a property,

so that I can manage it within PropertyOS.

### Acceptance Criteria

- Property details are required.
- Property belongs to organization.
- Property appears in property list.

### Related Requirements

- BR-PROP-001

### Related Use Case

- UC-PROP-001

---

## US-PROP-002 — Update Property

**Priority:** Medium

**Persona**

Property Manager

**Story**

As a property manager,

I want to update property information,

so that records remain accurate.

### Acceptance Criteria

- Existing information is editable.
- Changes are saved.
- Updated information is displayed.

### Related Requirements

- BR-PROP-002

### Related Use Case

- UC-PROP-002

---

# Epic: Unit Management

---

## US-UNIT-001 — Create Unit

**Priority:** High

**Persona**

Property Manager

**Story**

As a property manager,

I want to create rental units,

so that available spaces can be leased.

### Acceptance Criteria

- Unit belongs to a property.
- Unit number is unique within property.
- Occupancy status defaults to Vacant.

### Related Requirements

- BR-PROP-002

### Related Use Case

- UC-UNIT-001

---

# Epic: Tenant Management

---

## US-TEN-001 — Register Tenant

**Priority:** High

**Persona**

Property Manager

**Story**

As a property manager,

I want to register tenants,

so that leases can be assigned.

### Acceptance Criteria

- Tenant profile is created.
- Contact information is stored.
- Tenant becomes searchable.

### Related Requirements

- BR-TEN-001

### Related Use Case

- UC-TEN-001

---

# Epic: Lease Management

---

## US-LEASE-001 — Create Lease

**Priority:** High

**Persona**

Property Manager

**Story**

As a property manager,

I want to create lease agreements,

so that tenants can legally occupy units.

### Acceptance Criteria

- Unit must exist.
- Tenant must exist.
- Unit cannot already have an active lease.
- Lease dates are validated.
- Lease becomes active.

### Related Requirements

- BR-LEASE-001
- BRULE-LEASE-001
- BRULE-LEASE-002

### Related Use Case

- UC-LEASE-001

---

## US-LEASE-002 — Renew Lease

**Priority:** Medium

**Persona**

Property Manager

**Story**

As a property manager,

I want to renew leases,

so that tenants may continue occupying units.

### Acceptance Criteria

- Active lease selected.
- New lease dates validated.
- Renewal recorded.

### Related Requirements

- BR-LEASE-004

### Related Use Case

- UC-LEASE-002

---

# Epic: Maintenance

---

## US-MAINT-001 — Submit Maintenance Request

**Priority:** High

**Persona**

Tenant

**Story**

As a tenant,

I want to submit maintenance requests,

so that property issues can be resolved quickly.

### Acceptance Criteria

- Description required.
- Photos optional.
- Request linked to unit.
- Status defaults to Submitted.
- Property manager receives notification.

### Related Requirements

- BR-MAINT-001
- BRULE-MAINT-001

### Related Use Case

- UC-MAINT-001

---

## US-MAINT-002 — Assign Maintenance Request

**Priority:** High

**Persona**

Property Manager

**Story**

As a property manager,

I want to assign maintenance requests,

so that vendors can complete repairs.

### Acceptance Criteria

- Vendor selected.
- Vendor notified.
- Request status changes to Assigned.

### Related Requirements

- BR-MAINT-002

### Related Use Case

- UC-MAINT-002

---

## US-MAINT-003 — Complete Maintenance Request

**Priority:** Medium

**Persona**

Vendor

**Story**

As a vendor,

I want to complete maintenance work,

so that the request can be closed.

### Acceptance Criteria

- Work marked complete.
- Completion notes recorded.
- Status changes to Completed.

### Related Requirements

- BRULE-MAINT-003

### Related Use Case

- UC-MAINT-003

---

# Epic: Financial Management

---

## US-PAY-001 — Record Rent Payment

**Priority:** High

**Persona**

Property Manager

**Story**

As a property manager,

I want to record rent payments,

so that tenant balances remain accurate.

### Acceptance Criteria

- Active lease required.
- Payment amount recorded.
- Outstanding balance recalculated.
- Payment history updated.

### Related Requirements

- BR-FIN-001
- BRULE-PAY-001

### Related Use Case

- UC-PAY-001

---

# Epic: Reporting

---

## US-REPORT-001 — View Dashboard

**Priority:** Medium

**Persona**

Property Manager

**Story**

As a property manager,

I want to view operational dashboards,

so that I can monitor portfolio performance.

### Acceptance Criteria

Dashboard displays:

- Occupancy Rate
- Revenue
- Outstanding Rent
- Open Maintenance Requests
- Lease Expirations

### Related Requirements

- BR-REPORT-001

### Related Use Case

- UC-REPORT-001

---

# 4. Story Prioritization

| Priority | Description |
|----------|-------------|
| High | Required for MVP |
| Medium | Important after MVP foundation |
| Low | Future enhancement |

---

# 5. Story Traceability

Every story is linked to:

Business Requirement

↓

Business Rule

↓

Use Case

↓

User Story

↓

Development Task

↓

Test Case

Example

BR-MAINT-001

↓

UC-MAINT-001

↓

US-MAINT-001

↓

Maintenance Feature

↓

Maintenance Tests

---

# End of Document
