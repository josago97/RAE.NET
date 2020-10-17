using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RAE
{
/*
    Based on information obtained from:
    Basado en la información obtenida de:
    https://devhub.io/repos/mgp25-RAE-API
*/
    internal class RAEAPI
    {
        private const string URLBASE = "https://dle.rae.es/data";
        private const string TOKEN = "cDY4MkpnaFMzOmFHZlVkQ2lFNDM0";

        private HttpClient _httpClient;

        public RAEAPI()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", TOKEN);
        }

        public async Task<string[]> FetchWordByIdAsync(string wordId)
        {
            var response = await _httpClient.GetStringAsync($"{URLBASE}/fetch?id={wordId}");

            MatchCollection matches = Regex.Matches(response, "<p class=\"(?:j|m)\".*?>.*?</p>");

            string[] definitions = matches.Cast<Match>()
                                          .Select(m => Regex.Replace(m.Value, "<.*?>", ""))
                                          .ToArray();

            return definitions;
        }

        public async Task<string[]> GetKeysAsync(string query)
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/keys?q={query}&callback=");
            string json = Regex.Match(response, @"\[.*?\]").Value;

            var keys = JsonConvert.DeserializeObject<string[]>(json);

            return keys;
        }

        public async Task<Word> GetRandomWordAsync()
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/random");

            string idFormat = "article id=\"";
            Match idMatch = Regex.Match(response, $@"{idFormat}\w+");
            string id = Regex.Replace(idMatch.Value, idFormat, "");

            string contentFormat = "class=\"f\".*?>";
            Match contentMatch = Regex.Match(response, $@"{contentFormat}\w[\,\s\w]*");
            string content = Regex.Replace(contentMatch.Value, contentFormat, "");

            return new Word(id, content);
        }

        public async Task<Word> GetWordOfTheDayAsync()
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/wotd?callback=");
            string json = response.Substring(1, response.Length - 2);
            JObject jobject = JObject.Parse(json);

            string id = jobject.Value<string>("id");
            string content = jobject.Value<string>("header");

            return new Word(id, content);
        }

        public async Task<List<Word>> SearchWordAsync(string word, bool allGroups = true)
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/search?w={word}");
            JToken jtoken = JToken.Parse(response);

            var words = new List<Word>();
            foreach (var w in jtoken.SelectToken("res"))
            {
                int group = w.Value<int>("grp");

                if (allGroups || group == 0)
                {
                    string id = w.Value<string>("id");
                    string content = w.Value<string>("header");

                    Match contentMatch = Regex.Match(content, @"[A-Za-z/-]+");

                    words.Add(new Word(id, contentMatch.Success ? contentMatch.Value : content));
                }
            }

            return words;
        }
    }
}
