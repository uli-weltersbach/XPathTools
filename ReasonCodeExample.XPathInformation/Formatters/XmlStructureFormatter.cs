using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ReasonCodeExample.XPathInformation.Formatters
{
    internal class XmlStructureFormatter
    {
        public XElement Format(XElement element)
        {
            if (element == null)
                return null;
            XElement originalRoot = element.AncestorsAndSelf().Last();
            XDocument copyDocument = new XDocument(new XElement(originalRoot));
            string xpath = new AbsoluteXPathFormatter().Format(element);
            IList<XElement> copiesToKeep = GetElementsToKeep(copyDocument, xpath);
            IList<XElement> allCopies = copyDocument.Root.DescendantsAndSelf().ToArray();
            foreach (XElement copy in allCopies)
            {
                if (!copiesToKeep.Contains(copy))
                    copy.Remove();
            }
            return copyDocument.Root;
        }

        private IList<XElement> GetElementsToKeep(XDocument copyDocument, string xpath)
        {
            List<XElement> elementsToKeep = new List<XElement>();
            XElement element = copyDocument.XPathSelectElement(xpath, new SimpleXmlNamespaceResolver(copyDocument));
            elementsToKeep.AddRange(element.AncestorsAndSelf());
            elementsToKeep.AddRange(element.Descendants());
            return elementsToKeep;
        }
    }
}
