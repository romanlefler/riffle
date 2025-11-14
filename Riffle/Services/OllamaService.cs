using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using NuGet.Protocol;

namespace Riffle.Services
{
    public class OllamaService
    {
        private const string MODEL = "llama3.1";
        public const string USER = "user";
        public const string SYSTEM = "system";
        public const string ASSISTANT = "assistant";

        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;

        public OllamaService(HttpClient http, IOptions<BuildCfg> cfg)
        {
            _http = http;
            _http.BaseAddress = new Uri(cfg.Value.OllamaUri);
            _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        public readonly struct ChatMessage
        {
            public required string Role { get; init; }
            public required string Content { get; init; }

            public static ChatMessage User(string msg) => new() { Role = USER, Content = msg };
            public static ChatMessage System(string msg) => new() { Role = SYSTEM, Content = msg };
            public static ChatMessage Assistant(string msg) => new() { Role = ASSISTANT, Content = msg };
        }

        public async Task<string> ChatAsync(
            string prompt,
            double temp,
            CancellationToken tok = default
        )
        {
            return await ChatAsync([ new ChatMessage() { Role = "user", Content = prompt } ], temp, tok);
        }

        public async Task<string> ChatAsync(
            ChatMessage[] msgs,
            double temp,
            CancellationToken tok = default
        )
        {
            var body = new
            {
                model = MODEL,
                stream = false,
                options = new { temperature = temp, num_predict = 256 },
                messages = msgs
            };

            using var msg = new HttpRequestMessage(HttpMethod.Post, "api/chat")
            {
                Content = new StringContent(JsonSerializer.Serialize(body, _json),
                                                    Encoding.UTF8,
                                                    "application/json")
            };

            using var resp = await _http.SendAsync(msg, tok);
            resp.EnsureSuccessStatusCode();

            using var stream = await resp.Content.ReadAsStreamAsync(tok);
            var json = await resp.Content.ReadAsStringAsync(tok);
            Console.WriteLine(json);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: tok);

            if (doc.RootElement.TryGetProperty("error", out var err))
            {
                throw new InvalidOperationException(err.GetString());
            }
            return doc.RootElement
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? throw new Exception("AI response doesn't contain content: " + doc.ToJson());
        }
    }
}
