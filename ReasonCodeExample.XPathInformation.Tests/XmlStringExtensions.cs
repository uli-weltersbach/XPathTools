using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ReasonCodeExample.XPathInformation.Tests
{
    internal static class XmlStringExtensions
    {
        public static XObject SelectSingleNode(this string xml, string xpath)
        {
            return xml.SelectNodes(xpath).Single();
        }

        public static IList<XObject> SelectNodes(this string xml, string xpath)
        {
            if(string.IsNullOrEmpty(xml))
            {
                throw new ArgumentNullException(nameof(xml), "xml is null or empty");
            }
            if(string.IsNullOrEmpty(xpath))
            {
                throw new ArgumentNullException(nameof(xpath), "xpath is null or empty");
            }
            var document = XDocument.Parse(xml);
            return ((IEnumerable)document.Root.XPathEvaluate(xpath, new SimpleXmlNamespaceResolver(document))).Cast<XObject>().ToList();
        }
    }
}