using System;
using System.Collections;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ReasonCodeExample.XPathInformation.Tests
{
    internal static class Extensions
    {
        public static XObject SelectSingleNode(this string xml, string xpath)
        {
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentNullException("xml", "xml is null or empty");
            if (string.IsNullOrEmpty(xpath))
                throw new ArgumentNullException("xpath", "xpath is null or empty");
            var document = XDocument.Parse(xml);
            var enumerator = ((IEnumerable)document.Root.XPathEvaluate(xpath, new SimpleXmlNamespaceResolver(document))).GetEnumerator();
            enumerator.MoveNext();
            return (XObject)enumerator.Current;
        }
    }
}