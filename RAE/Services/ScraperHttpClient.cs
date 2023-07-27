using System.Net.Http;
using System.Net.Http.Headers;

namespace RAE.Services
{
    internal class ScraperHttpClient : HttpClient
    {
        public ScraperHttpClient() : base()
        {
            DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
        }

        public ScraperHttpClient(string token) : this()
        {
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
        }
    }
}
