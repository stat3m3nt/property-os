# ADR-0002: Select the active organization with a request header, validated against the database

- **Status:** Proposed (confirm or edit before marking Accepted)
- **Date:** 2026-10-17
- **Deciders:** Andrew Evboifo
- **Related:** `docs/07-api-design.md`, `docs/08-agile-sprints.md` (Sprint 2), `supabase/migrations/20261003190840_enable_rls_policies.sql`

## Context

A user can belong to more than one organization, so every tenant-scoped request must say
which organization it acts for, and the API must prove the caller belongs to it. The
original design docs described a Supabase custom access token hook that writes `org_id`,
`role` and `permissions` into the JWT. The implementation instead reads an
`X-Organization-Id` header and checks membership in the database
(`OrganizationContextMiddleware`). Tenant isolation is also enforced in PostgreSQL by
row-level security policies that read `app.current_org_id`, which the middleware sets
inside a per-request transaction.

## Options considered

1. **Organization and permissions as JWT claims (custom access token hook).**
   - Pros: no per-request lookup; the token is self-describing.
   - Cons: a token carries one organization, so switching organizations needs a token
     refresh; claims go stale until the token expires, so role changes and removals are
     not immediate; the token grows with permissions.
2. **`X-Organization-Id` header, membership verified server-side (current).**
   - Pros: switching organizations needs no new token; membership and permissions are
     checked against current data; the header is never trusted without verification.
   - Cons: a database lookup on each request (reducible with a short cache); clients must
     send the header on every tenant-scoped call.

## Decision

We will select the active organization with the `X-Organization-Id` header. The API
accepts it only after confirming the authenticated user has an active, non-deleted
membership in that organization, then sets `app.current_org_id` with a transaction-local
`set_config` so row-level security applies to the same connection. Permissions will be
resolved server-side per (user, organization), not read from token claims. JWT claims are
never taken from `user_metadata`.

## Consequences

- **Positive:** correct multi-organization behavior; revocation takes effect on the next
  request; the database enforces isolation even if application code forgets a filter.
- **Negative / trade-offs:** an extra membership query per request; the OpenAPI document
  and generated client must declare and send the header; the custom access token hook
  becomes optional.
- **Known gaps to close in Sprint 2:** the staging API currently connects as `postgres`,
  which bypasses RLS, so the runtime connection must move to `propertyos_api`; the
  middleware does not yet check user status or organization active/deleted state; the
  policies need `NULLIF(current_setting('app.current_org_id', true), '')::uuid` so an
  empty setting on a reused connection does not raise an error.

## Verification

- Integration tests against PostgreSQL, running as the non-bypass runtime role, show a
  member of organization A cannot read or change organization B's rows.
- A request with an organization ID the user does not belong to returns 403.
- A check in CI or a test confirms the runtime role is neither owner nor `BYPASSRLS`.