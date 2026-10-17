DO $$ BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'propertyos_api') THEN
    CREATE ROLE propertyos_api LOGIN NOSUPERUSER NOBYPASSRLS NOINHERIT;
  END IF;
END $$;

GRANT USAGE ON SCHEMA public TO propertyos_api;

-- Runtime privileges (least privilege; extend per table in Sprint 2 migrations)
GRANT SELECT, UPDATE                 ON public.organizations            TO propertyos_api;
GRANT SELECT                         ON public.permissions              TO propertyos_api;
GRANT SELECT, INSERT, UPDATE, DELETE ON public.roles                    TO propertyos_api;
GRANT SELECT, INSERT, DELETE         ON public.role_permissions         TO propertyos_api;
GRANT SELECT, INSERT, DELETE         ON public.user_organization_roles  TO propertyos_api;
GRANT SELECT                         ON public.users                    TO propertyos_api;
REVOKE ALL ON public."__EFMigrationsHistory" FROM propertyos_api;