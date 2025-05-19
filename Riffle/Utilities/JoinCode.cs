using Riffle.Services;

namespace Riffle.Utilities
{
    public static class JoinCode
    {

        private static readonly int LEN = 6;
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

        public static string? Create()
        {
            string code;
            int tries = 0;
            do
            {
                if (++tries > 500) return null;
                code = GenerateCode();
            }
            while(RoomManager.Rooms.ContainsKey(code));

            return code;
        }

    }
}
