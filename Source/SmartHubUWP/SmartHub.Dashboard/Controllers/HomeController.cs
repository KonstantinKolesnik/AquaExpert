using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SmartHub.Dashboard.Common;
using SmartHub.Dashboard.Models;
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
            ViewBag.Text = "<h3>Hello!</h3><script>var a = 0; alert('OK!');</script>";
            return View();
            //return Redirect("/Home/System");
            //return View("System");
        }
        public async Task<IActionResult> Applications()
        {
            ViewData["Message"] = "Applications";
            ViewBag.ServerUrl = config["serverUrl"];

            var data = await Utils.GETRequest(config["serverUrl"] + "/api/ui/sections/apps");
            var model = Utils.DtoDeserialize<List<AppSectionItem>>(data);

            return View(model);
        }
        public async Task<IActionResult> System()
        {
            ViewData["Message"] = "System maintenance";
            ViewBag.ServerUrl = config["serverUrl"];

            var data = await Utils.GETRequest(config["serverUrl"] + "/api/ui/sections/system");
            var model = Utils.DtoDeserialize<List<AppSectionItem>>(data);

            return View(model);
        }

        public IActionResult SectionItem(AppSectionItem item)
        {
            ViewData["Message"] = item.Name;
            ViewBag.ServerUrl = config["serverUrl"];

            return View(item);
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
}
