using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class XmlStructureWriter
    {
        public string Write(XElement element)
        {
            if (element == null)
                return null;
            XElement originalRoot = element.AncestorsAndSelf().Last();
            XDocument copyDocument = new XDocument(new XElement(originalRoot));
            XDocument shallowCopy = CopyAncestorsAndDescendantsOf(copyDocument, element);
            RemoveComments(shallowCopy);
            return shallowCopy.Root.ToString(SaveOptions.None);
        }

        private XDocument CopyAncestorsAndDescendantsOf(XDocument document, XElement element)
        {
            string xpath = new AbsoluteXPathWriter().Write(element);
            IList<XElement> elementsToKeep = GetElementsToKeep(document, xpath);
            IList<XElement> allElements = document.Root.DescendantsAndSelf().ToArray();
            foreach (XElement copy in allElements)
            {
                if (!elementsToKeep.Contains(copy))
                    copy.Remove();
            }
            return document;
        }

        private IList<XElement> GetElementsToKeep(XDocument copyDocument, string xpath)
        {
            List<XElement> elementsToKeep = new List<XElement>();
            XElement element = copyDocument.XPathSelectElement(xpath, new SimpleXmlNamespaceResolver(copyDocument));
            elementsToKeep.AddRange(element.AncestorsAndSelf());
            elementsToKeep.AddRange(element.Descendants());
            return elementsToKeep;
        }

        private void RemoveComments(XDocument document)
        {
            IList<XComment> comments = document.DescendantNodes().OfType<XComment>().ToArray();
            foreach (XComment comment in comments)
            {
                comment.Remove();
            }
        }
    }
}