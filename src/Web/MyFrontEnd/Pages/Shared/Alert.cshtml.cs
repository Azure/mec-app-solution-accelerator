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
	public class AlertModel : PageModel
    {
        public IndexModel IndexModel;
        public IEnumerable<Alert> Alerts;

        public AlertModel(IndexModel indexModel)
        {
            IndexModel = indexModel;
        }

        public void OnGet()
        {
            //Alerts = (List<Alert>)ViewData["Alerts"];
            Alerts = IndexModel.GetAlerts();
        }
    }
}
