// Copyright 2025 Roman Lefler

using Riffle.Models;
using System.Collections.Concurrent;

namespace Riffle.Services
{
    public class RoomManager
    {
        public static ConcurrentDictionary<string, Room> Rooms { get; } = [];
        public static ConcurrentDictionary<string, Room> HostToRoom { get; } = [];

        public static bool AddRoom(Room room)
        {
            return Rooms.TryAdd(room.JoinCode, room) && HostToRoom.TryAdd(room.HostConnectionId, room);
        }

        public static bool RemoveRoom(Room room)
        {
            return Rooms.TryRemove(room.JoinCode, out _) && Rooms.TryRemove(room.HostConnectionId, out _);
        }

    }
}
