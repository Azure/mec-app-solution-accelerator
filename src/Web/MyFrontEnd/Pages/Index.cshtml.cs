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
    public List<Source> Sources { get; set; }
    public Alert selectedAlert { get; set; }
    public Boolean imageSelected;
    public Boolean textSelected;
    public Boolean hasTwoInputs;

    public IndexModel() //DaprClient daprClient
    {
        //_daprClient = daprClient;
        AlertController = new AlertController();
        SourceController = new SourceController();
        Alerts = new Stack<Alert>();
        Alerts = AlertController.Alert();
        Sources = new List<Source>();
        Sources = SourceController.Source();
        selectedAlert = null;
    }

    public async Task OnGet()
    {
        //var alerts = await _daprClient.InvokeMethodAsync<IEnumerable<Alert>>(
        //    HttpMethod.Get,
        //    "alerts-api",
        //    "alerts");

        //ViewData["AlertsData"] = alerts;
        Alerts = AlertController.Alert();
        ViewData["Alerts"] = Alerts;

        Sources = SourceController.Source();
        ViewData["Sources"] = Sources;
    }

    public Stack<Alert> GetAlerts()
    {
        foreach(var alert in Alerts)
        {
            Console.WriteLine(alert.toString());
        }
        return Alerts;
    }

    public string SelectAlert(Alert alert)
    {
        selectedAlert = alert;
        SelectInputs(selectedAlert);
        return "Alert: " + selectedAlert.Id + " has been selected, whose source is "+ selectedAlert.SourceId;
    }

    public void SelectInputs(Alert alert)
    {
        if(alert.Information != "null" && alert.UrlImageEncoded != "null")
        {
            imageSelected = false;
            textSelected = false;
            hasTwoInputs = true;
        } else if (alert.Information != "null")
        {
            imageSelected = false;
            textSelected = true;
            hasTwoInputs = false;
        } else
        {
            imageSelected = true;
            textSelected = false;
            hasTwoInputs = false;
        }
    }

    public string SelectText()
    {
        imageSelected = false;
        textSelected = true;
        hasTwoInputs = false;
        return "Text selected";
    }

    public string SelectImage()
    {
        imageSelected = true;
        textSelected = false;
        hasTwoInputs = false;
        return "Image selected";
    }

    public string FindInputs(Alert alert)
    {
        if (alert.Information != "null" && alert.UrlImageEncoded != "null")
        {
            return "both";
        }
        else if (alert.Information != "null")
        {
            return "text";
        }
        return "image";
    }

    public Source FindSource(Alert alert)
    {
        var sourceId = alert.SourceId;
        var selectedSource = new Source("", "", "", 0, 0);
        foreach (var source in Sources)
        {
            if (source.Id == sourceId)
            {
                selectedSource = source;
            }
        }
        return selectedSource;
    }

    public string getSourceName(string id)
    {
        foreach(var source in Sources)
        {
            if(source.Id == id)
            {
                return source.Name;
            }
        }
        return "";
    }

    public string RefreshData()
    {
        Alerts = AlertController.RefreshData();
        ViewData["Alerts"] = Alerts;
        Console.WriteLine(Alerts.Count);
        return "New data added";
    }

    public Boolean GetMax(Alert alert)
    {
        //foreach(var alert in AlertController.GetAlerts())
        //int index = AlertController.GetAlerts().IndexOf(alert);
        //int max = Math.Max(GetLoaded(), Alerts.Count);
        //Console.WriteLine(alert.toString());
        //Console.WriteLine($"Loaded is {GetLoaded()} and index is {index}, so max is {max}.");
        //if (index < max)
        //{
            return true;
        //}
        //else
        //{
        //    return false;
        //}
    }

    public int GetLoaded()
    {
        return 20;
    }
}