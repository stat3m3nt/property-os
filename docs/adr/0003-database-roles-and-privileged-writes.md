# ADR-0003: Runtime database role, migration identity, and privileged writes through SECURITY DEFINER functions

- **Status:** Proposed (confirm or edit before marking Accepted)
- **Date:** 2026-11-13
- **Deciders:** Andrew Evboifo
- **Related:** `docs/07-api-design.md` (section 8, least privilege for the service-role key), `docs/08-agile-sprints.md` (Sprint 2), ADR-0002, `supabase/scripts/bootstrap-api-role.sql`, `supabase/scripts/verify-api-role.sql`, `supabase/migrations/20261003190840_enable_rls_policies.sql`, `supabase/migrations/20261016120000_revoke_default_client_privileges.sql`

## Context

Row-level security only protects data if the API connects as a role that RLS applies to.
Until 2026-11-13 the staging API connected as `postgres`, which bypasses RLS. It now
connects as `propertyos_api`: `LOGIN`, `NOSUPERUSER`, `NOBYPASSRLS`, owner of no tables,
no `CREATE` on schema `public`, and only the table privileges listed in the bootstrap
script. Tables are owned by `postgres`. No default privileges grant anything to
`propertyos_api`, so every new table needs an explicit grant.

Sprint 2 needs writes that happen before any organization context exists: creating an
organization at signup, creating the `Users` row for a new Supabase Auth user, and
creating a pending user for an invitation. The Sprint 1 policies deliberately allow none
of these (`organizations` and `users` have no INSERT policy, and `propertyos_api` has no
INSERT grant on them). `docs/07-api-design.md` section 8 also says the Supabase
service-role key, which bypasses RLS, must not be reachable from request-scoped
controller code. The docs do not say how pre-tenant writes should work.

EF Core migrations are applied manually as `postgres`; the API never runs DDL
(the Docker image does not run migrations at startup).

## Options considered

1. **Run the API, or a second connection, as `postgres` or with the service-role key.**
   - Pros: no new database objects; every write just works.
   - Cons: bypasses RLS for the code path that handles the most sensitive writes;
     directly contradicts section 8; one injection bug exposes every tenant.
2. **A second narrow database role and connection (for example `propertyos_provisioner`).**
   - Pros: no logic in SQL; privileges are explicit.
   - Cons: a second credential to store, rotate and pool; a second `DbContext` or
     connection path; the role still needs broad INSERTs across several tables.
3. **Permissive RLS policies for "no organization context" inserts.**
   - Pros: logic stays in C#; no functions.
   - Cons: puts a "no context is allowed" exception into tenant tables, which is where
     fail-open bugs and the empty-string setting problem live; multi-table inserts
     need chained policies that are hard to reason about and to test.
4. **A small allow-list of `SECURITY DEFINER` functions, callable only by `propertyos_api`.**
   - Pros: each privileged operation is one atomic, reviewable, testable unit; tenant
     tables keep strict policies with no exceptions; no extra credential; consistent
     with section 8 (no bypass credential in request paths); Supabase's own access token
     hook is already a privileged Postgres function, so the pattern fits.
   - Cons: some business logic lives in SQL; function bodies are security-critical and
     need care (search path, grants, input handling).

## Decision

We will use option 4.

- The runtime API connects only as `propertyos_api`. Migrations run as `postgres` from a
  developer machine or a CI deploy step, never through the API connection.
- Tenant context stays as decided in ADR-0002: a transaction-local `set_config`. A helper
  `app_current_org_id()` returning `NULLIF(current_setting('app.current_org_id', true), '')::uuid`
  will be used by every policy, delivered as its own migration.
- Writes that cannot happen inside an organization context go through `SECURITY DEFINER`
  functions owned by `postgres`. The initial allow-list is `app_create_organization`
  (organization, its settings, the creator's `Users` row if missing, and the Owner
  membership, in one transaction). Adding another function requires a PR that updates this
  list, the grants, and the tests; invitations and session or login-attempt writes will be
  assessed individually when their stories start.
- Every such function: pins `SET search_path = ''` and schema-qualifies every object; uses
  no dynamic SQL; takes the acting auth user id as a parameter that the API fills from the
  verified JWT `sub`, never from a request body; has `EXECUTE` revoked from `PUBLIC`,
  `anon` and `authenticated` and granted only to `propertyos_api`.
- Table grants stay explicit per migration, and `verify-api-role.sql` is updated in the
  same PR as any new grant.
- Request-scoped code never uses the service-role key. How the Supabase invite-link API,
  which needs that key, is called will be decided separately.

## Consequences

- **Positive:** the API credential cannot bypass RLS; privileged writes are few, named and
  tested; tenant policies stay free of "no context" exceptions; no second credential.
- **Negative / trade-offs:** signup logic is split between C# and SQL; functions owned by
  `postgres` bypass RLS on the tables they touch (RLS is not forced), so their bodies must
  be reviewed as security code; EF cannot map these functions, so they are called with raw
  SQL from Infrastructure behind an Application-layer interface.
- **Follow-ups:** migration adding `app_current_org_id()` and rewriting the Sprint 1
  policies; `app_create_organization` and its tests in Sprint 2; the Supabase
  invite-key question; revisit if the allow-list grows past a handful of functions or a
  function needs logic that is awkward in SQL.

## Verification

- `supabase/scripts/verify-api-role.sql` passes on every environment (role attributes,
  ownership, RLS enabled, no `CREATE` on `public`, exact grants, no default privileges).
- Integration tests on PostgreSQL, running as `propertyos_api`, show that: a direct INSERT
  into `organizations` or `users` is denied; each allow-listed function works for its
  intended case; calling a function as `anon` or `authenticated` is denied.
- A query or test lists every `SECURITY DEFINER` function in `public` and fails if one is
  missing from the allow-list, lacks a pinned `search_path`, or is executable by `PUBLIC`.
- Review checklist for any migration: explicit grants, a verify-script entry, an isolation
  test.