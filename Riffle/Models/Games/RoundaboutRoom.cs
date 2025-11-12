using Microsoft.AspNetCore.SignalR;
using Riffle.Services;
using Riffle.Utilities;

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

        private readonly List<string> _sentences;

        private string? _wipSentBase;

        private string[][]? _wipSentOptions;

        private string? _secretWord;

        private CancellationTokenSource? _sentenceWait;

        private RoundaboutMember MemUp { get => _members[_userUp]; }

        public RoundaboutRoom(BadWordService badWordService, string hostConnId) :
            base(badWordService, hostConnId, GameType.Roundabout)
        {
            _stage = Stage.Lobby;
            _members = new(8);
            _sentences = new(16);
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

            _sentences.Clear();
            GenSentenceInfo();
        }

        private void GenSentenceInfo()
        {
            _wipSentBase = "It's a {0} {1}.";
            _wipSentOptions = [ [ "colorful", "dusty" ], [ "experience", "spot" ] ];
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

        private void NewSentence(string sentence)
        {
            _sentences.Add(sentence);
            GenSentenceInfo();
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
                        await Task.WhenAll(
                            clients.GroupExcept(JoinCode, [ MemUp.ConnectionId ]).SendAsync("GuessingStarted"),
                            clients.Client(MemUp.ConnectionId).SendAsync("SentenceOptions", _wipSentBase, _wipSentOptions)
                        );
                    }
                    return;
                case "SelSentence":
                    // Must be in guessing stage
                    if (m is null || _stage != Stage.GuessWord) return;
                    // Basic NULL checks
                    if (_wipSentOptions is null || _wipSentBase is null) return;
                    // Only user who's up can use this
                    if (m != MemUp) return;

                    int[]? indices = JsonUtil.TryDeserialize<int[]>(msgContent);
                    if (indices is null || _wipSentOptions.Length != indices.Length) return;

                    // Someone's trying to send before the wait is up
                    if (_sentenceWait != null) return;
                    _sentenceWait = new CancellationTokenSource();

                    string[] choices = new string[indices.Length];
                    for(int i = 0; i < indices.Length; i++)
                    {
                        int d = indices[i];
                        string[] opts = _wipSentOptions[i];
                        if (d is < 0 || d >= opts.Length) return;
                        choices[i] = opts[d];
                    }
                    string sentence;
                    try
                    {
                        sentence = string.Format(_wipSentBase, choices);
                    }
                    catch(FormatException)
                    {
                        _sentenceWait.Dispose();
                        _sentenceWait = null;
                        sentence = "An error occurred.";
                        return;
                    }
                    NewSentence(sentence);
                    await clients.Group(JoinCode).SendAsync("SentenceSelected", sentence);

                    // Player has to wait 4 seconds before submitting another sentence
                    bool succ = await AsyncUtil.TaskDelay(4000, _sentenceWait.Token);
                    _sentenceWait.Dispose();
                    _sentenceWait = null;
                    if(succ) await clients.Client(MemUp.ConnectionId).SendAsync("SentenceOptions", _wipSentBase, _wipSentOptions);
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
                                    // Cancel the sentence gen process (it will dispose and take care of it)
                                    _sentenceWait?.Cancel();
                                    // 7 second delay for client animations
                                    await Task.Delay(7000);

                                    await Task.WhenAll(
                                        clients.GroupExcept(JoinCode, [ MemUp.ConnectionId ]).SendAsync("GuessingStarted"),
                                        clients.Client(MemUp.ConnectionId).SendAsync("SentenceOptions", _wipSentBase, _wipSentOptions)
                                    );
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
