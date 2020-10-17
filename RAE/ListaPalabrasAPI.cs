using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RAE
{
    internal class ListaPalabrasAPI
    {
        private const string URLBASE = "https://www.listapalabras.com";
        private static readonly string[] ALPHABET = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "Ñ", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        private HttpClient _httpClient;

        public ListaPalabrasAPI()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string[]> GetAllWordsAsync()
        {
            List<string> words = new List<string>();

            await Task.Run(() => Array.ForEach(ALPHABET, s => words.AddRange(GetWordsStartWithAsync(s).Result)));

            return words.ToArray();
        }

        public async Task<string[]> GetWordsStartWithAsync(string query)
        {
            HtmlDocument page = await LoadPageAsync($"{URLBASE}/palabras-con.php?letra={query}&total=s");

            return GetWords(page);
        }

        public async Task<string[]> GetWordsContainAsync(string query)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("busqueda", query)
            });

            HtmlDocument page = await LoadPageAsync($"{URLBASE}/palabras-con.php", content);

            return GetWords(page);
        }

        private async Task<HtmlDocument> LoadPageAsync(string url)
        {
            var document = new HtmlDocument();

            using (Stream documentStream = await _httpClient.GetStreamAsync(url))
            {
                document.Load(documentStream);
            }

            return document;
        }

        private async Task<HtmlDocument> LoadPageAsync(string url, HttpContent content)
        {
            var document = new HtmlDocument();

            var response = await _httpClient.PostAsync(url, content);

            using (Stream documentStream = await response.Content.ReadAsStreamAsync())
            {
                document.Load(documentStream);
            }

            return document;
        }

        private string[] GetWords(HtmlDocument document)
        {
            string[] words = document.GetElementbyId("columna_resultados_generales")
                                     .SelectNodes("descendant::*[@id='palabra_resultado']")
                                     .Select(n => n.InnerText.Trim(' ', '\n'))
                                     .ToArray();

            return words;
        }
    }
}
