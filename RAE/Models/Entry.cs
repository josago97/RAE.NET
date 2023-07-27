namespace RAE.Models
{
    internal class Entry : IEntry
    {
        public string Id { get; private set; }
        public string Content { get; private set; }

        internal Entry(string id, string content)
        {
            Id = id;
            Content = content;
        }

        public override string ToString()
        {
            return $"#{Id} => {Content}";
        }
    }
}
