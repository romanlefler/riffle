using Riffle.Utilities;

namespace Riffle.Models.Games
{
    public class RoundaboutRoom : Room
    {
        private const int MAX_PLAYER_COUNT = 8;

        private readonly List<RoundaboutMember> members;

        public RoundaboutRoom(string hostConnId) : base(hostConnId, GameType.Roundabout)
        {
            members = new(8);
        }

        public override void AddMember(string connectionId, string name)
        {
            RoundaboutMember m = new RoundaboutMember(connectionId, name);
            members.Add(m);
        }
        public override void RemoveMember(string connectionId)
        {
            members.RemoveAll(m => m.ConnectionId == connectionId);
        }

        public override bool IsFull()
        {
            return members.Count >= MAX_PLAYER_COUNT;
        }

    }
}
