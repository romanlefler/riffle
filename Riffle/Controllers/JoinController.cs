// Copyright 2025 Roman Lefler

using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Riffle.Models;
using Riffle.Services;
using Riffle.Utilities;

namespace Riffle.Controllers
{

    public class JoinController : Controller
    {
        private readonly AssetMapService _assetMapService;

        public JoinController(AssetMapService assetMapService)
        {
            _assetMapService = assetMapService;
        }

        [HttpGet("/Join")]
        public IActionResult Join(string? code)
        {
            if (code != null) ViewData["code"] = code;

            return View("Join");
        }

        [HttpGet("/Join/Play")]
        public IActionResult Play()
        {
            string? joinCode = HttpContext.Session.GetString("JoinCode");
            if (joinCode == null) return RedirectToAction("Join");
            string? plName = HttpContext.Session.GetString("PlayerName");
            if (plName == null) return RedirectToAction("Join");

            RoomManager.Rooms.TryGetValue(joinCode, out Room? room);
            if (room == null) return RedirectToAction("Join");

            ViewData["joinCode"] = joinCode;
            ViewData["playerName"] = plName;

            string view;
            switch (room.Game)
            {
                case GameType.Roundabout:
                    ViewData["ScriptSrc"] = _assetMapService.RoundaboutMemberJs;
                    ViewData["StyleSrc"] = _assetMapService.RoundaboutMemberCss;
                    view = "Roundabout";
                    break;
                default:
                    view = "Join";
                    break;
            }

            return View(view);
        }

        [HttpPost("/Join/JoinAction")]
        public IActionResult JoinAction(string name, string joinCode)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("Name", "Empty player name.");
                return RedirectToAction("Join");
            }

            if(string.IsNullOrWhiteSpace(joinCode))
            {
                ModelState.AddModelError("JoinCode", "Empty join code.");
                return RedirectToAction("Join");
            }

            string norm = JoinCode.NormalizeCode(joinCode);

            if(!JoinCode.IsValidCode(norm))
            {
                ModelState.AddModelError("JoinCode", "Invalid join code.");
                return RedirectToAction("Join");
            }

            RoomManager.Rooms.TryGetValue(norm, out Room? room);
            if(room == null)
            {
                ModelState.AddModelError("JoinCode", "That room doesn't exist.");
                return RedirectToAction("Join");
            }

            if(room.IsFull())
            {
                ModelState.AddModelError("JoinCode", "That room is full!");
                return RedirectToAction("Join");
            }

            HttpContext.Session.SetString("JoinCode", norm);
            HttpContext.Session.SetString("PlayerName", name);
            return RedirectToAction("Play", "Join");
        }

    }

}
