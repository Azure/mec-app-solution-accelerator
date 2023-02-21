using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.MecSolutionAccelerator.AlertsUI.Models;
using Microsoft.AspNetCore.Mvc;
using Dapr.Client;

namespace Microsoft.MecSolutionAccelerator.AlertsUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DaprClient _daprClient;

        public IndexModel(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public IEnumerable<AlertReducedModel> Alerts { get; private set; }

        public async Task<IActionResult> OnGetRefresh()
        {
            Alerts = await _daprClient.InvokeMethodAsync<IEnumerable<AlertReducedModel>>(
                HttpMethod.Get,
                "alerts-api",
                "alerts");

            return Partial("Alerts/_AlertsTable", Alerts);
        }

        public async Task<IActionResult> OnGetDetails(string id)
        {
            AlertDetailsModel alertDetail = await _daprClient.InvokeMethodAsync<AlertDetailsModel>(
                HttpMethod.Get,
                "alerts-api",
                $"alerts/{id}");

            return Partial("Alerts/_AlertsDetails", alertDetail);
        }
    }
}
