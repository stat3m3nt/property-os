# PropertyOS Mobile

Expo (React Native) client for [PropertyOS](../../README.md), a multi-tenant
property and lease management platform.

**Status: scaffold.** The Supabase client and session storage are configured.
Feature screens are planned for later sprints (see
[`docs/08-agile-sprints.md`](../../docs/08-agile-sprints.md)). The "Explore"
tab is Expo starter content and will be replaced.

## Stack

- Expo SDK with Expo Router (file-based routing under `src/app`)
- TypeScript
- Supabase JS client for authentication

## Run locally

1. Create `apps/mobile/.env.local` with your Supabase project values:

   ```
   EXPO_PUBLIC_SUPABASE_URL=https://<project-ref>.supabase.co
   EXPO_PUBLIC_SUPABASE_ANON_KEY=<anon key>
   ```

   Use only the public `anon` key here. Never put the `service_role` key or a
   database connection string in a mobile app.

2. Install and start:

   ```bash
   npm install
   npx expo start
   ```

   Then open the app in an Android emulator, an iOS simulator, or Expo Go.

See the [root README](../../README.md) for backend and database setup.

## Known issues

- `npx expo export --platform web` currently fails during static rendering with
  `localStorage is not defined`. Native (Android/iOS) exports are unaffected.