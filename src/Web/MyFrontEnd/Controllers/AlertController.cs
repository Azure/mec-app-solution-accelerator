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
        public List<Alert> Alerts;
        //private readonly DaprClient _daprClient;

        //public AlertController(DaprClient daprClient)
        //{
        //    _daprClient = daprClient;
        //}

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
            this.Alerts = new List<Alert>();
            IndexModel Model = new IndexModel();
            Model.Alerts = this.Alerts;
            return View("Alert", Model);
        }

        private async Task RefreshData()
        {
            //this.Alerts = await _daprClient.InvokeMethodAsync<IEnumerable<Alert>>(
            //    HttpMethod.Get,
            //    "alerts-api",
            //    "alerts");
            Alerts.Add(new Alert("alert-1", new Sensor("sensor-1", "Sensor 1", "sensor", 222, 333, "temperature"), "High temperature", "Very haigh temperature has been detected!", null, 95, DateTime.Now, DateTime.Now));
            Alerts.Add(new Alert("alert-2", new Camera("camera-1", "Camera 1", "camera", 222, 333, 50, 125), "Individual detected", "A suspect individual has been detected!", null, 65, DateTime.Now, DateTime.Now));
            ViewData["Alerts"] = Alerts;
        }
    }
}