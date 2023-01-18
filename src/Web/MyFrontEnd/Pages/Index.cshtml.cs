using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyFrontEnd.Models;

namespace MyFrontEnd.Pages;

public class IndexModel : PageModel
{
    public IEnumerable<Alert> Alerts { get; set; }

    public IndexModel()
    {
        Alerts = new List<Alert>();

    }

    public async Task OnGet()
    {
        ViewData["Alerts"] = Alerts;
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
        return Sources.OrderBy(o => o.Name).ToList();
    }
}