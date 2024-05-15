using ControlPlane.API.Models;
using ControlPlane.API.Settings;
using Microsoft.Extensions.Options;

public class AttachedDataNetworkService
{
    private readonly HttpClient httpClient;
    private readonly MobileNetworkSettings mobileNetworkSettings;

    public AttachedDataNetworkService(HttpClient httpClient,
        IOptions<MobileNetworkSettings> mobileNetworkOptions)
    {
        this.httpClient = httpClient;
        mobileNetworkSettings = mobileNetworkOptions.Value;
    }

    public async Task<AttachedDataNetworkProperties> GetAttachedDataNetworkProperties()
    {
        var response = await httpClient.GetAsync($"{mobileNetworkSettings.AttachedDataNetwork}?api-version=2023-09-01");
        if (response.IsSuccessStatusCode)
        {
            var attachedDataNetwork = await response.Content
                .ReadFromJsonAsync<ControlPlane.API.Models.Azure.AttachedDataNetworkResponse>();

            return new AttachedDataNetworkProperties(attachedDataNetwork?.Name ?? string.Empty,
                attachedDataNetwork?
                    .Properties?
                    .UserEquipmentStaticAddressPoolPrefix?
                    .FirstOrDefault() ?? string.Empty);
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(error);
        }
    }
}