namespace RAE
{
    public interface IDefinition
    {
        WordType Type { get; }
        WordGenre? Genre { get; }
        WordLanguage? Language { get; }
        bool IsObsolete { get; }
        string Content { get; }
        string[] Regions { get; }
        string[] Examples { get; }
        string[] OtherData { get; }
    }
}
