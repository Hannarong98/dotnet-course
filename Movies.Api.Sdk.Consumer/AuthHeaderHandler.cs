using System.Net.Http.Headers;

namespace Movies.Api.Sdk.Consumer;

public class AuthHeaderHandler(TokenProvider provider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await provider.AcquireTokenAsync();
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}