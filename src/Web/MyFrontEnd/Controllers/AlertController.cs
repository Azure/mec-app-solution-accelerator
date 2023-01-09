using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyFrontEnd.Models;
using MyFrontEnd.Pages;
using MyFrontEnd.Pages.Shared;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFrontEnd.Controllers
{
    public class AlertController : Controller
    {
        public Stack<Alert> Alerts;
        public List<Alert> AlertsToAdd;

        // GET: /<controller>/
        public Stack<Alert> Alert()
        {
            AlertsToAdd = new List<Alert>();
            AddAlertsToAdd();

            Alerts = new Stack<Alert>();
            // Alerts.Push(new Alert("alert-1", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // Alerts.Push(new Alert("alert-2", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // Alerts.Push(new Alert("alert-3", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // Alerts.Push(new Alert("alert-4", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // Alerts.Push(new Alert("alert-5", "camera-4", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            ViewData["Alerts"] = Alerts;
            return Alerts;
        }

        public Stack<Alert> GetAlerts()
        {
            return Alerts;
        }

        public ActionResult RefreshPage()
        {
            Alerts = RefreshData();
            Console.WriteLine(Alerts.Count);
            IndexModel Model = new IndexModel();
            return View("Alert", Model);
        }

        public Stack<Alert> RefreshData()
        {
            Alerts = Alert();
            // Alerts.Push(new Alert("alert-6", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // Alerts.Push(new Alert("alert-7", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // Alerts.Push(new Alert("alert-8", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // Alerts.Push(new Alert("alert-9", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // Alerts.Push(new Alert("alert-10", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // Alerts.Push(new Alert("alert-11", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // Alerts.Push(new Alert("alert-12", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // Alerts.Push(new Alert("alert-13", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // Alerts.Push(new Alert("alert-14", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // Alerts.Push(new Alert("alert-15", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // Alerts.Push(new Alert("alert-16", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // Alerts.Push(new Alert("alert-17", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            return Alerts;
        }

        public void AddAlertsToAdd()
        {
            // AlertsToAdd.Add(new Alert("alert-6", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-7", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-8", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-9", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-10", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-11", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-12", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-13", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-14", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-15", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-16", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-17", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-18", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-19", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-20", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-21", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-22", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-23", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-24", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-25", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-26", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-27", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-28", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-29", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-30", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-31", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-32", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-33", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-34", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-35", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-36", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-37", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-38", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-39", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-40", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-41", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-42", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-43", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-44", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-45", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-46", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-47", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-48", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-49", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-50", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-51", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-52", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            // AlertsToAdd.Add(new Alert("alert-53", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
        }
    }
}

