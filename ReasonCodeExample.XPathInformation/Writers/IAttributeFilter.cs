using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal interface IAttributeFilter
    {
        bool IsIncluded(XAttribute attribute);
    }
}