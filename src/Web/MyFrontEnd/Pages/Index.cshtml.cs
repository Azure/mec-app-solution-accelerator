using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapr.Client;
using MyFrontEnd.Models;
using MyFrontEnd.Controllers;

namespace MyFrontEnd.Pages;

public class IndexModel : PageModel
{
    private readonly DaprClient _daprClient;
    private AlertController AlertController;
    private SourceController SourceController;
    public Stack<Alert> Alerts { get; private set; }
    public Alert? SelectedAlert { get; set; }

    public IndexModel() //DaprClient daprClient
    {
        //_daprClient = daprClient;
        AlertController = new AlertController();
        SourceController = new SourceController();
        Alerts = new Stack<Alert>();
        Alerts = AlertController.Alert();
        //SelectedAlert = selectedAlert;

    }

    public async Task OnGet()
    {
        //var Alerts = await _daprClient.InvokeMethodAsync<IEnumerable<Alert>>(
        //    HttpMethod.Get,
        //    "alerts-api",
        //    "alerts");

        //ViewData["Alerts"] = Alerts;
        Alerts = AlertController.Alert();
        ViewData["Alerts"] = Alerts;
    }

    public void SelectAlert(Alert alert)
    {
        SelectedAlert = alert;
    }

    public string FindInputs(Alert alert)
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

    public List<Source> getSources()
    {
        List<Source> Sources = new List<Source>();
        foreach(Alert alert in Alerts)
        {
            if(Sources.Contains(alert.Source) == false)
            {
                Sources.Add(alert.Source);
            }
        }
        Sources = Sources.OrderBy(o => o.Name).ToList();
        return Sources;
    }
}