using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace Movies.Api.Sdk.Consumer;

class TokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
} 
public class TokenProvider(HttpClient client, IConfiguration configuration)
{
    private string _cachedToken = string.Empty;
    private static readonly SemaphoreSlim Lock = new(1, 1);

    public async Task<string> AcquireTokenAsync()
    {
        if (IsTokenValid(_cachedToken))
            return _cachedToken;

        await Lock.WaitAsync();
        try
        {
            if (IsTokenValid(_cachedToken))
                return _cachedToken;

            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", configuration["Consumer:ClientId"]!),
                new KeyValuePair<string, string>("client_secret", configuration["Consumer:ClientSecret"]!)
            });

            var response = await client.PostAsync(configuration["MoviesAPI:TokenURL"]!, tokenRequest);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (token != null)
                _cachedToken = token.AccessToken;

            return _cachedToken;
        }
        finally
        {
            Lock.Release();
        }
    }

    private static bool IsTokenValid(string token)
    {
        if (string.IsNullOrEmpty(token)) return false;

        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var expClaim = jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (expClaim == null || !long.TryParse(expClaim, out var exp))
                return false;

            return UnixToDateTime(exp) > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }

    private static DateTime UnixToDateTime(long unixTimestamp) =>
        DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).UtcDateTime;
}
