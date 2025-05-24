using Riffle.Models;

namespace Riffle.Utilities
{
    public static class RankMembers
    {
        /// <summary>
        /// Ranks a list of members by their scores in descending order and extracts their connection IDs and scores.
        /// </summary>
        /// <remarks>The method orders the members by their scores in descending order. The resulting
        /// arrays of connection IDs and scores are aligned such that the connection ID at a given index corresponds to
        /// the score at the same index.</remarks>
        /// <typeparam name="T">The type of the members in the list. Must implement both <see cref="RoomMember"/> and <see
        /// cref="IScoreMember"/>.</typeparam>
        /// <param name="members">The list of members to rank. Each member must have a score and a connection ID.</param>
        /// <param name="connIds">When this method returns, contains an array of connection IDs corresponding to the ranked members.</param>
        /// <param name="scores">When this method returns, contains an array of scores corresponding to the ranked members.</param>
        public static void RankScore<T>(IList<T> members, out string[] connIds, out long[] scores)
            where T : RoomMember, IScoreMember
        {
            var sorted = from m in members orderby m.Score descending select m;
            connIds = [.. from m in sorted select m.ConnectionId ];
            scores = [.. from m in sorted select m.Score ];
        }
    }
}
