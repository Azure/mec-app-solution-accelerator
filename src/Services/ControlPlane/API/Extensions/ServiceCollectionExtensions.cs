namespace ControlPlane.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string AzureApiUri = "https://management.azure.com/";

        public static IServiceCollection AddAzureHttpClient<T>(this IServiceCollection services) where T : class
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddHttpClient<T>(client =>
            {
                client.BaseAddress = new Uri(AzureApiUri);
            })
            .ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<ClientApplicationHttpClientHandler>());

            return services;
        }
    }
}