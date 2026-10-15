-- Secure EF Core migration metadata from direct browser/mobile access.
-- Run after public."__EFMigrationsHistory" has been created.
-- Safe to run repeatedly.

DO $$
BEGIN
    IF to_regclass('public."__EFMigrationsHistory"') IS NOT NULL THEN
        EXECUTE 'ALTER TABLE public."__EFMigrationsHistory" ENABLE ROW LEVEL SECURITY';

        EXECUTE 'REVOKE ALL PRIVILEGES ON TABLE public."__EFMigrationsHistory" FROM anon, authenticated';
    END IF;
END
$$;
