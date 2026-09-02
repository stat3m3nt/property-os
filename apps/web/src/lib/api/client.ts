import createClient, { type Middleware } from "openapi-fetch";
import type { paths } from "@/lib/api/generated/schema";
import { createClient as createSupabaseClient } from "@/lib/supabase/client";

// Falls back to the local .NET dev port (see
// src/PropertyOS.Api/Properties/launchSettings.json) so `npm run dev` works
// out of the box; set NEXT_PUBLIC_API_URL to point elsewhere (staging,
// a deployed preview, etc.) without touching this file.
const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5252";

const authMiddleware: Middleware = {
  async onRequest({ request }) {
    const supabase = createSupabaseClient();
    const {
      data: { session },
    } = await supabase.auth.getSession();

    if (session?.access_token) {
      request.headers.set("Authorization", `Bearer ${session.access_token}`);
    }

    return request;
  },
};

export const apiClient = createClient<paths>({ baseUrl: API_URL });
apiClient.use(authMiddleware);
