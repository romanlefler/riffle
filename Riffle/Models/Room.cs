// Copyright 2025 Roman Lefler

using Riffle.Utilities;

namespace Riffle.Models
{
    public class Room
    {

        public string JoinCode { get; }

        public string HostConnectionId { get; }

        public GameType Game { get; }

        public HashSet<RoomMember> Participants { get; }

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

        public void RemoveParticipant(string connectionId)
        {
            RoomMember member = new RoomMember(connectionId, "");
            // RoomMembers with the same connectionId all have the same hash and are equal
            Participants.Remove(member);
        }

        public int GetMaxPlayerCount()
        {
            switch(Game)
            {
                case GameType.Roundabout:
                    return 8;
                default:
                    return 1;
            }
        }
        
        public bool IsFull()
        {
            return Participants.Count >= GetMaxPlayerCount();
        }

    }
}
