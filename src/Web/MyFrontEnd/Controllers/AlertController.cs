using Dapr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using MyFrontEnd.Models;
using MyFrontEnd.Pages;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFrontEnd.Controllers
{
    public class AlertController : Controller
    {
        public IEnumerable<Alert> Alerts;
        //public List<Alert> Alerts;
        private readonly DaprClient _daprClient;

        public AlertController(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        // GET: /<controller>/
        public IEnumerable<Alert> Alert()
        {
            Alerts = new List<Alert>();
            ViewData["Alerts"] = Alerts;
            return Alerts;
        }

        public async Task<ActionResult> RefreshPage()
        {
            await RefreshData();
            IndexModel Model = new IndexModel();
            Model.Alerts = Alerts;
            return View("Alert", Model);
        }

        private async Task<string> RefreshData()
        {
            this.Alerts = await _daprClient.InvokeMethodAsync<IEnumerable<Alert>>(
                HttpMethod.Get,
                "alerts-api",
                "alerts");

            ViewData["Alerts"] = Alerts;
            return "New data added";
        }

        //public int count = 0;

        //public ActionResult RefreshPage()
        //{
        //    RefreshData();
        //    IndexModel Model = new IndexModel();
        //    Model.Alerts = Alerts;
        //    return View("Alert", Model);
        //}

        //public string RefreshData()
        //{
        //    Console.WriteLine("Here I am!");
        //    Alerts = new List<Alert>();
        //    Alerts.Add(new Alert("1", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("2", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("3", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("4", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("5", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("6", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("7", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("8", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("9", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("10", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("11", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("12", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("13", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("14", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("15", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("16", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("17", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("18", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("19", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("20", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("21", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    Alerts.Add(new Alert("22", new Source("Camera 1", "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    for(int i = 0; i<=count; i++)
        //    {
        //        Alerts.Add(new Alert((22 + i).ToString(), new Source("Camera "+(i).ToString(), "camera", 124, 125), "", "", "", 75, DateTime.Now, DateTime.Now));
        //    }
        //    count = count + 1;
        //    ViewData["Alerts"] = Alerts;
        //    return "New data added";
        //}
    }
}