using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapr.Client;
using Microsoft.MecSolutionAccelerator.AlertsUI.Models;

namespace NewAlertsUI.Pages.Alerts
{
    public class IndexAlertModel : PageModel
    {
        private IEnumerable<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert> alerts;
        private readonly DaprClient _daprClient;

       

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

        }

        [HttpGet]
        public async Task<IActionResult> OnGetRefresh()
        {
            await RefreshData();
            return Partial("_AlertsTable", alerts);
        }

        [HttpGet]
        public IActionResult OnGetDetails(string id)
        {
            Alert alertDetail = null;
            alerts = (IEnumerable<Alert>?)ViewData["Alerts"];
            if (alerts != null) {
                foreach (var alert in alerts.ToList())
                {
                    if (alert.Id == id)
                    {
                        alertDetail = alert;
                    }
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

