
using System.Net.Http.Json;

namespace DevBoard.Api.Tests;

public static class HttpClientExtensions
{
    public static async Task<string> LoginAsAdminAsync(this HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "admin@devboard.test",
            Password = "Password123!"
        });

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        return result!.AccessToken;
    }

    private sealed record LoginResponseDto(string AccessToken, string RefreshToken, DateTime ExpiresAt);
}