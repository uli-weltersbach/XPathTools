using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation
{
    internal class ResultCachingXmlParser
    {
        private int _hashCode;
        private XElement _xml;

        public XElement Parse(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return null;

            if (_hashCode == xml.GetHashCode())
                return _xml;

            _xml = XElement.Parse(xml, LoadOptions.SetLineInfo);
            _hashCode = xml.GetHashCode();
            return _xml;
        }
    }
}