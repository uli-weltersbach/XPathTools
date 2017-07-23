using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ReasonCodeExample.XPathTools.Writers
{
    internal class XmlStructureWriter : IWriter
    {
        public string Write(XObject xml)
        {
            XElement element;
            if(xml is XAttribute)
            {
                element = xml.Parent;
            }
            else if(xml is XElement)
            {
                element = (XElement)xml;
            }
            else
            {
                return null;
            }
            var originalRoot = element.AncestorsAndSelf().Last();
            var copyDocument = new XDocument(new XElement(originalRoot));
            var shallowCopy = CopyAncestorsAndDescendantsOf(copyDocument, element);
            RemoveComments(shallowCopy);
            return shallowCopy.Root.ToString(SaveOptions.None);
        }

        private XDocument CopyAncestorsAndDescendantsOf(XDocument document, XElement element)
        {
            var xpath = new AbsoluteXPathWriter().Write(element);
            var elementsToKeep = GetElementsToKeep(document, xpath);
            var allElements = document.Root.DescendantsAndSelf().ToArray();
            foreach(var copy in allElements)
            {
                if(!elementsToKeep.Contains(copy))
                {
                    copy.Remove();
                }
            }
            return document;
        }

        private IList<XElement> GetElementsToKeep(XDocument copyDocument, string xpath)
        {
            var elementsToKeep = new List<XElement>();
            var element = copyDocument.XPathSelectElement(xpath, new SimpleXmlNamespaceResolver(copyDocument));
            elementsToKeep.AddRange(element.AncestorsAndSelf());
            elementsToKeep.AddRange(element.Descendants());
            return elementsToKeep;
        }

        private void RemoveComments(XDocument document)
        {
            var comments = document.DescendantNodes().OfType<XComment>().ToArray();
            foreach(var comment in comments)
            {
                comment.Remove();
            }
        }
    }
}