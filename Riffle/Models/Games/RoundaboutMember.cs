namespace Riffle.Models.Games
{
    public class RoundaboutMember : RoomMember, IScoreMember
    {

        public long Score { get; set; }

        public string? SecretWord { get; set; }

        public RoundaboutMember(string connId, string name) : base(connId, name)
        {
            Score = 0;
            SecretWord = null;
        }

    }
}
