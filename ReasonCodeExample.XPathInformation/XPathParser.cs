using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation
{
    internal class XPathParser
    {
        private const int LinePositionOffset = 1;

        /// <summary>
        /// Gets the XPath of the element located at the specified line and position.
        /// </summary>
        /// <param name="lineNumber">First line is 1.</param>
        public string GetPath(string xml, int lineNumber, int linePosition)
        {
            XElement rootElement = XElement.Parse(xml, LoadOptions.SetLineInfo);
            return GetPath(rootElement, lineNumber, linePosition);
        }

        /// <summary>
        /// Gets the XPath of the element located at the specified line and position.
        /// </summary>
        /// <param name="lineNumber">First line is 1.</param>
        public string GetPath(XElement xml, int lineNumber, int linePosition)
        {
            IEnumerable<XElement> elements = xml.DescendantsAndSelf();
            XElement match = FindMatchingElement(lineNumber, linePosition + LinePositionOffset, elements);
            return CreateXPath(match);
        }

        private XElement FindMatchingElement(int lineNumber, int linePosition, IEnumerable<XElement> elements)
        {
            return (from element in elements
                    where IsCorrectLine(element, lineNumber)
                    where IsCorrectPosition(element, linePosition)
                    orderby GetLinePosition(element) descending
                    select element).FirstOrDefault();
        }

        private bool IsCorrectLine(IXmlLineInfo lineInfo, int lineNumber)
        {
            return lineInfo.LineNumber == lineNumber;
        }

        private bool IsCorrectPosition(IXmlLineInfo lineInfo, int linePosition)
        {
            return lineInfo.LinePosition <= linePosition;
        }

        private int GetLinePosition(IXmlLineInfo lineInfo)
        {
            return lineInfo.LinePosition;
        }

        private string CreateXPath(XElement element)
        {
            if (element == null)
                return string.Empty;

            return element.AncestorsAndSelf().Reverse().Select(GetElementName).Aggregate(string.Empty, ConcatenateXPath);
        }

        private string GetElementName(XElement element)
        {
            if (string.IsNullOrEmpty(element.Name.NamespaceName))
                return element.Name.LocalName;
            string namespacePrefix = element.GetPrefixOfNamespace(element.Name.Namespace);
            if (string.IsNullOrEmpty(namespacePrefix))
                return element.Name.LocalName;
            return namespacePrefix + ":" + element.Name.LocalName;
        }

        private string ConcatenateXPath(string current, string next)
        {
            return current + "/" + next;
        }
    }
}