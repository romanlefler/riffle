using Riffle.Services;

namespace Riffle.Utilities
{
    public static class JoinCode
    {

        private const int LEN = 6;
        private static readonly char[] validChars =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ23456789".ToCharArray();

        private static string GenerateCode()
        {
            char[] result = new char[LEN];
            byte[] buffer = new byte[LEN];
            Random.Shared.NextBytes(buffer);

            for(int i = 0; i < LEN; i++)
            {
                int charInx = buffer[i] % validChars.Length;
                result[i] = validChars[charInx];
            }

            return new string(result);
        }

        public static string? Create(BadWordService? badWordService)
        {
            string code;
            int tries = 0;

        TryAgain:
            if (++tries > 500) return null;
            code = GenerateCode();

            if (RoomManager.Rooms.ContainsKey(code)) goto TryAgain;
            if (badWordService != null && badWordService.ContainsBad(code)) goto TryAgain;

            return code;
        }

        public static string NormalizeCode(string joinCode)
        {
            string newCode = joinCode.Trim();
            newCode = newCode.Replace('0', 'O');
            newCode = newCode.Replace('1', 'I');
            newCode = newCode.ToUpper();
            return newCode;
        }

        public static bool IsValidCode(string joinCode)
        {
            return joinCode.Length == LEN && joinCode.All(c => validChars.Contains(c));
        }

    }
}
