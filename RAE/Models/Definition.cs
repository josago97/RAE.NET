using System.Text;

namespace RAE.Models
{
    internal class Definition : IDefinition
    {
        public WordType Type { get; private set; }
        public WordGenre? Genre { get; private set; }
        public WordLanguage? Language { get; private set; }
        public bool IsObsolete { get; private set; }
        public string Content { get; private set; }
        public string[] Regions { get; private set; }
        public string[] Examples { get; private set; }
        public string[] OtherData { get; private set; }

        internal Definition(WordType type, WordGenre? genre, WordLanguage? language, bool isObsolete,
            string content, string[] origins, string[] examples)
        {
            Type = type;
            Genre = genre;
            Language = language;
            IsObsolete = isObsolete;
            Content = content;
            Regions = origins;
            Examples = examples;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(Type);
            if (Genre.HasValue) stringBuilder.Append(", " + Genre.Value);
            if (Language.HasValue) stringBuilder.Append(", " + Language.Value);
            if (IsObsolete) stringBuilder.Append(", Obsolete");
            if (Regions != null && Regions.Length > 0) stringBuilder.Append(", " + string.Join(", ", Regions));
            if (OtherData != null && OtherData.Length > 0) stringBuilder.Append(", " + string.Join(", ", OtherData));
            stringBuilder.Append(" => " + Content);
            if (Examples != null && Examples.Length > 0) stringBuilder.Append(" (" + string.Join(" ", Examples) + ")");

            return stringBuilder.ToString();
        }
    }
}
