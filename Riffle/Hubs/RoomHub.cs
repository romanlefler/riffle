// Copyright 2025 Roman Lefler

using Microsoft.AspNetCore.SignalR;
using Riffle.Models;
using Riffle.Services;
using Riffle.Utilities;

namespace Riffle.Hubs
{
    public class RoomHub : Hub
    {
        public async Task CreateRoom(GameType game)
        {
            string host = Context.ConnectionId;
            Room room;
            try
            {
                room = new(host, game);
            }
            catch(TooManyRoomsException)
            {
                await Clients.Caller.SendAsync("RoomError", "Couldn't create a room.");
                return;
            }

            bool addSuccess = RoomManager.AddRoom(room);
            if(!addSuccess)
            {
                await Clients.Caller.SendAsync("RoomError", "You can't create multiple rooms.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, room.JoinCode);
            await Clients.Caller.SendAsync("RoomCreated", room.JoinCode);
        }

        public async Task JoinRoom(string joinCode)
        {
            Room? room;
            if(RoomManager.Rooms.TryGetValue(joinCode, out room))
            {
                room.Participants.Add(Context.ConnectionId);
                await Groups.AddToGroupAsync(Context.ConnectionId, joinCode);
                await Clients.Group(joinCode).SendAsync("UserJoined", Context.ConnectionId);
            }
            else
            {
                await Clients.Caller.SendAsync("RoomError", "Room doesn't exist.");
            }
        }

        public async Task LeaveRoom(string joinCode)
        {
            Room? room;
            if(RoomManager.Rooms.TryGetValue(joinCode, out room))
            {
                room.Participants.Remove(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, joinCode);
                await Clients.Group(joinCode).SendAsync("UserLeft", Context.ConnectionId);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Room? room;
            RoomManager.HostToRoom.TryGetValue(Context.ConnectionId, out room);
            if(room != null)
            {
                bool removeSuccess = RoomManager.RemoveRoom(room);
                if(removeSuccess)
                {
                    await Clients.Group(room.JoinCode).SendAsync("RoomClosed", room.JoinCode);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

    }
}
