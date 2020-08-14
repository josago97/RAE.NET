using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
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
    public class DRAE
    {
        private const string URLBASE = "https://dle.rae.es/data";
        private const string TOKEN = "cDY4MkpnaFMzOmFHZlVkQ2lFNDM0";

        private HttpClient _httpClient;

        public DRAE()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", TOKEN);
        }

        /// <summary>
        /// <para>Get definitions of a word.</para>
        /// <para>Obtiene las definiciones de una palabra.</para>
        /// </summary>
        /// <param name="wordId">
        /// <para>The id of the word to search.</para>
        /// <para>El id de la palabra a buscar.</para>
        /// </param>
        public async Task<string[]> FetchWordByIdAsync(string wordId)
        {
            var response = await _httpClient.GetStringAsync($"{URLBASE}/fetch?id={wordId}");

            MatchCollection matches = Regex.Matches(response, "<p class=\"(?:j|m)\".*?>.*?</p>");

            string[] definitions = matches.Cast<Match>()
                                          .Select(m => Regex.Replace(m.Value, "<.*?>", ""))
                                          .ToArray();

            return definitions;
        }

        /// <summary>
        /// <para>Get keywords from another.</para>
        /// <para>Obtiene palabras claves a partir de otra.</para>
        /// </summary>
        /// <param name="query">
        /// <para>The base word.</para>
        /// <para>La palabra base.</para>
        /// </param>
        public async Task<List<string>> GetKeysAsync(string query)
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/keys?q={query}&callback=");
            string json = Regex.Match(response, @"\[.*?\]").Value;

            var keys = JsonConvert.DeserializeObject<List<string>>(json);

            return keys;
        }

        /// <summary>
        /// <para>Get a random word.</para>
        /// <para>Obtiene una palabra aleatoria.</para>
        /// </summary>
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

        /// <summary>
        /// <para>Get the word of the day.</para>
        /// <para>Obtiene la palabra del día.</para>
        /// </summary>
        public async Task<Word> GetWordOfTheDayAsync()
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/wotd?callback=");
            string json = response.Substring(1, response.Length - 2);
            JObject jobject = JObject.Parse(json);

            string id = jobject.Value<string>("id");
            string content = jobject.Value<string>("header");

            return new Word(id, content);
        }

        /// <summary>
        /// <para>Search all entries of a word.</para>
        /// <para>Busca todas las entradas de una palabra.</para>
        /// </summary>
        /// <param name="word">
        /// <para>Word to search.</para>
        /// <para>Palabra a buscar.</para>
        /// </param>
        /// <param name="allGroups">
        /// <para>If true it will take secondary entries.</para>
        /// <para>Si es verdadero cogerá entradas secundarias.</para>
        /// </param>
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
