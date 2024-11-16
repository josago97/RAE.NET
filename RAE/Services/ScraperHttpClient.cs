using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

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
            DefaultRequestVersion = HttpVersion.Version20;
        }
    }
}
