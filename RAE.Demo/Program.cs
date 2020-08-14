using System;
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
            FetchRandomWorldAsync
        };

        static async Task Main()
        {
            drae = new DRAE();

            for(int i = 0; i < functions.Length; i++)
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
            var query = "hola";
            var keys = await drae.GetKeysAsync(query);

            Console.WriteLine($"GetKeys ({query}): {string.Join(", ", keys)}");
        }

        static async Task SearchWordAsync()
        {
            var query = "a";
            var words = await drae.SearchWordAsync(query, false);

            Console.WriteLine($"SearchWord ({query}): {string.Join(", ", words.Select(w => $"{w.Content} ({w.Id})"))}");
        }

        static async Task WordOfTheDayAsync()
        {
            var word = await drae.GetWordOfTheDayAsync();

            Console.WriteLine($"Word of the day: {word} ({word.Id})");
        }

        static async Task GetRandomWorldAsync()
        {
            var word = await drae.GetRandomWordAsync();

            Console.WriteLine($"A random word: {word}");
        }

        static async Task FetchRandomWorldAsync()
        {
            Word word = await drae.GetRandomWordAsync();
            string[] definitions = await drae.FetchWordByIdAsync(word.Id);

            Console.WriteLine($"Definitions of {word.Content}:");
            Array.ForEach(definitions, Console.WriteLine);
        }
    }
}
