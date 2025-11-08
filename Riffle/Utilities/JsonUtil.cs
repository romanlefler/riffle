using System.Text.Json;

namespace Riffle.Utilities
{
    public static class JsonUtil
    {
        public static T? TryDeserialize<T>(string s)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(s);
            }
            catch(JsonException)
            {
                return default;
            }
        }
    }
}