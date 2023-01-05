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
    public class SourceController : Controller
    {
        public List<Source> Sources;
        // GET: /<controller>/
        public List<Source> Source()
        {
            Sources = new List<Source>();
            Sources.Add(new Camera("camera-1", "Camera 1", "camera", 123, 234, 50, 90));
            Sources.Add(new Sensor("sensor-1", "Sensor 1", "sensor", 345, 456, "temperature"));
            Sources.Add(new Camera("camera-2", "Camera 2", "camera", 189, 178, 50, 180));
            Sources.Add(new Camera("camera-3", "Camera 3", "camera", 257, 234, 50, 135));
            Sources.Add(new Camera("camera-4", "Camera 4", "camera", 257, 234, 50, 135));
            ViewData["Sources"] = Sources;
            return Sources;
        }
    }
}

