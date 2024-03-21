using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.AlertsUI.Models;

namespace Microsoft.MecSolutionAccelerator.AlertsUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : Controller
    {
        private readonly DaprClient _daprClient;

        public AlertsController(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlertReducedModel>>> GetAlerts()
        {
            var alerts = await _daprClient.InvokeMethodAsync<IEnumerable<AlertReducedModel>>(
                HttpMethod.Get,
                "alerts-api",
                "alerts");

            return Ok(alerts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlertDetails(string id)
        {
            var alertDetail = await _daprClient.InvokeMethodAsync<AlertDetailsModel>(
                    HttpMethod.Get,
                    "alerts-api",
                    $"alerts/{id}");

            if (alertDetail == null)
            {
                return NotFound();
            }

            return Ok(alertDetail);
        }
    }
}
