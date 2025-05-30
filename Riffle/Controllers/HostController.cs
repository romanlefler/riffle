// Copyright 2025 Roman Lefler

using Microsoft.AspNetCore.Mvc;
using Riffle.Services;

namespace Riffle.Controllers
{
    public class HostController : Controller
    {

        private readonly AssetMapService _assetMapService;

        public HostController(AssetMapService assetMapService)
        {
            _assetMapService = assetMapService;
        }

        [HttpGet("/Host")]
        public IActionResult Host()
        {
            return View();
        }

        [HttpGet("/Host/Roundabout")]
        public IActionResult Roundabout()
        {
            ViewData["ScriptSrc"] = _assetMapService.RoundaboutHostJs;
            ViewData["StyleSrc"] = _assetMapService.RoundaboutHostCss;
            return View("Roundabout");
        }

    }
}
