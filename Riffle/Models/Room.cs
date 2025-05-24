// Copyright 2025 Roman Lefler

using Microsoft.AspNetCore.SignalR;
using Riffle.Utilities;
using System.Runtime.CompilerServices;

namespace Riffle.Models
{
    public abstract class Room
    {
        public string JoinCode { get; }

        public string HostConnectionId { get; }

        public GameType Game { get; }

        public Room(string hostConnectionId, GameType game)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(hostConnectionId, nameof(hostConnectionId));

            string? code = Utilities.JoinCode.Create();
            if (code == null) throw new TooManyRoomsException();
            JoinCode = code;

            HostConnectionId = hostConnectionId;
            Game = game;
        }

        public abstract void RemoveMember(string connectionId);

        public abstract void AddMember(string connectionId, string name);

        public abstract IReadOnlyCollection<RoomMember> GetMembers();

        public abstract bool IsFull();

        public abstract bool StartGame();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public virtual async Task StringMsg(string connId, IHubCallerClients clients, string msgName, string msgContent)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return;
        }

    }
}
