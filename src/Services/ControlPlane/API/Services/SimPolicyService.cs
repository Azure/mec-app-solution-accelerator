using ControlPlane.API.Models;
using ControlPlane.API.Settings;
using Microsoft.Extensions.Options;

public class SimPolicyService
{
    private readonly HttpClient httpClient;
    private readonly MobileNetworkSettings mobileNetworkSettings;

    public SimPolicyService(HttpClient httpClient,
        IOptions<MobileNetworkSettings> mobileNetworkOptions)
    {
        this.httpClient = httpClient;
        mobileNetworkSettings = mobileNetworkOptions.Value;
    }

    public async Task<IEnumerable<SimPolicy>> GetSimPolicies()
    {
        var basePath = BuildBaseApiPath();
        var response = await httpClient.GetAsync($"{basePath}/mobileNetworks/{mobileNetworkSettings.Name}/simPolicies?api-version=2023-09-01");
        if (response.IsSuccessStatusCode)
        {
            var textresponse = await response.Content.ReadAsStringAsync();
            var simPolicyResponse = await response.Content.ReadFromJsonAsync<ControlPlane.API.Models.Azure.SimPoliciesResponse>();
            return simPolicyResponse.Value.Select(x => new SimPolicy(x.Id, x.Name));
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