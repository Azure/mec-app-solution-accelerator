using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyFrontEnd.Pages;
using MyFrontEnd.Models;

namespace MyFrontEnd.Pages.Shared
{
	public class _InspectionTableModel : PageModel
    {
        public IndexModel IndexModel;
        public List<Alert> Alerts;

        public _InspectionTableModel(IndexModel indexModel)
        {
            IndexModel = indexModel;
        }

        public void OnGet()
        {
            Alerts = IndexModel.GetAlerts();
        }

        public string RefreshData()
        {
            Alerts.Add(new Alert("alert-6", "camera-1", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            Alerts.Add(new Alert("alert-7", "sensor-1", "medium", "Sensor got wet, possible flooding!", "null", 75, DateTime.Now));
            Alerts.Add(new Alert("alert-8", "camera-2", "low", "Animal detected!", "./assets/bird", 40, DateTime.Now));
            Alerts.Add(new Alert("alert-9", "camera-3", "high", "Suspicious individual detected!", "./assets/suspect-individual", 94, DateTime.Now));
            return "New data added";
        }
    }
}
