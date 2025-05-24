using Riffle.Models.Games;
using Riffle.Utilities;

namespace RiffleTest
{
    public class Tests
    {
        [Test]
        public void TestRankScore()
        {
            List<RoundaboutMember> members =
            [
                new RoundaboutMember("123456789", "Jimmy"),
                new RoundaboutMember("abcdefGHI", "xxcraigxx"),
                new RoundaboutMember("ZXCVBNMQW", "ayo37"),
                new RoundaboutMember("lkjhgfdsa", "jeff997")
            ];

            members[0].Score = 5;
            members[1].Score = 999;
            members[2].Score = 100;
            members[3].Score = 1500;

            RankMembers.RankScore(members, out string[] connIds, out long[] scores);

            string[] expectedConns = [ "lkjhgfdsa", "abcdefGHI", "ZXCVBNMQW", "123456789" ];
            long[] expectedScores = [ 1500, 999, 100, 5 ];
            for (int i = 0; i < members.Count; i++)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(scores[i], Is.EqualTo(expectedScores[i]));
                    Assert.That(connIds[i], Is.EqualTo(expectedConns[i]));
                });
            }

        }
    }
}
