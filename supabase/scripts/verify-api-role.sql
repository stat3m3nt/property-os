-- =========================================================================
-- Verify the propertyos_api runtime role (read-only).
-- Run in the Supabase SQL editor as postgres, after bootstrap-api-role.sql.
-- Every row must show passed = true.
-- Keep `expected` in sync with bootstrap-api-role.sql.
-- =========================================================================
with expected(table_name, privilege) as (
  values
    ('organizations',            'SELECT'),
    ('organizations',            'UPDATE'),
    ('permissions',              'SELECT'),
    ('roles',                    'SELECT'),
    ('roles',                    'INSERT'),
    ('roles',                    'UPDATE'),
    ('roles',                    'DELETE'),
    ('role_permissions',         'SELECT'),
    ('role_permissions',         'INSERT'),
    ('role_permissions',         'DELETE'),
    ('user_organization_roles',  'SELECT'),
    ('user_organization_roles',  'INSERT'),
    ('user_organization_roles',  'DELETE'),
    ('users',                    'SELECT')
),
actual as (
  -- Effective privileges (includes PUBLIC and inherited grants)
  select c.relname::text as table_name, p.priv as privilege
  from pg_class c
  join pg_namespace n on n.oid = c.relnamespace and n.nspname = 'public'
  cross join (values ('SELECT'), ('INSERT'), ('UPDATE'), ('DELETE'),
                     ('TRUNCATE'), ('REFERENCES'), ('TRIGGER')) as p(priv)
  where c.relkind in ('r', 'p')
    and has_table_privilege('propertyos_api', c.oid, p.priv)
)
select 'role is LOGIN, NOSUPERUSER, NOBYPASSRLS' as check_name,
       exists (select 1 from pg_roles
               where rolname = 'propertyos_api'
                 and rolcanlogin and not rolsuper and not rolbypassrls) as passed
union all
select 'role owns no tables in public',
       not exists (select 1 from pg_class c
                   join pg_namespace n on n.oid = c.relnamespace
                   where n.nspname = 'public'
                     and pg_get_userbyid(c.relowner) = 'propertyos_api')
union all
select 'RLS enabled on every table in public',
       not exists (select 1 from pg_class c
                   join pg_namespace n on n.oid = c.relnamespace
                   where n.nspname = 'public'
                     and c.relkind in ('r', 'p')
                     and not c.relrowsecurity)
union all
select 'role cannot CREATE in schema public',
       not has_schema_privilege('propertyos_api', 'public', 'CREATE')
union all
select 'no missing grants',
       not exists (select table_name, privilege from expected
                   except
                   select table_name, privilege from actual)
union all
select 'no extra grants (incl. __EFMigrationsHistory)',
       not exists (select table_name, privilege from actual
                   except
                   select table_name, privilege from expected)
union all
select 'no default privileges granted to the role',
       not exists (select 1 from pg_default_acl
                   where defaclacl::text like '%propertyos_api%');