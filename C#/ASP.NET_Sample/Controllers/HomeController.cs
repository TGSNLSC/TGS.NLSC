using ASP.NET_Sample.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ASP.NET_Sample.Controllers
{
    public class HomeController : Controller
    {
        private const string SceneLayerPath = "3dms/I3S/building/i3s/SceneServer/layers/0";

        private readonly IHttpClientFactory _clientFactory;

        public HomeController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateClient(TgsApiOptions.HttpClientName);

            using var response = await client.GetAsync(SceneLayerPath, cancellationToken);
            ViewBag.StatusCode = (int)response.StatusCode;
            ViewBag.ApiResponse = FormatJson(await response.Content.ReadAsStringAsync(cancellationToken));

            return View();
        }

        private static string FormatJson(string content)
        {
            try
            {
                return JToken.Parse(content).ToString(Formatting.Indented);
            }
            catch (JsonReaderException)
            {
                // Non-JSON responses (e.g. error pages) are shown as-is.
                return content;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
