-- =========================================================================
-- PropertyOS — app_current_org_id() helper and Sprint 1 policy rewrite
-- Migration: add_app_current_org_id
--
-- Problem: policies read current_setting('app.current_org_id', true)::uuid.
-- After a request transaction that used set_config(..., true) ends, a pooled
-- connection reports the setting as an EMPTY STRING, not NULL, so a later
-- query without an org context raises "invalid input syntax for type uuid"
-- instead of returning zero rows. It fails closed, but as a confusing 500.
--
-- Fix: one helper that maps '' to NULL, used by every tenant policy.
--   * NULL makes every "= org id" comparison false, so no rows are visible.
--   * Policies call it as (SELECT public.app_current_org_id()) so Postgres
--     evaluates it once per statement (an InitPlan), not once per row.
--
-- Policy names, commands and semantics are unchanged. The whole file runs in
-- one transaction and is safe to re-run.
-- =========================================================================

BEGIN;

CREATE OR REPLACE FUNCTION public.app_current_org_id()
RETURNS uuid
LANGUAGE sql
STABLE
SET search_path = ''
AS $$
  SELECT NULLIF(pg_catalog.current_setting('app.current_org_id', true), '')::uuid;
$$;

COMMENT ON FUNCTION public.app_current_org_id() IS
  'Active organization id for the current transaction, or NULL when none is set. Set per request by the API via set_config(''app.current_org_id'', <id>, true).';

-- Do not depend on default function privileges: grant explicitly. Guarded so a
-- fresh environment without the role still migrates; bootstrap-api-role.sql
-- grants it there.
DO $$
BEGIN
  IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'propertyos_api') THEN
    GRANT EXECUTE ON FUNCTION public.app_current_org_id() TO propertyos_api;
  END IF;
END $$;

-- -------------------------------------------------------------------------
-- organizations — the tenant root, scoped by its own "Id".
-- -------------------------------------------------------------------------
DROP POLICY IF EXISTS org_isolation_select ON public.organizations;
CREATE POLICY org_isolation_select ON public.organizations FOR SELECT
  USING ("Id" = (SELECT public.app_current_org_id()));

DROP POLICY IF EXISTS org_isolation_update ON public.organizations;
CREATE POLICY org_isolation_update ON public.organizations FOR UPDATE
  USING ("Id" = (SELECT public.app_current_org_id()))
  WITH CHECK ("Id" = (SELECT public.app_current_org_id()));

-- -------------------------------------------------------------------------
-- roles — NULL "OrganizationId" = system role, readable by everyone.
-- -------------------------------------------------------------------------
DROP POLICY IF EXISTS org_isolation_select ON public.roles;
CREATE POLICY org_isolation_select ON public.roles FOR SELECT
  USING (
    "OrganizationId" = (SELECT public.app_current_org_id())
    OR "OrganizationId" IS NULL
  );

DROP POLICY IF EXISTS org_isolation_write ON public.roles;
CREATE POLICY org_isolation_write ON public.roles FOR INSERT
  WITH CHECK ("OrganizationId" = (SELECT public.app_current_org_id()));

DROP POLICY IF EXISTS org_isolation_update ON public.roles;
CREATE POLICY org_isolation_update ON public.roles FOR UPDATE
  USING ("OrganizationId" = (SELECT public.app_current_org_id()))
  WITH CHECK ("OrganizationId" = (SELECT public.app_current_org_id()));

DROP POLICY IF EXISTS org_isolation_delete ON public.roles;
CREATE POLICY org_isolation_delete ON public.roles FOR DELETE
  USING ("OrganizationId" = (SELECT public.app_current_org_id()));

-- -------------------------------------------------------------------------
-- role_permissions — scoped through a join to roles. No UPDATE policy by design.
-- -------------------------------------------------------------------------
DROP POLICY IF EXISTS org_isolation_select ON public.role_permissions;
CREATE POLICY org_isolation_select ON public.role_permissions FOR SELECT
  USING (
    EXISTS (
      SELECT 1 FROM public.roles r
      WHERE r."Id" = role_permissions."RoleId"
        AND (r."OrganizationId" = (SELECT public.app_current_org_id())
             OR r."OrganizationId" IS NULL)
    )
  );

DROP POLICY IF EXISTS org_isolation_write ON public.role_permissions;
CREATE POLICY org_isolation_write ON public.role_permissions FOR INSERT
  WITH CHECK (
    EXISTS (
      SELECT 1 FROM public.roles r
      WHERE r."Id" = role_permissions."RoleId"
        AND r."OrganizationId" = (SELECT public.app_current_org_id())
    )
  );

DROP POLICY IF EXISTS org_isolation_delete ON public.role_permissions;
CREATE POLICY org_isolation_delete ON public.role_permissions FOR DELETE
  USING (
    EXISTS (
      SELECT 1 FROM public.roles r
      WHERE r."Id" = role_permissions."RoleId"
        AND r."OrganizationId" = (SELECT public.app_current_org_id())
    )
  );

-- -------------------------------------------------------------------------
-- user_organization_roles — direct "OrganizationId". No UPDATE policy by design.
-- -------------------------------------------------------------------------
DROP POLICY IF EXISTS org_isolation_select ON public.user_organization_roles;
CREATE POLICY org_isolation_select ON public.user_organization_roles FOR SELECT
  USING ("OrganizationId" = (SELECT public.app_current_org_id()));

DROP POLICY IF EXISTS org_isolation_write ON public.user_organization_roles;
CREATE POLICY org_isolation_write ON public.user_organization_roles FOR INSERT
  WITH CHECK ("OrganizationId" = (SELECT public.app_current_org_id()));

DROP POLICY IF EXISTS org_isolation_delete ON public.user_organization_roles;
CREATE POLICY org_isolation_delete ON public.user_organization_roles FOR DELETE
  USING ("OrganizationId" = (SELECT public.app_current_org_id()));

-- -------------------------------------------------------------------------
-- users — visible when the user has a membership in the active organization.
-- -------------------------------------------------------------------------
DROP POLICY IF EXISTS org_isolation_select ON public.users;
CREATE POLICY org_isolation_select ON public.users FOR SELECT
  USING (
    EXISTS (
      SELECT 1 FROM public.user_organization_roles uor
      WHERE uor."UserId" = users."Id"
        AND uor."OrganizationId" = (SELECT public.app_current_org_id())
    )
  );

-- permissions keeps its catalog_read_only policy (USING (true)); it does not
-- read the organization setting, so nothing changes there.

COMMIT;
