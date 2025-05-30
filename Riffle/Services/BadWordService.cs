using Ganss.Text;
using Microsoft.IdentityModel.Tokens;
using Riffle.Utilities;
using System.Reflection;
using System.Text;

namespace Riffle.Services
{
    public class BadWordService
    {

        private readonly AhoCorasick _full;
        private readonly AhoCorasick _sixChars;

        public BadWordService()
        {
            string[] fullList = ReadBase64Resource("Riffle.Resources.badWords.txt.base64");
            _full = new AhoCorasick(fullList);

            string[] shortWordsList = ReadBase64Resource("Riffle.Resources.badWords6Chars.txt.base64");
            _sixChars = new AhoCorasick(shortWordsList);
        }

        public bool ContainsBad(string s)
        {
            string norm = s.ToLower().Replace("1", "i").Replace("3", "e").Replace("0", "o");
            if (s.Length <= 6) return !_sixChars.Search(norm).IsNullOrEmpty();
            else return !_full.Search(norm).IsNullOrEmpty();
        }

        private static string[] ReadBase64Resource(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using Stream? s = asm.GetManifestResourceStream(resourceName);
            _ = s ?? throw new Exception($"Missing embedded resource {resourceName}.");

            using StreamReader sr = new(s);
            string base64 = sr.ReadToEnd();
            byte[] binary = Convert.FromBase64String(base64);
            string lines = Encoding.UTF8.GetString(binary);
            return lines.Split();
        }

    }
}
