namespace RAE
{
    public class Word
    {
        public string Id { get; private set; }

        public string Content { get; private set; }

        public Word(string id, string content)
        {
            Id = id;
            Content = content;
        }

        public override string ToString()
        {
            return Content;
        }
    }
}
