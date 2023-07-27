using System.Text;

namespace RAE.Models
{
    internal class Word : Entry, IWord
    {
        public string Origin { get; private set; }
        public string[] Values { get; set; }
        public IDefinition[] Definitions { get; private set; }

        internal Word(string id, string content, string origin, string[] values, IDefinition[] definitions) : base(id, content)
        {
            Origin = origin;
            Values = values;
            Definitions = definitions;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.ToString());
            stringBuilder.Append($" ({string.Join(", ", Values)})");
            if (!string.IsNullOrEmpty(Origin)) stringBuilder.Append($" {{{Origin}}}");
            stringBuilder.AppendLine();

            if (Definitions != null)
            {
                for (int i = 0; i < Definitions.Length; i++)
                {
                    stringBuilder.Append($"\t{i + 1}. ");
                    stringBuilder.AppendLine(Definitions[i].ToString());
                }
            }

            return stringBuilder.ToString();
        }
    }
}
