using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using Osmium.WebCore.Services;

public enum LibType
{
    None,
    Angular2,
    Aurelia
}

namespace Osmium.WebCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyEnvironment _env;

        public HomeController(MyEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index(LibType lib = LibType.None)
        {
            switch (lib)
            {
                case LibType.Angular2:
                    return View("Index-Angular2");
                case LibType.Aurelia:
                    return View("Index-Aurelia");
                default:
                    return View();
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] =
                $@"{_env.ApplicationName} v{_env.ApplicationVersion} 
                   using {_env.Framework} and running in the 
                   {_env.HostEnvironment} environment";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
