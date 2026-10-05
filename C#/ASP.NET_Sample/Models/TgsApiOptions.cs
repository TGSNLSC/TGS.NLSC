using System.ComponentModel.DataAnnotations;

namespace ASP.NET_Sample.Models
{
    public class TgsApiOptions
    {
        public const string SectionName = "TgsApi";
        public const string HttpClientName = "TgsApi";

        [Required, Url]
        public string BaseUrl { get; set; } = string.Empty;

        [Required]
        public string ApiKey { get; set; } = string.Empty;
    }
}
