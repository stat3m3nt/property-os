# PropertyOS — Use Cases

**Document Version:** 1.0  
**Status:** Approved  
**Author:** Andrew Evboifo  
**Last Updated:** July 2026

---

# 1. Purpose

This document defines the primary business workflows supported by PropertyOS.

Each use case describes how an actor interacts with the system to accomplish a business objective.

The use cases documented here serve as the foundation for:

- User Stories
- User Interface Design
- API Design
- Database Design
- Test Cases
- Sprint Planning

This document is directly traceable to the Business Requirements Document.

---

# 2. Scope

This document covers the core Minimum Viable Product (MVP) workflows required to support day-to-day property management operations.

Future releases may introduce additional use cases for advanced reporting, accounting integrations, AI automation, CRM functionality, and smart building integrations.

---

# 3. Actors

## Primary Actors

| Actor | Description |
|---------|-------------|
| Property Manager | Manages daily operations of one or more properties. |
| Independent Landlord | Manages personally owned rental properties. |
| Tenant | Occupies a rental unit and interacts with management. |
| Vendor | Performs maintenance work. |
| System Administrator | Manages organizations, permissions, and platform administration. |

---

# 4. Use Case Summary

| ID | Use Case | Primary Actor |
|----|-----------|---------------|
| UC-AUTH-001 | Register Account | User |
| UC-AUTH-002 | Login | User |
| UC-ORG-001 | Create Organization | Administrator |
| UC-ORG-002 | Invite User | Administrator |
| UC-PROP-001 | Create Property | Property Manager |
| UC-PROP-002 | Edit Property | Property Manager |
| UC-UNIT-001 | Create Unit | Property Manager |
| UC-TEN-001 | Register Tenant | Property Manager |
| UC-LEASE-001 | Create Lease | Property Manager |
| UC-LEASE-002 | Renew Lease | Property Manager |
| UC-MAINT-001 | Submit Maintenance Request | Tenant |
| UC-MAINT-002 | Assign Maintenance Request | Property Manager |
| UC-MAINT-003 | Complete Maintenance Request | Vendor |
| UC-PAY-001 | Record Rent Payment | Property Manager |
| UC-REPORT-001 | View Dashboard | Property Manager |

---

# 5. Detailed Use Cases

---

# UC-AUTH-001 — Register Account

## Goal

Allow a new user to create an account.

### Primary Actor

User

### Preconditions

- User is not authenticated.

### Trigger

User selects **Create Account**.

### Main Flow

1. User enters registration details.
2. System validates information.
3. System creates account.
4. Verification email is sent.
5. User verifies account.

### Alternative Flows

- Email already exists.
- Password does not meet security requirements.

### Postconditions

- User account exists.
- Account is ready for authentication.

### Related Requirements

- BR-ORG-001

---

# UC-AUTH-002 — Login

## Goal

Allow users to securely access the platform.

### Primary Actor

User

### Preconditions

- User account exists.

### Main Flow

1. User enters credentials.
2. System validates credentials.
3. User is authenticated.
4. Dashboard loads.

### Alternative Flows

- Invalid credentials.
- Locked account.

### Postconditions

- Active authenticated session exists.

---

# UC-ORG-001 — Create Organization

## Goal

Create a new property management organization.

### Primary Actor

Administrator

### Preconditions

- User is authenticated.

### Main Flow

1. User selects Create Organization.
2. Organization details are entered.
3. System validates information.
4. Organization is created.
5. User becomes Organization Administrator.

### Postconditions

- Organization exists.
- Organization workspace is available.

---

# UC-ORG-002 — Invite User

## Goal

Allow administrators to add members.

### Primary Actor

Organization Administrator

### Main Flow

1. Administrator selects Invite User.
2. Email address is entered.
3. Role is selected.
4. Invitation is sent.
5. User accepts invitation.

### Postconditions

- User joins organization.

---

# UC-PROP-001 — Create Property

## Goal

Add a property to an organization's portfolio.

### Primary Actor

Property Manager

### Preconditions

- User belongs to an organization.

### Main Flow

1. Open Property Management.
2. Select Add Property.
3. Enter property information.
4. Submit form.
5. System validates.
6. Property is created.

### Alternative Flows

- Missing required information.
- Duplicate property detected.

### Postconditions

- Property exists.
- Property belongs to organization.

### Related Requirements

- BR-PROP-001
- BRULE-PROP-001

---

# UC-PROP-002 — Edit Property

## Goal

Modify property information.

### Primary Actor

Property Manager

### Main Flow

1. Select property.
2. Edit information.
3. Save changes.
4. System validates.
5. Property is updated.

### Postconditions

- Updated property information is stored.

---

# UC-UNIT-001 — Create Unit

## Goal

Create a rentable unit.

### Primary Actor

Property Manager

### Preconditions

- Property exists.

### Main Flow

1. Select property.
2. Add unit.
3. Enter unit details.
4. Save.
5. Unit becomes available.

### Postconditions

- Unit exists.

---

# UC-TEN-001 — Register Tenant

## Goal

Create a tenant profile.

### Primary Actor

Property Manager

### Main Flow

1. Open Tenant Management.
2. Create tenant.
3. Enter personal information.
4. Save.

### Postconditions

- Tenant profile exists.

---

# UC-LEASE-001 — Create Lease

## Goal

Associate a tenant with a rental unit.

### Primary Actor

Property Manager

### Preconditions

- Tenant exists.
- Unit exists.
- Unit has no active lease.

### Main Flow

1. Select unit.
2. Select tenant.
3. Enter lease information.
4. Save lease.
5. Lease becomes active.

### Alternative Flows

- Unit already leased.
- Invalid lease dates.

### Postconditions

- Active lease exists.

### Related Requirements

- BR-LEASE-001
- BRULE-LEASE-001
- BRULE-LEASE-002

---

# UC-LEASE-002 — Renew Lease

## Goal

Extend an existing lease.

### Primary Actor

Property Manager

### Main Flow

1. Select active lease.
2. Choose Renew.
3. Enter renewal information.
4. Save.

### Postconditions

- Lease renewal recorded.

---

# UC-MAINT-001 — Submit Maintenance Request

## Goal

Allow tenants to report maintenance issues.

### Primary Actor

Tenant

### Preconditions

- Tenant has an active lease.

### Main Flow

1. Login.
2. Select Maintenance.
3. Create request.
4. Enter description.
5. Upload photos (optional).
6. Submit request.
7. System creates maintenance ticket.
8. Property manager is notified.

### Alternative Flows

- Missing required information.
- Duplicate request detected.

### Postconditions

- Maintenance request exists.
- Status = Submitted.

### Related Requirements

- BR-MAINT-001
- BRULE-MAINT-001

---

# UC-MAINT-002 — Assign Maintenance Request

## Goal

Assign maintenance work to a vendor.

### Primary Actor

Property Manager

### Main Flow

1. Review request.
2. Select vendor.
3. Assign work.
4. Vendor notified.

### Postconditions

- Vendor assignment exists.

---

# UC-MAINT-003 — Complete Maintenance Request

## Goal

Close completed maintenance work.

### Primary Actor

Vendor

### Main Flow

1. View assigned work.
2. Update progress.
3. Mark complete.
4. Property manager verifies.
5. Ticket closed.

### Postconditions

- Maintenance request completed.

---

# UC-PAY-001 — Record Rent Payment

## Goal

Record rent received from a tenant.

### Primary Actor

Property Manager

### Preconditions

- Active lease exists.

### Main Flow

1. Open lease.
2. Select Record Payment.
3. Enter payment information.
4. Save.
5. Outstanding balance updated.

### Alternative Flows

- Partial payment.
- Overpayment.

### Postconditions

- Payment recorded.

### Related Requirements

- BR-FIN-001
- BRULE-PAY-001

---

# UC-REPORT-001 — View Dashboard

## Goal

Provide an overview of business operations.

### Primary Actor

Property Manager

### Main Flow

1. Login.
2. Dashboard loads.
3. View key metrics.

Displayed information includes:

- Occupancy
- Revenue
- Outstanding Rent
- Open Maintenance Requests
- Lease Expirations

### Postconditions

- User has current operational insight.

---

# 6. Use Case Relationships

Several use cases depend on the successful completion of earlier workflows.

Example dependency chain:

```
Register Account
        ↓
Create Organization
        ↓
Create Property
        ↓
Create Unit
        ↓
Register Tenant
        ↓
Create Lease
        ↓
Record Rent Payment
        ↓
Submit Maintenance Request
        ↓
Assign Maintenance Request
        ↓
Complete Maintenance Request
```

---

# 7. Traceability

Each use case is traceable to one or more business requirements and will later map to:

- User Stories
- Domain Entities
- Database Tables
- API Endpoints
- Automated Tests

Example:

```
BR-MAINT-001
        ↓
UC-MAINT-001
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

# 8. Assumptions

- Users are authenticated before accessing protected features.
- Organizations manage only their own data.
- Permissions are enforced through role-based access control.
- Future releases may expand existing workflows without replacing them.

---

# End of Document
