using System.Text.Json.Serialization;

namespace Movies.Api.Auth;


public class KeycloakAccessToken
{
    [JsonPropertyName("exp")]
    public long Exp { get; set; }

    [JsonPropertyName("iat")]
    public long Iat { get; set; }

    [JsonPropertyName("iss")]
    public string Iss { get; set; }

    [JsonPropertyName("sub")]
    public string Sub { get; set; }

    [JsonPropertyName("typ")]
    public string Typ { get; set; }

    [JsonPropertyName("azp")]
    public string Azp { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("preferred_username")]
    public string PreferredUsername { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }

    [JsonPropertyName("resource_access")]
    public Dictionary<string, ResourceAccess> ResourceAccess { get; set; }
}

public class ResourceAccess
{
    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = [];
}