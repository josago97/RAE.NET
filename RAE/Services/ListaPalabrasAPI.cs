using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace RAE.Services
{
    internal class ListaPalabrasAPI
    {
        private const string URLBASE = "https://www.listapalabras.com";
        private static readonly string[] ALPHABET = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "Ñ", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        private HttpClient _httpClient;

        public ListaPalabrasAPI()
        {
            _httpClient = new ScraperHttpClient();
        }

        public async Task<string[]> GetAllWordsAsync()
        {
            List<string> result = new List<string>();

            IEnumerable<Task<string[]>> tasks = ALPHABET.Select(GetWordsStartWithAsync);

            foreach (string[] words in await Task.WhenAll(tasks))
            {
                result.AddRange(words);
            }

            return result.ToArray();
        }

        public async Task<string[]> GetWordsStartWithAsync(string query)
        {
            HtmlDocument page = await LoadPageAsync($"{URLBASE}/palabras-con.php?letra={query}&total=s");

            return GetWords(page);
        }

        public async Task<string[]> GetWordsContainAsync(string query)
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("busqueda", query)
            });

            HtmlDocument page = await LoadPageAsync($"{URLBASE}/palabras-con.php", content);

            return GetWords(page);
        }

        private async Task<HtmlDocument> LoadPageAsync(string url)
        {
            HtmlDocument document = new HtmlDocument();

            using (Stream documentStream = await _httpClient.GetStreamAsync(url))
            {
                document.Load(documentStream);
            }

            return document;
        }

        private async Task<HtmlDocument> LoadPageAsync(string url, HttpContent content)
        {
            HtmlDocument document = new HtmlDocument();
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            using (Stream documentStream = await response.Content.ReadAsStreamAsync())
            {
                document.Load(documentStream);
            }

            return document;
        }

        private string[] GetWords(HtmlDocument document)
        {
            HtmlNode resultNode = document.GetElementbyId("columna_resultados_generales");
            HtmlNodeCollection wordNodes = resultNode.SelectNodes("descendant::*[@id='palabra_resultado']");

            return wordNodes == null 
                ? new string[0] 
                : wordNodes.Select(node => node.InnerText.Trim(' ', '\n')).ToArray();
        }
    }
}
