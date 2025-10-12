namespace Movies.Api.Auth;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext context)
    {
        var subject = context.User.Claims.SingleOrDefault(x => x.Type == "sub");

        if (Guid.TryParse(subject?.Value, out var userId)) return userId;

        return null;
    }
}