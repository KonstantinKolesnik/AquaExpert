using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmartHub.Dashboard.Common;
using SmartHub.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
//using SmartHub.Dashboard.Services;

namespace SmartHub.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        //private readonly SmartHubService smartHubService;
        private readonly IConfiguration config;

        //public HomeController(SmartHubService smartHubService)
        //{
        //    this.smartHubService = smartHubService;
        //}
        public HomeController(IConfiguration config)
        {
            this.config = config;
            //ViewBag.ServerUrl = config["serverUrl"];
        }


        public IActionResult Index()
        {
            return View();
            //return Redirect("/Home/System");
        }
        public IActionResult Applications()
        {
            ViewData["Message"] = "Applications";
            return View();
        }
        public async Task<IActionResult> System()
        {
            ViewData["Message"] = "System maintenance";
            ViewBag.ServerUrl = config["serverUrl"];

            //var data = await Utils.GETRequest(config["serverUrl"] + "/api/ui/sections/system");
            //var model = Utils.DtoDeserialize<List<AppSectionItemAttribute>>(data);

            return View();// model);
        }
        //public IActionResult About()
        //{
        //    ViewData["Message"] = "Your application description page.";
        //    return View();
        //}
        //public IActionResult Contact()
        //{
        //    //var result = smartHubService.Function<string>("gothicmaestro.asuscomm.com", 11111, "/api/ui/sections/apps");

        //    ViewData["Message"] = "Your contact page.";
        //    return View();
        //}

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }





    public enum AppSectionType
    {
        Applications,
        System
    }

    public class AppSectionItemAttribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public AppSectionType Type { get; set; }
        //public Type UIModuleType { get; set; }
    }

}
