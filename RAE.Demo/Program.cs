﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAE.Demo
{
    class Program
    {
        static DRAE drae;

        static Func<Task>[] functions =
        {
            FetchWordAsync,
            GetKeysAsync,
            SearchWordAsync,
            GetWordOfTheDayAsync,
            GetRandomWorldAsync,
            GetWordsStartWithAsync,
            GetWordsContainAsync,
            GetAllWordsAsync
        };

        static async Task Main()
        {
            drae = new DRAE();

            await drae.FetchWordByIdAsync("7lsKMtR");

            foreach (Func<Task> function in functions)
            {
                await function.Invoke();
                Console.WriteLine();
                Console.WriteLine(new string('-', 20));
                Console.WriteLine();
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        static async Task FetchWordAsync()
        {
            string[] wordsToFetch = { "y", "tonto", "niño", "en", "manada" };

            foreach (string wordToFetch in wordsToFetch)
            {
                IList<IWord> words = await drae.FetchWordAsync(wordToFetch);

                foreach (IWord word in words)
                {
                    Console.WriteLine(word);
                }

                Console.WriteLine();
            }
        }

        static async Task GetKeysAsync()
        {
            string query = "hola";
            string[] keys = await drae.GetKeysAsync(query);

            Console.WriteLine($"GetKeys ({query}): {string.Join(", ", keys)}");
        }

        static async Task SearchWordAsync()
        {
            string query = "a";
            IEntry[] entries = await drae.SearchWordAsync(query, false);

            Console.WriteLine($"SearchWord ({query}): {string.Join<IEntry>(", ", entries)}");
        }

        static async Task GetWordOfTheDayAsync()
        {
            IEntry word = await drae.GetWordOfTheDayAsync();

            Console.WriteLine($"Word of the day: {word} ({word.Id})");
        }

        static async Task GetRandomWorldAsync()
        {
            IWord word = await drae.GetRandomWordAsync();

            Console.WriteLine($"A random word: {word}");
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
