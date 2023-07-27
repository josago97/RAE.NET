namespace RAE.Tests;

public class DRAE
{
    private RAE.DRAE Drae { get; } = new RAE.DRAE();

    [Theory]
    [InlineData("-ato")]
    [InlineData("en")]
    [InlineData("en-")]
    [InlineData("manada")]
    [InlineData("niño")]
    [InlineData("tonto")]
    [InlineData("y")]
    public async Task FetchWordExist(string word)
    {
        IList<IWord> words = await Drae.FetchWordAsync(word);

        Assert.NotNull(words);
        Assert.NotEmpty(words);
    }

    [Theory]
    [InlineData("asdadasdada")]
    public async Task FetchWordNoExist(string word)
    {
        IList<IWord> words = await Drae.FetchWordAsync(word);

        Assert.NotNull(words);
        Assert.Empty(words);
    }

    [Theory]
    [InlineData("SPSNBuG")] //pelo
    [InlineData("R9dHZWB")] //ordenador
    public async Task FetchWordByIdExist(string wordId)
    {
        IWord word = await Drae.FetchWordByIdAsync(wordId);

        Assert.NotNull(word);
    }

    [Theory]
    [InlineData("sadafasda")]
    public async Task FetchWordByIdNoExist(string wordId)
    {
        IWord word = await Drae.FetchWordByIdAsync(wordId);

        Assert.Null(word);
    }

    [Fact]
    public async Task GetAllWords()
    {
        string[] allWords = await Drae.GetAllWordsAsync();

        Assert.NotNull(allWords);
        Assert.NotEmpty(allWords);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("la")]
    public async Task GetKeysExist(string query)
    {
        string[] keys = await Drae.GetKeysAsync(query);

        Assert.NotNull(keys);
        Assert.NotEmpty(keys);
    }

    [Theory]
    [InlineData("asdasdsad")]
    public async Task GetKeysNoExist(string query)
    {
        string[] keys = await Drae.GetKeysAsync(query);

        Assert.NotNull(keys);
        Assert.Empty(keys);
    }

    [Fact]
    public async Task GetRandomWord()
    {
        IWord word = await Drae.GetRandomWordAsync();

        Assert.NotNull(word);
    }

    [Fact]
    public async Task GetWordOfTheDay()
    {
        IEntry word = await Drae.GetWordOfTheDayAsync();

        Assert.NotNull(word);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("la")]
    public async Task GetWordsStartWithExist(string query)
    {
        string[] words = await Drae.GetWordsStartWithAsync(query);

        Assert.NotNull(words);
        Assert.NotEmpty(words);
    }

    [Theory]
    [InlineData("aadadsadasdc")]
    public async Task GetWordsStartWithNoExist(string query)
    {
        string[] words = await Drae.GetWordsStartWithAsync(query);

        Assert.NotNull(words);
        Assert.Empty(words);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("la")]
    public async Task GetWordsContainExist(string query)
    {
        string[] words = await Drae.GetWordsContainAsync(query);

        Assert.NotNull(words);
        Assert.NotEmpty(words);
    }

    [Theory]
    [InlineData("dfsdfsdfsfdsa")]
    public async Task GetWordsContainNoExist(string query)
    {
        string[] words = await Drae.GetWordsContainAsync(query);

        Assert.NotNull(words);
        Assert.Empty(words);
    }

    [Theory]
    [InlineData("avión")]
    [InlineData("planeta")]
    public async Task SearchWordExist(string word)
    {
        IEntry[] entries = await Drae.SearchWordAsync(word, true);

        Assert.NotNull(entries);
        Assert.NotEmpty(entries);
    }

    [Theory]
    [InlineData("sdfsfsdfsfssdfsdfs")]
    public async Task SearchWordNoExist(string word)
    {
        IEntry[] entries = await Drae.SearchWordAsync(word, true);

        Assert.NotNull(entries);
        Assert.Empty(entries);
    }
}