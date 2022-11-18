using Dapr.Client;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyFrontEnd.Pages;

public class IndexModel : PageModel
{
    private readonly DaprClient _daprClient;

    public IndexModel(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task OnGet()
    {
        var alerts = await _daprClient.InvokeMethodAsync<IEnumerable<Alert>>(
            HttpMethod.Get,
            "alerts-api",
            "alerts");

        ViewData["AlertsData"] = alerts;
    }
}