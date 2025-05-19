// Copyright 2025 Roman Lefler

using Microsoft.AspNetCore.SignalR;
using Riffle.Models;
using Riffle.Models.Games;
using Riffle.Services;
using Riffle.Utilities;

namespace Riffle.Hubs
{
    public class RoomHub : Hub
    {
        public async Task CreateRoom(int game)
        {
            string host = Context.ConnectionId;

            GameType gtype = (GameType)game;
            if(gtype < GameType.Min || gtype > GameType.Max)
            {
            }

            Room room;
            try
            {
                switch(gtype)
                {
                    case GameType.Roundabout:
                        room = new RoundaboutRoom(host);
                        break;
                    default:
                        await Clients.Caller.SendAsync("RoomError", "Not a valid game ID.");
                        return;
                }
            }
            catch (TooManyRoomsException)
            {
                await Clients.Caller.SendAsync("RoomError", "Couldn't create a room.");
                return;
            }

            bool addSuccess = RoomManager.AddRoom(room);
            if (!addSuccess)
            {
                await Clients.Caller.SendAsync("RoomError", "You can't create multiple rooms.");
            }

            await Groups.AddToGroupAsync(host, room.JoinCode);
            await Clients.Caller.SendAsync("RoomCreated", room.JoinCode);
        }

        public async Task JoinRoom(string joinCode, string name)
        {
            Room? room;
            if(RoomManager.Rooms.TryGetValue(joinCode, out room))
            {
                room.AddMember(Context.ConnectionId, name);
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
                room.RemoveMember(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, joinCode);
                await Clients.Group(joinCode).SendAsync("UserLeft", Context.ConnectionId);
            }
        }

        public async Task StringMsg(string msgName, string content)
        {

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
                    await Clients.Group(room.JoinCode).SendAsync("RoomClosed");
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

    }
}
