public class ApiKeyMiddleware
{
    public const string APIKEY_HEADER_NAME = "api-key";

    private readonly RequestDelegate _next;
    private const string APIKEY_CONFIG_NAME = "APIKey";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        var appSettingsApiKey = configuration.GetValue<string>(APIKEY_CONFIG_NAME);
        // If no api-key configured assuming it's a public API.
        if (string.IsNullOrEmpty(appSettingsApiKey))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(APIKEY_HEADER_NAME, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key was not provided. Access denied.");
            return;
        }

        if (!appSettingsApiKey.Equals(extractedApiKey, StringComparison.InvariantCultureIgnoreCase))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }

        await _next(context);
    }
}