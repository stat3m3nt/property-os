"use client";

import { useEffect, useState } from "react";
import { apiClient } from "@/lib/api/client";
import type { paths } from "@/lib/api/generated/schema";

type ApiResponse =
  paths["/api/v1/auth/me"]["get"]["responses"][200]["content"]["application/json"];

export default function Home() {
  const [user, setUser] = useState<ApiResponse | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function testApiAuthentication() {
      // Checked via response.ok rather than the `error` field: this
      // endpoint's 401 response has no body, which makes openapi-fetch
      // infer `error` as `never` — checking a never-typed value makes
      // TypeScript treat the branch as unreachable. response.ok sidesteps
      // that entirely and is the more robust check for a bodyless error
      // response anyway.
      const { data, response } = await apiClient.GET("/api/v1/auth/me");

      if (!response.ok || !data) {
        setError(`API request failed: ${response.status} ${response.statusText}`);
        setLoading(false);
        return;
      }

      setUser(data);
      setLoading(false);
    }

    testApiAuthentication();
  }, []);

  return (
    <main className="flex min-h-screen items-center justify-center px-6">
      <div className="w-full max-w-xl space-y-6">
        <div>
          <h1 className="text-3xl font-semibold">PropertyOS</h1>
          <p className="mt-2 text-zinc-600">
            API authentication test
          </p>
        </div>

        {loading && (
          <p className="text-sm text-zinc-600">
            Testing authenticated API request...
          </p>
        )}

        {error && (
          <div className="rounded-md border border-red-200 bg-red-50 p-4">
            <p className="font-medium text-red-800">
              API authentication failed
            </p>
            <p className="mt-1 text-sm text-red-700">{error}</p>
          </div>
        )}

        {user && (
          <div className="rounded-md border p-4">
            <h2 className="font-medium">API Response</h2>

            <dl className="mt-4 space-y-2 text-sm">
              <div>
                <dt className="font-medium">Authenticated</dt>
                <dd>{String(user.isAuthenticated)}</dd>
              </div>

              <div>
                <dt className="font-medium">Claims</dt>
                <dd className="mt-2 space-y-1">
                  {user.claims.map((claim, index) => (
                    <div key={index} className="rounded bg-zinc-100 p-2">
                      <div className="font-medium">{claim.type}:</div>
                      <div className="break-all text-zinc-600">{claim.value}</div>
                    </div>
                  ))}
                </dd>
              </div>
               
            </dl>
          </div>
        )}
      </div>
    </main>
  );
}