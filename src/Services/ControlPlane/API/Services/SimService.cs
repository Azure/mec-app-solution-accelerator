using System.Net.Http.Headers;
using System.Text.Json;
using ControlPlane.API.Models;
using ControlPlane.API.Settings;
using Microsoft.Extensions.Options;

public class SimService
{
    private readonly HttpClient httpClient;
    private readonly MobileNetworkSettings mobileNetworkSettings;

    public SimService(HttpClient httpClient,
        IOptions<MobileNetworkSettings> mobileNetworkOptions)
    {
        this.httpClient = httpClient;
        mobileNetworkSettings = mobileNetworkOptions.Value;
    }

    public async Task<IEnumerable<Sim>> GetSims(string simGroup)
    {
        var basePath = BuildBaseApiPath();
        var response = await httpClient.GetAsync($"{basePath}/simGroups/{simGroup}/sims?api-version=2023-09-01");
        if (response.IsSuccessStatusCode)
        {
            var simsResponse = await response.Content.ReadFromJsonAsync<ControlPlane.API.Models.Azure.SimsResponse>();
            return simsResponse?.Value.Select(x => new Sim(
                x.Name,
                x.Properties.IntegratedCircuitCardIdentifier ?? "",
                x.Properties.InternationalMobileSubscriberIdentity,
                x.Properties.OperatorKeyCode ?? "",
                simGroup,
                x.Properties.AuthenticationKey ?? "",
                x.Properties.StaticIpConfiguration?.FirstOrDefault()?.StaticIp?.Ipv4Address,
                x.Properties.SimPolicy?.Id,
                x.Properties.SimState
            )) ?? new List<Sim>();
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(error);
        }
    }

    public async Task<Sim> CreateSim(Sim sim)
    {
        var basePath = BuildBaseApiPath();

        var policy = String.IsNullOrEmpty(sim.PolicyId) ? null :
            new ControlPlane.API.Models.Azure.ResourceReference(sim.PolicyId);
        var ip = String.IsNullOrEmpty(sim.Ip) ? null :
            new List<ControlPlane.API.Models.Azure.StaticIpConfiguration>() {
                new ControlPlane.API.Models.Azure.StaticIpConfiguration(
                    new ControlPlane.API.Models.Azure.ResourceReference(mobileNetworkSettings.AttachedDataNetwork),
                    new ControlPlane.API.Models.Azure.ResourceReference(mobileNetworkSettings.Slice),
                    new ControlPlane.API.Models.Azure.StaticIp(sim.Ip))};
        var requestBody = new ControlPlane.API.Models.Azure.CreateSimRequest(
            new ControlPlane.API.Models.Azure.CreateSimProperties(
                string.IsNullOrEmpty(sim.Iccid) ? null : sim.Iccid,
                sim.Imsi,
                sim.Ki,
                sim.Opc,
                "Camera",
                policy,
                ip
            )
        );

        var requestBodyString = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var response = await httpClient.PutAsync(
            $"{basePath}/simGroups/{sim.GroupId}/sims/{sim.Name}?api-version=2023-09-01",
            new StringContent(requestBodyString, new MediaTypeHeaderValue("application/json")));

        if (response.IsSuccessStatusCode)
        {
            return sim;
        }

        var error = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(error);
    }

    public async Task<bool> DeleteSim(string simName, string groupName)
    {
        var basePath = BuildBaseApiPath();
        var path = $"{basePath}/simGroups/{groupName}/sims/{simName}?api-version=2023-09-01";
        var response = await httpClient.DeleteAsync(path);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        var error = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(error);
    }

    private string BuildBaseApiPath()
    {
        return $"/subscriptions/{mobileNetworkSettings.SubscriptionId}/resourceGroups/{mobileNetworkSettings.ResourceGroup}/providers/Microsoft.MobileNetwork";
    }
}