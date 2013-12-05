using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Formatters
{
    public interface IXPathFormatter
    {
        /// <summary>
        /// Returns the XPath of the element or attribute. 
        /// E.g. "/configuration/ns:settings" or "/configuration/ns:settings[@ns:name]".
        /// </summary>
        string Format(XObject obj);

        /// <summary>
        /// Returns the XPath of the element. E.g. "/configuration/ns:settings".
        /// </summary>
        string Format(XElement element);

        /// <summary>
        /// Returns the local XPath of the attribute. E.g. "[@ns:name]".
        /// </summary>
        string Format(XAttribute attribute);
    }
}