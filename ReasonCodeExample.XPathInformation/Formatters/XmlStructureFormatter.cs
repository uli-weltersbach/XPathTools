using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Formatters
{
    internal class XmlStructureFormatter
    {
        public XElement Format(XElement element)
        {
            if (element == null)
                return null;
            XElement root = element.AncestorsAndSelf().Last();
            XElement copy = new XElement(root);
            List<XElement> ancestorsAndSelf = copy.AncestorsAndSelf().ToList();
            List<XElement> descendants = copy.Descendants().ToList();
            List<XElement> allElements = root.DescendantsAndSelf().ToList();
            foreach (XElement e in allElements)
            {
                if (ancestorsAndSelf.Contains(e))
                    continue;
                if (descendants.Contains(e))
                    continue;
                e.Remove();
            }
            return root;
        }
    }
}
