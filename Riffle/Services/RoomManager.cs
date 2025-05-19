// Copyright 2025 Roman Lefler

using Riffle.Models;
using System.Collections.Concurrent;

namespace Riffle.Services
{
    public class RoomManager
    {
        public static ConcurrentDictionary<string, Room> Rooms { get; } = [];
        public static ConcurrentDictionary<string, Room> UserToRoom { get; } = [];

        public static bool AddRoom(Room room)
        {
            return Rooms.TryAdd(room.JoinCode, room) && MapUserToRoom(room.HostConnectionId, room);
        }

        public static bool MapUserToRoom(string connId, Room room)
        {
            return UserToRoom.TryAdd(connId, room);
        }

        public static bool RemoveRoom(Room room)
        {
            if (!Rooms.TryRemove(room.JoinCode, out _)) return false;
            foreach(RoomMember m in room.GetMembers())
            {
                UnmapUser(m.ConnectionId);
            }
            return true;
        }

        public static bool UnmapUser(string connId)
        {
            return Rooms.TryRemove(connId, out _);
        }

    }
}
