// Copyright 2025 Roman Lefler

using Microsoft.AspNetCore.Mvc;
using Riffle.Models;
using Riffle.Services;
using Riffle.Utilities;

namespace Riffle.Controllers
{

    public class JoinController : Controller
    {
        [HttpGet("/Join")]
        public IActionResult JoinGet(string? code)
        {
            if (code != null) ViewData["code"] = code;

            return View("Join");
        }

        [HttpPost("/Join/JoinGame")]
        public IActionResult JoinGame(string name, string joinCode)
        {
            if(string.IsNullOrWhiteSpace(joinCode))
            {
                ModelState.AddModelError("JoinCode", "Empty join code.");
                return View("Join");
            }

            string norm = JoinCode.NormalizeCode(joinCode);

            if(!JoinCode.IsValidCode(norm))
            {
                ModelState.AddModelError("JoinCode", "Invalid join code.");
                return View("Join");
            }

            RoomManager.Rooms.TryGetValue(norm, out Room? room);
            if(room == null)
            {
                ModelState.AddModelError("JoinCode", "That room doesn't exist.");
                return View("Join");
            }

            if(room.IsFull())
            {
                ModelState.AddModelError("JoinCode", "That room is full!");
                return View("Join");
            }

            ViewData["joinCode"] = joinCode;
            ViewData["playerName"] = name;

            string view = room.Game switch
            {
                GameType.Roundabout => "Roundabout",
                _ => "Join"
            };

            return View(view);

        }

    }

}
