using ControlPlane.API.Models;
using ControlPlane.API.Settings;
using Microsoft.Extensions.Options;

public class SimGroupService
{
    private readonly HttpClient httpClient;
    private readonly MobileNetworkSettings mobileNetworkSettings;

    public SimGroupService(HttpClient httpClient,
        IOptions<MobileNetworkSettings> mobileNetworkOptions)
    {
        this.httpClient = httpClient;
        mobileNetworkSettings = mobileNetworkOptions.Value;
    }

    public async Task<IEnumerable<SimGroup>> GetSimGroups()
    {
        var basePath = BuildBaseApiPath();
        var response = await httpClient.GetAsync($"{basePath}/simGroups?api-version=2023-09-01");
        if (response.IsSuccessStatusCode)
        {
            var simGroupResponse = await response.Content.ReadFromJsonAsync<ControlPlane.API.Models.Azure.SimGroupsResponse>();
            // Using name as id, we are not using the internal id anywhere.
            return simGroupResponse?.Value.Select(x => new SimGroup(x.Name, x.Name)) ?? new List<SimGroup>();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(error);
        }
    }

    private string BuildBaseApiPath()
    {
        return $"/subscriptions/{mobileNetworkSettings.SubscriptionId}/resourceGroups/{mobileNetworkSettings.ResourceGroup}/providers/Microsoft.MobileNetwork";
    }
}