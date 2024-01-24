
using System.Net.Http.Headers;
using Microsoft.Identity.Client;

public class ClientApplicationHttpClientHandler : HttpClientHandler
{
    private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
    private static int MINUTES_BEFORE_REFRESH = 5;
    private readonly IConfidentialClientApplication _clientApp;
    private readonly string[] _scopes;
    private AuthenticationResult _cachedTokenResult;

    public ClientApplicationHttpClientHandler(IConfidentialClientApplication clientApp, string[] scopes)
    {
        _clientApp = clientApp;
        _scopes = scopes;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tokenResult = await GetOrRefreshTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.AccessToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<AuthenticationResult> GetOrRefreshTokenAsync()
    {
        await semaphoreSlim.WaitAsync();
        try
        {
            if (_cachedTokenResult == null || IsTokenNearExpiration(_cachedTokenResult.ExpiresOn))
                _cachedTokenResult = await _clientApp.AcquireTokenForClient(_scopes)
                    .ExecuteAsync();
        }
        finally
        {
            semaphoreSlim.Release();
        }

        return _cachedTokenResult;
    }

    private static bool IsTokenNearExpiration(DateTimeOffset expirationTime)
    {
        return (expirationTime - DateTimeOffset.UtcNow).TotalMinutes <= MINUTES_BEFORE_REFRESH;
    }
}