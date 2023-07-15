using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAE.Demo
{
    class Program
    {
        static DRAE drae;

        static Func<Task>[] functions =
        {
            GetKeysAsync,
            SearchWordAsync,
            WordOfTheDayAsync,
            FetchRandomWorldAsync,
            GetRandomWorldAsync,
            GetWordsStartWithAsync,
            GetWordsContainAsync,
            GetAllWordsAsync
        };

        static async Task Main()
        {
            drae = new DRAE();

            for (int i = 0; i < functions.Length; i++)
            {
                await functions[i]();
                Console.WriteLine();
                Console.WriteLine(new string('-', 20));
                Console.WriteLine();
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        static async Task GetKeysAsync()
        {
            string query = "w";
            string[] keys = await drae.GetKeysAsync(query);

            Console.WriteLine($"GetKeys ({query}): {string.Join(", ", keys)}");
        }

        static async Task SearchWordAsync()
        {
            string query = "a";
            IList<Word> words = await drae.SearchWordAsync(query, false);

            Console.WriteLine($"SearchWord ({query}): {string.Join(", ", words.Select(w => $"{w.Content} ({w.Id})"))}");
        }

        static async Task WordOfTheDayAsync()
        {
            Word word = await drae.GetWordOfTheDayAsync();

            Console.WriteLine($"Word of the day: {word} ({word.Id})");
        }

        static async Task GetRandomWorldAsync()
        {
            Word word = await drae.GetRandomWordAsync();

            Console.WriteLine($"A random word: {word}");
        }

        static async Task FetchRandomWorldAsync()
        {
            Word word = await drae.GetRandomWordAsync();
            string[] definitions = await drae.FetchWordByIdAsync(word.Id);

            Console.WriteLine($"Definitions of {word.Content}:");
            Array.ForEach(definitions, Console.WriteLine);
        }

        static async Task GetWordsStartWithAsync()
        {
            string character = "A";
            string[] words = await drae.GetWordsStartWithAsync(character);

            Console.WriteLine($"There are {words.Length} words in the spanish dictionary that start with '{character}'");
        }

        static async Task GetWordsContainAsync()
        {
            string character = "A";
            string[] words = await drae.GetWordsContainAsync(character);

            Console.WriteLine($"There are {words.Length} words in the spanish dictionary that contain '{character}'");
        }

        static async Task GetAllWordsAsync()
        {
            string[] allWords = await drae.GetAllWordsAsync();

            Console.WriteLine($"There are {allWords.Length} words in the spanish dictionary");
        }
    }
}
