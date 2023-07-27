using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using RAE.Models;

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

        private XmlDocument ParseToDocument(string xml)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml($"<root>{xml}</root>");

            return document;
        }

        private Word GetWord(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return null;

            XmlDocument document = ParseToDocument(xml);

            string id = document.SelectSingleNode("//article").Attributes["id"].Value;
            XmlElement header = (XmlElement)document.SelectSingleNode("//header");
            string content = header.SelectSingleNode("./node()[not(self::sup)]").InnerText;
            string[] values = header.GetAttribute("title").Replace("Definición de ", "").Split(", ");
            string origin = BuildString(document.SelectNodes(".//p[@class='n2' or @class='n4']/node()[not(self::sup)]"));
            Definition[] definitions = GetDefinitions(document.DocumentElement);

            return new Word(id, content, origin, values, definitions);
        }

        private Definition[] GetDefinitions(XmlElement document)
        {
            List<Definition> result = new List<Definition>();
            XmlNodeList definitions = document.SelectNodes("//p[contains(@class, 'j')]");

            foreach (XmlElement definition in definitions)
            {
                result.Add(GetDefinition(definition));
            }

            return result.ToArray();
        }

        private Definition GetDefinition(XmlElement document)
        {
            WordType type = default;
            WordGenre? genre = null;
            WordLanguage? language = null;
            bool isObsolete = false;
            List<string> extraData = new List<string>();

            XmlNodeList dataNodes = document.SelectNodes("./*/preceding-sibling::abbr");

            foreach (XmlElement dataNode in dataNodes)
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
                        extraData.Add(dataNode.GetAttribute("title"));
                        includedExtraData = true;
                    }
                }
            }

            string content = GetDefinitionContent(document);
            string[] origins = GetDefinitionOrigins(document);
            string[] examples = GetDefinitionExamples(document);

            return new Definition(type, genre, language, isObsolete, content, origins, examples);
        }

        private string GetDefinitionContent(XmlElement document)
        {
            string startElementXpath = ".//*[not(self::abbr)]/preceding-sibling::abbr[1]";
            XmlElement startElement = (XmlElement)document.SelectSingleNode(startElementXpath);

            string nodesXpath = "./following-sibling::node()[not(@class='h')]";
            XmlNodeList nodes = startElement.SelectNodes(nodesXpath);

            string result = BuildString(nodes);

            if (result.Contains('‖'))
                result = result.Replace("‖ ", "");
            else if (ALLOW_ABBR_DEFINITION.Contains(startElement.InnerText))
                result = startElement.GetAttribute("title") + ' ' + result;

            return result;
        }

        private string[] GetDefinitionOrigins(XmlElement document)
        {
            List<string> result = new List<string>();
            XmlNodeList nodes = document.SelectNodes("./abbr[@class='c']");

            foreach (XmlElement node in nodes)
            {
                string[] origins = node.GetAttribute("title")
                    .Replace(" y", ",")
                    .Split(", ");

                result.AddRange(origins);
            }

            return result.ToArray();
        }

        private string[] GetDefinitionExamples(XmlElement document)
        {
            List<string> result = new List<string>();
            XmlNodeList nodes = document.SelectNodes("./*[@class='h']");

            foreach (XmlElement node in nodes)
            {
                XmlNodeList exampleNodes = node.SelectNodes("./node()");
                string example = BuildString(exampleNodes);

                result.Add(example);
            }

            return result.ToArray();
        }

        private string BuildString(XmlNodeList nodes)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < nodes.Count; i++)
            {
                XmlNode node = nodes[i];

                if (node is XmlElement element)
                {
                    if (element.HasAttribute("title"))
                        stringBuilder.Append(element.GetAttribute("title"));
                    else
                        stringBuilder.Append(element.InnerText);

                    if (i == nodes.Count - 1 && stringBuilder[^1] != '.')
                        stringBuilder.Append('.');
                    else if (i < nodes.Count - 1 && nodes[i + 1] is XmlElement)
                        stringBuilder.Append(' ');
                }
                else
                {
                    stringBuilder.Append(node.InnerText);
                }
            }

            return stringBuilder.ToString().Trim();
        }
    }
}
