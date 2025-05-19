// Copyright 2025 Roman Lefler

using Riffle.Utilities;

namespace Riffle.Models
{
    public class Room
    {

        public string JoinCode { get; }

        public string HostConnectionId { get; }

        public GameType Game { get; }

        public HashSet<string> Participants { get; }

        public Room(string hostConnectionId, GameType game)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(hostConnectionId, nameof(hostConnectionId));

            string? code = Utilities.JoinCode.Create();
            if (code == null) throw new TooManyRoomsException();
            JoinCode = code;

            HostConnectionId = hostConnectionId;
            Game = game;
            Participants = [];
        }
    }
}
