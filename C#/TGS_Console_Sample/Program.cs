using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TGS_Console_Sample
{
    internal static class Program
    {
        private const string SceneLayerPath = "3dms/I3S/building/i3s/SceneServer/layers/0";

        private static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            TgsApiOptions options = LoadOptions();

            // I3S resources are served gzip-compressed.
            using var httpClient = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All
            })
            {
                BaseAddress = new Uri(options.BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
            httpClient.DefaultRequestHeaders.Add("tgs-api-key", options.ApiKey);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            Console.WriteLine($"呼叫 TGS API: {new Uri(httpClient.BaseAddress, SceneLayerPath)}");
            using HttpResponseMessage response = await httpClient.GetAsync(SceneLayerPath, cts.Token);
            string content = await response.Content.ReadAsStringAsync(cts.Token);

            Console.WriteLine($"HTTP 狀態碼: {(int)response.StatusCode}");
            Console.WriteLine("\nAPI 回應內容:");
            Console.WriteLine(FormatJson(content));
        }

        private static TgsApiOptions LoadOptions()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            JObject settings = JObject.Parse(File.ReadAllText(path));
            TgsApiOptions options = settings["TgsApi"]?.ToObject<TgsApiOptions>()
                ?? throw new InvalidOperationException("appsettings.json 缺少 TgsApi 區段。");

            if (string.IsNullOrWhiteSpace(options.BaseUrl) || string.IsNullOrWhiteSpace(options.ApiKey))
            {
                throw new InvalidOperationException("appsettings.json 的 TgsApi:BaseUrl 或 TgsApi:ApiKey 未設定。");
            }

            return options;
        }

        private static string FormatJson(string content)
        {
            try
            {
                return JToken.Parse(content).ToString(Formatting.Indented);
            }
            catch (JsonReaderException)
            {
                return content;
            }
        }
    }

    public sealed class TgsApiOptions
    {
        public string BaseUrl { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;
    }
}
