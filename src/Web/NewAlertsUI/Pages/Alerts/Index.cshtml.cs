using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapr.Client;
using Microsoft.MecSolutionAccelerator.AlertsUI.Models;

namespace NewAlertsUI.Pages.Alerts
{
    public class IndexAlertModel : PageModel
    {
        public IEnumerable<Microsoft.MecSolutionAccelerator.AlertsUI.Models.AlertDetailsModel> alerts;
        private readonly DaprClient _daprClient;

       

        public IndexAlertModel(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }
        private async Task<string> RefreshData()
        {
            this.alerts = await _daprClient.InvokeMethodAsync<IEnumerable<Microsoft.MecSolutionAccelerator.AlertsUI.Models.AlertDetailsModel>>(
                HttpMethod.Get,
                "alerts-api",
                "alerts");

            //mockup
            //List<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert> alertsList = new List<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert>();
            //alertsList.Add(new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert("1", "1", "123", new DateTime(), new DateTime(), 20, "type", 10, new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Source("name", "type", 10, 10), "50"));
            //alerts = alertsList;


            return "New data added";
        }

        public void OnGet()
        {

        }

        [HttpGet]
        public async Task<IActionResult> OnGetRefresh()
        {
            await RefreshData();
            List<AlertReducedModel> alertsReduced = new List<AlertReducedModel>();
            foreach (AlertDetailsModel alert in alerts)
            {
                AlertReducedModel alertReduced = new AlertReducedModel(alert.Id, alert.CaptureTime, alert.AlertTime, alert.MsExecutionTime, alert.Source, alert.Type, alert.Information);
                alertsReduced.Add(alertReduced);
            }
            return Partial("_AlertsTable", alertsReduced);
        }

        [HttpGet]
        public async Task<IActionResult> OnGetDetails(string id)
        {
            this.alerts = await _daprClient.InvokeMethodAsync<IEnumerable<Microsoft.MecSolutionAccelerator.AlertsUI.Models.AlertDetailsModel>>(
                HttpMethod.Get,
                "alerts-api",
                "alerts");

            AlertDetailsModel alertDetail = alerts.Where(p => p.Id == id).DefaultIfEmpty(alerts.First()).First();

            return Partial("_AlertsDetails", alertDetail);

        }
    }
}

