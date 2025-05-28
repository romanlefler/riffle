using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Text.Json;

using StrDict = System.Collections.Generic.Dictionary<string, string>;
using DblStrDict = System.Collections.Generic.Dictionary<string,
    System.Collections.Generic.Dictionary<string, string>>;

namespace Riffle.Utilities
{
    public class AssetMapService
    {
        private const string FILE_NAME = "viteMap.json";
        private readonly string _fileDir;
        private readonly string _fileFullPath;
        private readonly ILogger<AssetMapService> _logger;

        public string RoundaboutHostJs { get; private set; } = "";
        private string _roundaboutHostCss = "";
        public string RoundaboutHostCss { get => _roundaboutHostCss; }

        public string RoundaboutMemberJs { get; private set; } = "";
        private string _roundaboutMemberCss = "";
        public string RoundaboutMemberCss { get => _roundaboutMemberCss; }

        public AssetMapService(ILogger<AssetMapService> logger)
        {
            _logger = logger;
            // TODO: In release mode the file should be embedded
            _fileDir = Path.Combine(AppContext.BaseDirectory, "Resources");
            _fileFullPath = Path.Combine(_fileDir, FILE_NAME);
            DebugWatch();
            DebugLoad();
        }

        private void DebugWatch()
        {
            PhysicalFileProvider prov = new(_fileDir)
            {
                UsePollingFileWatcher = false
            };
            IChangeToken tok = prov.Watch(FILE_NAME);
            tok.RegisterChangeCallback(DebugFileChanged, null);
        }

        private async void DebugFileChanged(object? _)
        {
            await Task.Delay(1500);
            _logger.LogInformation("Detected Vite Map change; remapping files.");
            DebugLoad();
            DebugWatch();
        }

        private static string GetMappedFile(JsonElement root, string name, ref string css)
        {
            JsonElement fileObj = root.GetProperty(name);
            JsonElement valProp = fileObj.GetProperty("file");
            string? nameVal = valProp.GetString();
            _ = nameVal ?? throw new InvalidOperationException("file property not a string.");

            if (fileObj.TryGetProperty("css", out JsonElement cssProp))
            {
                JsonElement firstItem = cssProp[0];
                css = $"dist/{firstItem.GetString()}";
            }
            else css = "";

            return $"dist/{nameVal}";
        }

        private void DebugLoad()
        {
            using FileStream fs = File.Open(_fileFullPath, FileMode.Open);
            using JsonDocument doc = JsonDocument.Parse(fs);
            JsonElement root = doc.RootElement;

            RoundaboutHostJs = GetMappedFile(root, "src/host/roundaboutHost.ts",
                ref _roundaboutHostCss);

            RoundaboutMemberJs = GetMappedFile(root, "src/member/roundaboutMember.ts",
                ref _roundaboutMemberCss);

        }

    }
}
