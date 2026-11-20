import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

const getSession = vi.fn();

vi.mock("@/lib/supabase/client", () => ({
  createClient: () => ({ auth: { getSession } }),
}));

type FetchMock = ReturnType<typeof vi.fn<(request: Request) => Promise<Response>>>;

function okFetch(): FetchMock {
  return vi.fn(async () => Response.json({ isAuthenticated: true, claims: [] }));
}

async function loadClient() {
  vi.resetModules();
  return (await import("@/lib/api/client")).apiClient;
}

describe("apiClient", () => {
  beforeEach(() => {
    getSession.mockReset();
  });

  afterEach(() => {
    vi.unstubAllEnvs();
  });

  it("sends the Supabase access token as a bearer token when signed in", async () => {
    getSession.mockResolvedValue({ data: { session: { access_token: "token-123" } } });
    const fetchMock = okFetch();
    const client = await loadClient();

    await client.GET("/api/v1/auth/me", { fetch: fetchMock });

    const request = fetchMock.mock.calls[0][0];
    expect(request.headers.get("Authorization")).toBe("Bearer token-123");
  });

  it("sends no Authorization header when there is no session", async () => {
    getSession.mockResolvedValue({ data: { session: null } });
    const fetchMock = okFetch();
    const client = await loadClient();

    await client.GET("/api/v1/auth/me", { fetch: fetchMock });

    const request = fetchMock.mock.calls[0][0];
    expect(request.headers.has("Authorization")).toBe(false);
  });

  it("calls the local API port by default", async () => {
    vi.stubEnv("NEXT_PUBLIC_API_URL", "");
    delete process.env.NEXT_PUBLIC_API_URL;
    getSession.mockResolvedValue({ data: { session: null } });
    const fetchMock = okFetch();
    const client = await loadClient();

    await client.GET("/api/v1/auth/me", { fetch: fetchMock });

    expect(fetchMock.mock.calls[0][0].url).toBe("http://localhost:5252/api/v1/auth/me");
  });

  it("calls the API named by NEXT_PUBLIC_API_URL", async () => {
    vi.stubEnv("NEXT_PUBLIC_API_URL", "https://api.example.test");
    getSession.mockResolvedValue({ data: { session: null } });
    const fetchMock = okFetch();
    const client = await loadClient();

    await client.GET("/api/v1/auth/me", { fetch: fetchMock });

    expect(fetchMock.mock.calls[0][0].url).toBe("https://api.example.test/api/v1/auth/me");
  });
});
