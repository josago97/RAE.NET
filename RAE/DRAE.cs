using System.Collections.Generic;
using System.Threading.Tasks;

namespace RAE
{
    public class DRAE
    {
        private RAEAPI _raeAPI;
        private ListaPalabrasAPI _listaPalabrasAPI;

        public DRAE()
        {
            _raeAPI = new RAEAPI();
            _listaPalabrasAPI = new ListaPalabrasAPI();
        }

        /// <summary>
        /// <para>Get definitions of a word.</para>
        /// <para>Obtiene las definiciones de una palabra.</para>
        /// </summary>
        /// <param name="wordId">
        /// <para>The id of the word to search.</para>
        /// <para>El id de la palabra a buscar.</para>
        /// </param>
        public Task<string[]> FetchWordByIdAsync(string wordId)
        {
            return _raeAPI.FetchWordByIdAsync(wordId);
        }

        /// <summary>
        /// <para>Get all the words that exist.</para>
        /// <para>Obtiene todas las palabras que existen.</para>
        /// </summary>
        public Task<string[]> GetAllWordsAsync()
        {
            return _listaPalabrasAPI.GetAllWordsAsync();
        }

        /// <summary>
        /// <para>Get keywords from another.</para>
        /// <para>Obtiene palabras claves a partir de otra.</para>
        /// </summary>
        /// <param name="query">
        /// <para>The base word.</para>
        /// <para>La palabra base.</para>
        /// </param>
        public Task<string[]> GetKeysAsync(string query)
        {
            return _raeAPI.GetKeysAsync(query);
        }

        /// <summary>
        /// <para>Get a random word.</para>
        /// <para>Obtiene una palabra aleatoria.</para>
        /// </summary>
        public Task<Word> GetRandomWordAsync()
        {
            return _raeAPI.GetRandomWordAsync();
        }

        /// <summary>
        /// <para>Get the word of the day.</para>
        /// <para>Obtiene la palabra del día.</para>
        /// </summary>
        public Task<Word> GetWordOfTheDayAsync()
        {
            return _raeAPI.GetWordOfTheDayAsync();
        }

        /// <summary>
        /// <para>Gets the words that start with a sequence of letters.</para>
        /// <para>Obtiene las palabras que empiezan con una secuencia de letras.</para>
        /// </summary>
        /// <param name="query">
        /// <para>Sequence of letters to search.</para>
        /// <para>Secuencia de letras a buscar.</para>
        /// </param>
        public Task<string[]> GetWordsStartWithAsync(string query)
        {
            return _listaPalabrasAPI.GetWordsStartWithAsync(query);
        }

        /// <summary>
        /// <para>Gets the words that contain a sequence of letters.</para>
        /// <para>Obtiene las palabras que contienen una secuencia de letras.</para>
        /// </summary>
        /// <param name="query">
        /// <para>Sequence of letters to search.</para>
        /// <para>Secuencia de letras a buscar.</para>
        /// </param>
        public Task<string[]> GetWordsContainAsync(string query)
        {
            return _listaPalabrasAPI.GetWordsContainAsync(query);
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
        public Task<List<Word>> SearchWordAsync(string word, bool allGroups = true)
        {
            return _raeAPI.SearchWordAsync(word, allGroups);
        }
    }
}
