using HtmlAgilityPack;

namespace RAE.Utilities
{
    internal static class Extensions
    {
        public static string GetAttributeValue(this HtmlNode node, string name)
        {
            return node.GetAttributeValue(name, string.Empty);
        }

        public static HtmlNodeCollection SelectNodesOrEmpty(this HtmlNode node, string xpath)
        {
            return node.SelectNodes(xpath) ?? new HtmlNodeCollection(null);
        }
    }
}
