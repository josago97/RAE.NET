using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RAE
{
    /*
        Based on information obtained from:
        Basado en la información obtenida de:
        https://devhub.io/repos/mgp25-RAE-API
        https://github.com/mgp25/RAE-API
    */
    internal class RAEAPI
    {
        private const string URLBASE = "https://dle.rae.es/data";
        private const string TOKEN = "cDY4MkpnaFMzOmFHZlVkQ2lFNDM0";

        private HttpClient _httpClient;

        public RAEAPI()
        {
            _httpClient = new ScraperHttpClient(TOKEN);
        }

        public async Task<string[]> FetchWordByIdAsync(string wordId)
        {
            var response = await _httpClient.GetStringAsync($"{URLBASE}/fetch?id={wordId}");

            MatchCollection matches = Regex.Matches(response, "<p class=\"(?:j|m)\".*?>.*?</p>");

            string[] definitions = matches.Cast<Match>()
                                          .Select(match => Regex.Replace(match.Value, "<.*?>", ""))
                                          .Select(HtmlDecode)
                                          .ToArray();

            return definitions;
        }

        public async Task<string[]> GetKeysAsync(string query)
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/keys?q={query}&callback=");
            string json = Regex.Match(response, @"\[.*?\]").Value;

            string[] keys = JsonConvert.DeserializeObject<string[]>(json);

            return keys;
        }

        public async Task<Word> GetRandomWordAsync()
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/random");
            XmlDocument xml = new XmlDocument();
            xml.LoadXml($"<root>{response}</root>");

            string id = xml.DocumentElement.SelectSingleNode("//article").Attributes["id"].Value;
            string content = xml.DocumentElement.SelectSingleNode("//header").InnerText;

            return new Word(id, content);
        }

        public async Task<Word> GetWordOfTheDayAsync()
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/wotd?callback=");
            JObject jobject = JObject.Parse(response);

            string id = jobject.Value<string>("id");
            string content = jobject.Value<string>("header");

            return new Word(id, HtmlDecode(content));
        }

        public async Task<List<Word>> SearchWordAsync(string word, bool allGroups = true)
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/search?w={word}");
            JToken jtoken = JToken.Parse(response);
            List<Word> words = new List<Word>();

            foreach (JToken jWord in jtoken.SelectToken("res"))
            {
                int group = jWord.Value<int>("grp");

                if (allGroups || group == 0)
                {
                    string id = jWord.Value<string>("id");
                    string content = jWord.Value<string>("header");
                    Match contentMatch = Regex.Match(content, @"\b[\p{L}\p{M}/-]+");
                    if (contentMatch.Success) content = contentMatch.Value;

                    words.Add(new Word(id, content));
                }
            }

            return words;
        }

        private string HtmlDecode(string content)
        {
            return WebUtility.HtmlDecode(content);
        }
    }
}
