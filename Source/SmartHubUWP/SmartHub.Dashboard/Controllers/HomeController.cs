using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartHub.Dashboard.Models;
using SmartHub.Dashboard.Services;

namespace SmartHub.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly SmartHubService smartHubService;

        public HomeController(SmartHubService smartHubService)
        {
            this.smartHubService = smartHubService;




        }










        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Applications()
        {
            ViewData["Message"] = "Registered applications";
            return View();
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }
        public IActionResult Contact()
        {
            var result = smartHubService.Function<string>("gothicmaestro.asuscomm.com", 11111, "/api/ui/sections/apps");

            ViewData["Message"] = "Your contact page.";
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
