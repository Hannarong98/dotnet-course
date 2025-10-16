using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Movies.Api.Auth;

namespace Movies.Application.Auth;

public class KeycloakRolesClaimsTransformation(IConfiguration config) : IClaimsTransformation
{
    private readonly string _clientId =
        config["Jwt:ClientId"] ?? throw new InvalidOperationException("ClientId is not defined");


    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity)principal.Identity!;
        var resourceAccessClaim = identity.FindFirst("resource_access");

        if (resourceAccessClaim != null)
        {
            var resourceAccess =
                JsonSerializer.Deserialize<Dictionary<string, ResourceAccess>>(resourceAccessClaim.Value);
            if (resourceAccess != null &&
                resourceAccess.TryGetValue(_clientId, out var clientAccess))
                foreach (var role in clientAccess.Roles)
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        return Task.FromResult(principal);
    }
}