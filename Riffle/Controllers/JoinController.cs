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
        public IActionResult Join()
        {
            return View();
        }

        [HttpPost("/Join")]
        public IActionResult Join(string joinCode)
        {
            if(string.IsNullOrWhiteSpace(joinCode))
            {
                ModelState.AddModelError("JoinCode", "Empty join code.");
                return View();
            }

            string norm = JoinCode.NormalizeCode(joinCode);

            if(!JoinCode.IsValidCode(norm))
            {
                ModelState.AddModelError("JoinCode", "Invalid join code.");
                return View();
            }

            RoomManager.Rooms.TryGetValue(norm, out Room? room);
            if(room == null)
            {
                ModelState.AddModelError("JoinCode", "That room doesn't exist.");
                return View();
            }

            if(room.IsFull())
            {
                ModelState.AddModelError("JoinCode", "That room is full!");
                return View();
            }

            return RedirectToAction("Play", new { roomId = norm });

        }

    }

}
