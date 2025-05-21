using Microsoft.AspNetCore.SignalR;
using Riffle.Utilities;

namespace Riffle.Models.Games
{
    public class RoundaboutRoom : Room
    {
        private const int MAX_PLAYER_COUNT = 8;

        private readonly List<RoundaboutMember> _members;

        private Stage _stage;

        public RoundaboutRoom(string hostConnId) : base(hostConnId, GameType.Roundabout)
        {
            _stage = Stage.Lobby;
            _members = new(8);
        }

        public override void AddMember(string connectionId, string name)
        {
            RoundaboutMember m = new RoundaboutMember(connectionId, name);
            _members.Add(m);
        }

        public override void RemoveMember(string connectionId)
        {
            _members.RemoveAll(m => m.ConnectionId == connectionId);
        }

        public override IReadOnlyCollection<RoomMember> GetMembers()
        {
            return _members.AsReadOnly();
        }

        public override bool IsFull()
        {
            return _members.Count >= MAX_PLAYER_COUNT;
        }

        public override void StartGame()
        {
            _stage = Stage.ChooseWord;
        }

        private bool AllPlayersChose()
        {
            foreach(RoundaboutMember m in _members)
            {
                if (m.SecretWord is null) return false;
            }
            return true;
        }

        private void StartGuessing()
        {
            _stage = Stage.GuessWord;
        }

        public override async Task StringMsg(string connId, IHubCallerClients clients, string msgName, string msgContent)
        {
            RoundaboutMember? m = _members.Find(k => k.ConnectionId == connId);
            ISingleClientProxy host = clients.Client(HostConnectionId);
            ISingleClientProxy caller = clients.Client(connId);
            switch(msgName)
            {
                case "ChooseWord":
                    if (m == null || _stage != Stage.ChooseWord) return;
                    m.SecretWord = msgContent;
                    await host.SendAsync("UserChoseWord", connId);
                    await caller.SendAsync("ChoiceAccepted");

                    if (AllPlayersChose())
                    {
                        StartGuessing();
                        await clients.Group(JoinCode).SendAsync("GuessingStarted");
                    }
                    return;
            }
        }

        private enum Stage
        {
            Lobby,
            ChooseWord,
            GuessWord
        }

    }
}
