using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.SignalR;
using Riffle.Services;
using Riffle.Utilities;
using System.Text.RegularExpressions;

namespace Riffle.Models.Games
{
    public class RoundaboutRoom : Room
    {
        private const int MIN_PLAYER_COUNT = 2;
        private const int MAX_PLAYER_COUNT = 8;


        private readonly List<RoundaboutMember> _members;

        private Stage _stage;

        // The index of the user that's word is being guessed.
        private int _userUp;

        private string? _secretWord;

        private RoundaboutMember MemUp { get => _members[_userUp]; }

        public RoundaboutRoom(BadWordService badWordService, string hostConnId) :
            base(badWordService, hostConnId, GameType.Roundabout)
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

        public override bool StartGame()
        {
            if (_members.Count < MIN_PLAYER_COUNT) return false;

            _stage = Stage.ChooseWord;
            return true;
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
            SetUserUp(0);
            _stage = Stage.GuessWord;
        }

        private void SetUserUp(int userIndex)
        {
            _userUp = userIndex;
            string secret = MemUp.SecretWord ?? throw new InvalidOperationException("Secret word was null.");

            // This both makes all whitespace uniform (all spaces)
            // and makes gets rid of any back-to-back spaces
            _secretWord = NormString.NormalizeString(secret);
        }

        private bool TryGuess(string guess)
        {
            string guessNorm = NormString.NormalizeString(guess);
            _ = _secretWord ?? throw new InvalidOperationException("Secret word is null.");
            int tol = _secretWord.Length / 4;
            int dist = StringDistance.ComputeLevenshtein(guessNorm, _secretWord);

            return dist <= tol;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the round is over.</returns>
        private bool NextUser()
        {
            if (++_userUp >= _members.Count)
            {
                SetUserUp(0);
                return true;
            }
            SetUserUp(_userUp);
            return false;
        }

        public override async Task StringMsg(string connId, IHubCallerClients clients, string msgName, string msgContent)
        {
            RoundaboutMember? m = _members.Find(k => k.ConnectionId == connId);
            ISingleClientProxy host = clients.Client(HostConnectionId);
            ISingleClientProxy caller = clients.Client(connId);
            switch(msgName)
            {
                case "ChooseWord":
                    if (m is null || _stage != Stage.ChooseWord) return;
                    // Host cannot execute this
                    if (HostConnectionId == connId) return;

                    m.SecretWord = msgContent;
                    await host.SendAsync("UserChoseWord", connId);
                    await caller.SendAsync("ChoiceAccepted");

                    if (AllPlayersChose())
                    {
                        StartGuessing();
                        await clients.Group(JoinCode).SendAsync("GuessingStarted", MemUp.ConnectionId);
                    }
                    return;
                case "GuessWord":
                    if (m is null || _stage != Stage.GuessWord) return;
                    // Host cannot execute this
                    if (HostConnectionId == connId) return;
                    // User that's up cannot execute this
                    if (MemUp == m) return;

                    if (TryGuess(msgContent))
                    {
                        string original = MemUp.SecretWord ?? throw new InvalidOperationException();
                        await clients.Group(JoinCode).SendAsync("SuccessfulGuess", connId, original);
                        if (!NextUser())
                        {
                            // 7 second delay for client animations
                            await Task.Delay(7000);

                            await clients.Group(JoinCode).SendAsync("GuessingStarted", MemUp.ConnectionId);
                        }
                        else
                        {
                            RankMembers.RankScore(_members, out string[] connIds, out long[] scores);
                            await clients.Group(JoinCode).SendAsync("GameEnded", connIds, scores);
                        }
                    }
                    // If it's wrong nothing happens
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
