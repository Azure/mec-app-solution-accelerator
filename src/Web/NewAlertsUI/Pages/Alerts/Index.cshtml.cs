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

        public IActionResult Mockup()
        {
            alerts.Add(new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert("3", "3", "123", new DateTime(), new DateTime(), 20, "type", 10, new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Source("name", "type", 10, 10), "50"));
            alerts.Add(new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Alert("4", "4", "123", new DateTime(), new DateTime(), 20, "type", 10, new Microsoft.MecSolutionAccelerator.AlertsUI.Models.Source("name", "type", 10, 10), "50"));
            return Page();
        }
    }
}

