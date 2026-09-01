# PropertyOS

Multi-tenant Property & Lease Management SaaS. See `docs/00-vision.md` for the product overview and `docs/08-agile-sprints.md` for the delivery roadmap this repository is being built against.

**Current status:** Sprint 1 (Platform Foundation) is nearing completion. See the engineering audit for the authoritative current state before assuming any feature beyond authentication and health checks exists.

## Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org) (Next.js 16 requires Node 20.9+)
- A [Supabase](https://supabase.com) project (free tier is sufficient for local development)
- PostgreSQL — provided by Supabase; no local Postgres install is required

## 1. Supabase project setup

1. Create a Supabase project.
2. From **Project Settings → API**, note:
   - Project URL (`NEXT_PUBLIC_SUPABASE_URL`)
   - `anon` public key (`NEXT_PUBLIC_SUPABASE_ANON_KEY`)
3. From **Project Settings → API → JWT Settings**, note the issuer URL — it follows the pattern `https://<project-ref>.supabase.co/auth/v1` and is already set as the default in `src/PropertyOS.Api/appsettings.json` under `Supabase:JwtIssuer`. Update it if your project ref differs.
4. From **Project Settings → Database**, copy the connection string (use the "Session pooler" or direct connection string depending on your setup).

## 2. Backend — `src/PropertyOS.Api`

```bash
# From the repository root
dotnet restore PropertyOS.sln

# Configure your local connection string (do not commit this)
cd src/PropertyOS.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Database=...;Username=...;Password=..."

# Apply migrations
dotnet ef database update --project ../PropertyOS.Infrastructure --startup-project .

# Run the API
dotnet run
```

The API listens on `http://localhost:5252` (see `Properties/launchSettings.json`). Verify it's up:

```bash
curl http://localhost:5252/api/v1/health
```

### Running tests

```bash
dotnet test PropertyOS.sln
```

## 3. Frontend — `apps/web`

```bash
cd apps/web
npm install
```

Create `apps/web/.env.local`:

```
NEXT_PUBLIC_SUPABASE_URL=https://<project-ref>.supabase.co
NEXT_PUBLIC_SUPABASE_ANON_KEY=<your-anon-key>
```

```bash
npm run dev
```

The app runs on `http://localhost:3000` and expects the API at `http://localhost:5252` (hardcoded in `src/lib/api/client.ts` for now — see the engineering audit for the follow-up to make this configurable).

Create a user under **Authentication → Users** in the Supabase dashboard, then sign in at `http://localhost:3000/login`.

## Solution structure

```
src/
  PropertyOS.Api/             ASP.NET Core Web API — controllers, middleware, composition root
  PropertyOS.Application/     Application layer — service interfaces, cross-cutting abstractions
  PropertyOS.Domain/          Domain entities, enums — zero external dependencies
  PropertyOS.Infrastructure/  EF Core DbContext, entity configurations, migrations
  PropertyOS.Contracts/       Request/response DTOs shared between Api and Application
tests/
  PropertyOS.UnitTests/
  PropertyOS.IntegrationTests/
apps/
  web/                        Next.js 16 (App Router) frontend
docs/                         Vision, requirements, system design, API design, sprint roadmap
```

Dependency direction (Clean Architecture): `Domain` ← `Application` ← `Infrastructure`, with `Api` depending on `Application`, `Infrastructure`, and `Contracts`. `Domain` has no project references.

## CI

Every PR runs `.github/workflows/ci.yml`: `dotnet build`/`dotnet test` for the backend, `npm run lint`/`npm run build` for the frontend. Both must be green before merge.
