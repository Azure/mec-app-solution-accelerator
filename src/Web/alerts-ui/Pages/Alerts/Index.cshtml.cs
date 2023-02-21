using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapr.Client;
using Microsoft.MecSolutionAccelerator.AlertsUI.Models;

namespace Microsoft.MecSolutionAccelerator.AlertsUI.Pages.Alerts
{
    public class IndexAlertModel : PageModel
    {
        public IEnumerable<AlertReducedModel> alerts;
        private readonly DaprClient _daprClient;

       

        public IndexAlertModel(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }
        private async Task<string> RefreshData()
        {
            this.alerts = await _daprClient.InvokeMethodAsync<IEnumerable<AlertReducedModel>>(
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

            return Partial("_AlertsTable", alerts);
        }

        [HttpGet]
        public async Task<IActionResult> OnGetDetails(string id)
        {
            AlertDetailsModel alertDetail = await _daprClient.InvokeMethodAsync<AlertDetailsModel>(
                HttpMethod.Get,
                "alerts-api",
                $"alerts/{id}");

            return Partial("_AlertsDetails", alertDetail);
        }
    }
}

