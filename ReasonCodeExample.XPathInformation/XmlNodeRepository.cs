using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation
{
    internal class XmlNodeRepository
    {
        /// <summary>
        /// Gets the XML node located at the specified line and position.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <param name="lineNumber">One-based line number.</param>
        /// <param name="linePosition">
        /// One-based line position. The line position of an XML element is equal to the position 
        /// of the first character in its name, not the position of the opening angle bracket.
        /// E.g. the XML fragment "&lt;element /&gt;" has line position 2.
        /// </param>
        /// <returns>The matching XML node or <c>null</c> if no match was found.</returns>
        public XElement GetElement(XElement rootElement, int lineNumber, int linePosition)
        {
            if (rootElement == null)
                return null;
            IEnumerable<XElement> elements = rootElement.DescendantsAndSelf();
            XElement matchingElement = (from element in elements
                                        where IsCorrectLine(element, lineNumber)
                                        where IsCorrectPosition(element, linePosition)
                                        select element).LastOrDefault();
            if (matchingElement != null)
                return matchingElement;

            XAttribute matchingAttribute = GetAttribute(elements, lineNumber, linePosition);
            return matchingAttribute == null ? null : matchingAttribute.Parent;
        }

        private bool IsCorrectLine(IXmlLineInfo lineInfo, int lineNumber)
        {
            return lineInfo.LineNumber == lineNumber;
        }

        private bool IsCorrectPosition(IXmlLineInfo lineInfo, int linePosition)
        {
            return lineInfo.LinePosition <= linePosition;
        }

        private XAttribute GetAttribute(IEnumerable<XElement> elements, int lineNumber, int linePosition)
        {
            return (from element in elements
                    from attribute in element.Attributes()
                    where IsCorrectLine(attribute, lineNumber)
                    where IsCorrectPosition(attribute, linePosition)
                    select attribute).LastOrDefault();
        }

        public XAttribute GetAttribute(XElement element, int lineNumber, int linePosition)
        {
            if (element == null)
                return null;
            return GetAttribute(new[] { element }, lineNumber, linePosition);
        }
    }
}