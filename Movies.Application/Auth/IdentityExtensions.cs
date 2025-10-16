using Microsoft.AspNetCore.Http;

namespace Movies.Application.Auth;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        var subject = context.User.Claims.SingleOrDefault(x => x.Type == "sub");

        return Guid.TryParse(subject?.Value, out var userId) ? userId : null;
    }
}