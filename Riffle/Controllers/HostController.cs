// Copyright 2025 Roman Lefler

using Microsoft.AspNetCore.Mvc;
using Riffle.Utilities;

namespace Riffle.Controllers
{
    public class HostController : Controller
    {
        [HttpGet("/Host")]
        public IActionResult Host()
        {
            return View();
        }

        [HttpGet("/Host/Roundabout")]
        public IActionResult Roundabout()
        {
            return View("Roundabout");
        }

    }
}
