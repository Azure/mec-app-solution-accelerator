using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyFrontEnd.Pages;
using MyFrontEnd.Pages.Shared;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyFrontEnd.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class AlertController : Controller
    {
        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        public ActionResult Get()
        {
            _InspectionTableModel model = new _InspectionTableModel(new IndexModel());
            Console.WriteLine(model);
            Console.WriteLine(model.RefreshData());
            return PartialView(model);
        }
    }
}

