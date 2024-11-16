using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RAE.Models;

namespace RAE.Services
{
    /*
        Based on information obtained from:
        Basado en la información obtenida de:
        https://devhub.io/repos/mgp25-RAE-API
        https://github.com/mgp25/RAE-API
    */
    internal partial class RAEAPI
    {
        private const string URLBASE = "https://dle.rae.es/data";
        private const string TOKEN = "cDY4MkpnaFMzOmFHZlVkQ2lFNDM0";

        private HttpClient _httpClient;

        public RAEAPI()
        {
            _httpClient = new ScraperHttpClient(TOKEN);
        }

        public async Task<IWord> FetchWordByIdAsync(string wordId)
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/fetch?id={wordId}");

            return GetWord(response);
        }

        public async Task<string[]> GetKeysAsync(string query)
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/keys?q={query}&callback=");
            string json = Regex.Match(response, @"\[.*?\]").Value;

            string[] keys = JsonSerializer.Deserialize<string[]>(json);

            return keys;
        }

        public async Task<IWord> GetRandomWordAsync()
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/random");

            return GetWord(response);
        }

        public async Task<IEntry> GetWordOfTheDayAsync()
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/wotd?callback=");
            JsonElement element = JsonDocument.Parse(response).RootElement;

            string id = element.GetProperty("id").GetString();
            string content = element.GetProperty("header").GetString();

            return new Entry(id, HtmlDecode(content));
        }

        public async Task<IEntry[]> SearchWordAsync(string word, bool allGroups = true)
        {
            string response = await _httpClient.GetStringAsync($"{URLBASE}/search?w={word}");
            JsonElement element = JsonDocument.Parse(response).RootElement;
            List<IEntry> words = new List<IEntry>();

            foreach (JsonElement jWord in element.GetProperty("res").EnumerateArray())
            {
                int group = jWord.GetProperty("grp").GetInt32();

                if (allGroups || group == 0)
                {
                    string id = jWord.GetProperty("id").GetString();
                    string content = jWord.GetProperty("header").GetString();
                    Match contentMatch = Regex.Match(content, @"\b[\p{L}\p{M}/-]+");
                    if (contentMatch.Success) content = contentMatch.Value;

                    words.Add(new Entry(id, content));
                }
            }

            return words.ToArray();
        }
    }
}
