# ADR-0001: Clean Architecture layering, without a repository layer

- **Status:** Proposed (confirm or edit before marking Accepted)
- **Date:** 2026-10-17
- **Deciders:** Andrew Evboifo
- **Related:** `docs/08-agile-sprints.md` (Sprint 1 deliverable and risk table), `docs/05-system-design.md`

## Context

PropertyOS is a multi-tenant SaaS built by a single developer on ASP.NET Core / .NET 9
with Entity Framework Core and Supabase PostgreSQL. The roadmap calls for Clean
Architecture scaffolding in Sprint 1 and lists "Clean Architecture over-engineering for
a small team" as a risk, with the mitigation of keeping abstractions minimal until a
later sprint actually needs them. The roadmap's original ADR title assumed a repository
pattern, but the code as built does not use one: controllers use `PropertyOsDbContext`
directly, and the Application layer currently holds only `IOrganizationContext` and the
`RequirePermission` attribute.

Tenant isolation is enforced in PostgreSQL row-level security (the API sets
`app.current_org_id` per request inside a transaction), so data access must stay close to
EF Core's transaction and connection handling.

## Options considered

1. **Layered Clean Architecture with EF Core used directly (current state).**
   - Pros: least code; EF's `DbContext` already is a unit of work and repository; keeps
     the per-request transaction used for RLS simple.
   - Cons: query logic can leak into controllers; harder to unit test without a database.
2. **Clean Architecture plus generic repositories.**
   - Pros: conventional separation; easy to fake in unit tests.
   - Cons: a thin wrapper over `DbContext`; extra code and indirection for a solo
     developer; generic repositories tend to hide the tenant and transaction behavior
     that RLS depends on.
3. **Vertical slices / MediatR.**
   - Pros: feature-oriented organization.
   - Cons: explicitly out of scope per the roadmap's risk mitigation for Sprint 1.

## Decision

We will keep the Clean Architecture project layout (`Domain` <- `Application` <-
`Infrastructure`, with `Api` as the composition root and `Contracts` for DTOs) and will
**not** introduce a generic repository layer. `PropertyOsDbContext` remains the data
access boundary. From Sprint 2, business logic moves out of controllers into
Application-layer services that depend on interfaces defined in `Application`.

## Consequences

- **Positive:** minimal ceremony; the RLS transaction and context handling stay in one
  place; fewer moving parts to maintain alone.
- **Negative / trade-offs:** Application services will depend on EF abstractions or on
  small purpose-built interfaces; unit tests rely on integration tests against a real
  PostgreSQL (Testcontainers) rather than fakes for data access.
- **Follow-ups:** add the database-backed test harness (Sprint 2, first task). Revisit
  this ADR if a second data store appears or if query duplication becomes a problem.

## Verification

- Project references enforce the dependency direction (`Domain` has no references).
- Controllers contain no business rules beyond request/response mapping (review item).
- Cross-tenant integration tests run against PostgreSQL with RLS enabled.