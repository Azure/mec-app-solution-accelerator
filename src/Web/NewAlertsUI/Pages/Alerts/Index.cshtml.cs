using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapr.Client;
using Microsoft.MecSolutionAccelerator.AlertsUI.Models;

namespace NewAlertsUI.Pages.Alerts
{
    public class IndexAlertModel : PageModel
    {
        public IEnumerable<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert>? alerts;
        private readonly DaprClient _daprClient;

        //mockup
        public Alert test;

        public IndexAlertModel(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }
        private async Task<string> RefreshData()
        {
            //this.alerts = await _daprClient.InvokeMethodAsync<IEnumerable<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert>>(
            //    HttpMethod.Get,
            //    "alerts-api",
            //    "alerts");

            //mockup
            List<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert> alertsList = new List<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert>();
            alertsList.Add(new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert("1", "1", "123", new DateTime(), new DateTime(), 20, "type", 10, new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Source("name", "type", 10, 10), "50"));
            alerts = alertsList;


            ViewData["Alerts"] = alerts;
            return "New data added";
        }

        public void OnGet()
        {
            alerts = new List<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert>();
            ViewData["Alerts"] = alerts;


            //mockup
            test = new Alert("test", "", "", new DateTime(), new DateTime(), 20, "type", 10, new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Source("name", "type", 10, 10), "50");
        }

        [HttpGet]
        public async Task<IActionResult> OnGetRefresh()
        {
            await RefreshData();
            return Partial("_AlertsTable", alerts);
        }

        [HttpGet]
        public IActionResult OnGetDetails(string alertId)
        {
            Alert alertDetail = null;
            foreach (var alert in alerts.ToList())
            {
                if(alert.Id == alertId)
                {
                    alertDetail = alert;
                }
            }
            if (alertDetail == null)
            {
                return NotFound();
            }
            else
            {
                return Partial("_AlertsDetails", alertDetail);

            }
        }
    }
}

