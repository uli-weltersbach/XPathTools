using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation
{
    internal class ResultCachingXmlParser
    {
        private int _cachedXmlHashCode;
        private XElement _rootElement;

        public XElement Parse(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return null;

            if (_cachedXmlHashCode == xml.GetHashCode())
                return _rootElement;

            _rootElement = XElement.Parse(xml, LoadOptions.SetLineInfo);
            _cachedXmlHashCode = xml.GetHashCode();
            return _rootElement;
        }
    }
}