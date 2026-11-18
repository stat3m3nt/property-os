using System.Net;
using System.Text.Json;

namespace PropertyOS.IntegrationTests.Database;

internal static class ApiAssert
{
    /// <summary>
    /// Asserts the response is the standard error envelope with the expected
    /// status and code, and that its requestId matches the X-Request-Id header.
    /// </summary>
    internal static async Task<JsonElement> ErrorAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatus,
        string expectedCode)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var error = document.RootElement.GetProperty("error").Clone();

        Assert.Equal(expectedCode, error.GetProperty("code").GetString());
        Assert.Equal(
            response.Headers.GetValues("X-Request-Id").Single(),
            error.GetProperty("requestId").GetString());

        return error;
    }
}
