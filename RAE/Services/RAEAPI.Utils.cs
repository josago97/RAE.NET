using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
using RAE.Models;
using RAE.Utilities;

namespace RAE.Services
{
    internal partial class RAEAPI
    {
        private static readonly string[] ALLOW_ABBR_DEFINITION = { "U." };

        private static readonly Dictionary<string, WordGenre> GENRES = new Dictionary<string, WordGenre>()
        {
            { "m.", WordGenre.Male },
            { "f.", WordGenre.Female },
            { "f. y m.", WordGenre.Both }
        };

        private static readonly Dictionary<string, WordLanguage> LANGUAGES = new Dictionary<string, WordLanguage>()
        {
            { "coloq.", WordLanguage.Colloquial },
            { "rur.", WordLanguage.Rural }
        };

        private static readonly Dictionary<string, WordType> TYPES = new Dictionary<string, WordType>()
        {
            { "adj.", WordType.Adjective },
            { "adv.", WordType.Adverb },
            { "art.", WordType.Article },
            { "conj.", WordType.Conjunction },
            { "interj.", WordType.Interjection },
            { "pref.", WordType.Prefix },
            { "prep.", WordType.Preposition },
            { "pron.", WordType.Pronoun },
            { "suf.", WordType.Suffix },
            { "verb.", WordType.Verb }
        };

        private string HtmlDecode(string content)
        {
            return WebUtility.HtmlDecode(content);
        }

        private Word GetWord(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return null;

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            HtmlNode root = document.DocumentNode;

            string id = root.SelectSingleNode("//article").GetAttributeValue("id");
            HtmlNode header = root.SelectSingleNode("//header");
            string content = header.SelectSingleNode("./node()[not(self::sup)]").InnerText;
            string[] values = header.GetAttributeValue("title").Replace("Definición de ", "").Split(", ");
            string origin = BuildString(root.SelectNodesOrEmpty(".//p[@class='n2' or @class='n4']/node()[not(self::sup)]"));
            Definition[] definitions = GetDefinitions(root);

            return new Word(id, content, origin, values, definitions);
        }

        private Definition[] GetDefinitions(HtmlNode document)
        {
            List<Definition> result = new List<Definition>();
            HtmlNodeCollection definitions = document.SelectNodesOrEmpty("//p[contains(@class, 'j')]");

            foreach (HtmlNode definition in definitions)
            {
                result.Add(GetDefinition(definition));
            }

            return result.ToArray();
        }

        private Definition GetDefinition(HtmlNode document)
        {
            WordType type = default;
            WordGenre? genre = null;
            WordLanguage? language = null;
            bool isObsolete = false;
            List<string> extraData = new List<string>();

            HtmlNodeCollection dataNodes = document.SelectNodesOrEmpty("./*/preceding-sibling::abbr");

            foreach (HtmlNode dataNode in dataNodes)
            {
                string[] dataValues = dataNode.InnerText.Split(' ');
                bool includedExtraData = false;

                foreach (string dataValue in dataValues)
                {
                    if (GENRES.TryGetValue(dataValue, out WordGenre genreAux))
                    {
                        genre = genreAux;
                        type = WordType.Noun;
                    }
                    else if (LANGUAGES.TryGetValue(dataValue, out WordLanguage languageAux))
                        language = languageAux;
                    else if (TYPES.TryGetValue(dataValue, out WordType typeAux))
                        type = typeAux;
                    else if (dataValue == "desus.")
                        isObsolete = true;
                    else if (!includedExtraData)
                    {
                        extraData.Add(dataNode.GetAttributeValue("title"));
                        includedExtraData = true;
                    }
                }
            }

            string content = GetDefinitionContent(document);
            string[] origins = GetDefinitionOrigins(document);
            string[] examples = GetDefinitionExamples(document);

            return new Definition(type, genre, language, isObsolete, content, origins, examples);
        }

        private string GetDefinitionContent(HtmlNode document)
        {
            string startElementXpath = ".//*[not(self::abbr)]/preceding-sibling::abbr[1]";
            HtmlNode startElement = document.SelectSingleNode(startElementXpath);

            string nodesXpath = "./following-sibling::node()[not(@class='h')]";
            HtmlNodeCollection nodes = startElement.SelectNodesOrEmpty(nodesXpath);

            string result = BuildString(nodes);

            if (result.Contains('‖'))
                result = result.Replace("‖ ", "");
            else if (ALLOW_ABBR_DEFINITION.Contains(startElement.InnerText))
                result = $"{startElement.GetAttributeValue("title")} {result}";

            return result;
        }

        private string[] GetDefinitionOrigins(HtmlNode document)
        {
            List<string> result = new List<string>();
            HtmlNodeCollection nodes = document.SelectNodesOrEmpty("./abbr[@class='c']");

            foreach (HtmlNode node in nodes)
            {
                string[] origins = node.GetAttributeValue("title")
                    .Replace(" y", ",")
                    .Split(", ");

                result.AddRange(origins);
            }

            return result.ToArray();
        }

        private string[] GetDefinitionExamples(HtmlNode document)
        {
            List<string> result = new List<string>();
            HtmlNodeCollection nodes = document.SelectNodesOrEmpty("./*[@class='h']");

            foreach (HtmlNode node in nodes)
            {
                HtmlNodeCollection exampleNodes = node.SelectNodesOrEmpty("./node()");
                string example = BuildString(exampleNodes);

                result.Add(example);
            }

            return result.ToArray();
        }

        private string BuildString(HtmlNodeCollection nodes)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < nodes.Count; i++)
            {
                HtmlNode node = nodes[i];

                if (node.GetDataAttribute("title") is HtmlAttribute attribute)
                    stringBuilder.Append(attribute.Value);
                else
                    stringBuilder.Append(node.InnerText);

                if (i == nodes.Count - 1 && stringBuilder[^1] != '.')
                    stringBuilder.Append('.');
                else if (i < nodes.Count - 1 && nodes[i + 1] is HtmlNode)
                    stringBuilder.Append(' ');
            }

            return stringBuilder.ToString().Trim();
        }
    }
}
