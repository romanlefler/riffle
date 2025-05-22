namespace Riffle.Utilities
{
    public static class StringDistance
    {
        /// <summary>
        /// Computes the number of edits needed to change a string to another.
        /// </summary>
        /// <returns>
        /// The amount of characters that must be changed.
        /// </returns>
        public static int ComputeLevenshtein(string s, string t)
        {
            if(string.IsNullOrEmpty(s))
            {
                return string.IsNullOrEmpty(t) ? 0 : t.Length;
            }
            if (string.IsNullOrEmpty(t)) return s.Length;

            int n = s.Length;
            int m = t.Length;

            int[,] mat = new int[n + 1, m + 1];

            for (int i = 0; i <= n; i++) mat[i, 0] = i;
            for (int j = 0; j <= m; j++) mat[0, j] = j;

            for(int i = 1; i <= n; i++)
            {
                for(int j = 1; j <= m; j++)
                {
                    int del = mat[i - 1, j] + 1;
                    int ins = mat[i, j - 1] + 1;

                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int sub = mat[i - 1, j - 1] + cost;

                    mat[i, j] = Math.Min(Math.Min(del, ins), sub);
                }
            }

            return mat[n, m];
        }
    }
}
