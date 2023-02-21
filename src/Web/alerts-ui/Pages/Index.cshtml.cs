using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.MecSolutionAccelerator.AlertsUI.Models;
using Microsoft.AspNetCore.Mvc;
using Dapr.Client;

namespace Microsoft.MecSolutionAccelerator.AlertsUI.Pages;

public class IndexModel : PageModel
{
    public IEnumerable<AlertReducedModel> alerts;
    private readonly DaprClient _daprClient;



    public IndexModel(DaprClient daprClient)
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
        var test = Request.Cookies["timezone"];

        return "New data added";
    }

    public void OnGet()
    {

    }

    [HttpGet]
    public async Task<IActionResult> OnGetRefresh()
    {
        await RefreshData();
        return Partial("Alerts/_AlertsTable", alerts);
    }

    [HttpGet]
    public async Task<IActionResult> OnGetDetails(string id)
    {
        AlertDetailsModel alertDetail = await _daprClient.InvokeMethodAsync<AlertDetailsModel>(
            HttpMethod.Get,
            "alerts-api",
            $"alerts/{id}");

        return Partial("Alerts/_AlertsDetails", alertDetail);

    }

    public string FindInputs(AlertDetailsModel alert)
    {
        if (alert.Information != null && alert.Frame != null)
        {
            return "both";
        }
        else if (alert.Information != null)
        {
            return "text";
        }
        return "image";
    }

    public List<SourceModel> getSources()
    {
        List<SourceModel> Sources = new List<SourceModel>();
        foreach(AlertReducedModel alert in alerts)
        {
            if(Sources.Contains(alert.Source) == false)
            {
                Sources.Add(alert.Source);
            }
        }
        return Sources.OrderBy(o => o.Name).ToList();
    }
}