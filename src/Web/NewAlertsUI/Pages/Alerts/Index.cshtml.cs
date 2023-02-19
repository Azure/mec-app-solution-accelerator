using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapr.Client;

namespace NewAlertsUI.Pages.Alerts
{
    public class IndexAlertModel : PageModel
    {
        public IEnumerable<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert>? alerts;
        private readonly DaprClient _daprClient;


        public IndexAlertModel(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }
        private async Task<string> RefreshData()
        {
            this.alerts = await _daprClient.InvokeMethodAsync<IEnumerable<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert>>(
                HttpMethod.Get,
                "alerts-api",
                "alerts");

            ViewData["Alerts"] = alerts;
            return "New data added";
        }

        public void OnGet()
        {
            alerts = new List<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert>();
            ViewData["Alerts"] = alerts;
        }

        [HttpGet]
        public async Task<IActionResult> OnGetRefresh()
        {
            await RefreshData();
            return Partial("_AlertsTable", alerts);
        }       
    }
}

