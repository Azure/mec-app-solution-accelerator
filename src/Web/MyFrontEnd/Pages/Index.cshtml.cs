using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapr.Client;
using MyFrontEnd.Models;

namespace MyFrontEnd.Pages;

public class IndexModel : PageModel
{
    private readonly DaprClient _daprClient;
    public List<Alert> Alerts { get; private set; }
    public List<Source> Sources { get; set; }
    public Alert selectedAlert { get; set; }
    public Boolean imageSelected;
    public Boolean textSelected;
    public Boolean hasTwoInputs;

    public IndexModel() //DaprClient daprClient
    {
        //_daprClient = daprClient;
        Alerts = new List<Alert>();
        Sources = new List<Source>();
    }

    public async Task OnGet()
    {
        //var alerts = await _daprClient.InvokeMethodAsync<IEnumerable<Alert>>(
        //    HttpMethod.Get,
        //    "alerts-api",
        //    "alerts");

        //ViewData["AlertsData"] = alerts;
        Alerts.Add(new Alert("alert-1", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
        Alerts.Add(new Alert("alert-2", "sensor-1", "medium",  "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
        Alerts.Add(new Alert("alert-3", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
        Alerts.Add(new Alert("alert-4", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
        Alerts.Add(new Alert("alert-5", "camera-4", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));

        Sources.Add(new Camera("camera-1", "Camera 1", "camera", 123, 234, 50, 90));
        Sources.Add(new Sensor("sensor-1", "Sensor 1", "sensor", 345, 456, "temperature"));
        Sources.Add(new Camera("camera-2", "Camera 2", "camera", 189, 178, 50, 180));
        Sources.Add(new Camera("camera-3", "Camera 3", "camera", 257, 234, 50, 135));
        Sources.Add(new Camera("camera-4", "Camera 4", "camera", 257, 234, 50, 135));
    }

    public List<Alert> GetAlerts()
    {
        foreach(var alert in Alerts)
        {
            Console.WriteLine(alert.toString());
        }
        return Alerts;
    }

    public string SelectAlert(Alert alert)
    {
        this.selectedAlert = alert;
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

    //public string RefreshData()
    //{
    //    Alerts.Add(new Alert("alert-6", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
    //    Alerts.Add(new Alert("alert-7", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
    //    Alerts.Add(new Alert("alert-8", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
    //    Alerts.Add(new Alert("alert-9", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
    //    return "New data added";
    //}
}