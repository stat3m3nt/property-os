-- Keep direct browser/mobile clients from accessing application tables.
-- Application data access is handled by the ASP.NET Core API.
-- The propertyos_api role retains its existing grants.

REVOKE ALL PRIVILEGES ON TABLE
    public.organizations,
    public.permissions,
    public.role_permissions,
    public.roles,
    public.user_organization_roles,
    public.users
FROM anon, authenticated;
