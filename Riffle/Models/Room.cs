// Copyright 2025 Roman Lefler

using Riffle.Utilities;

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

        public abstract bool IsFull();

    }
}
