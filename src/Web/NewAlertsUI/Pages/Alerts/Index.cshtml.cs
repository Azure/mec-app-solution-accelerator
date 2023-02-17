using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NewAlertsUI.Pages.Alerts
{
    public class IndexAlertModel : PageModel
    {
        public List<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert>? alerts;
        public void OnGet()
        {
            alerts = new List<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert>();
            alerts.Add(new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert("1", "1", "123", new DateTime(), new DateTime(), 20, "type", 10, new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Source("name", "type", 10, 10), "50"));
            alerts.Add(new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert("2", "2", "123", new DateTime(), new DateTime(), 20, "type", 10, new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Source("name", "type", 10, 10), "50"));
        }

        [HttpGet]
        public IActionResult OnGetRefresh()
        {
            alerts = new List<Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert>();
            alerts.Add(new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert("1", "3", "123", new DateTime(), new DateTime(), 20, "type", 10, new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Source("name", "type", 10, 10), "50"));
            return Partial("_AlertsTable", alerts);
        }       
    }
}

