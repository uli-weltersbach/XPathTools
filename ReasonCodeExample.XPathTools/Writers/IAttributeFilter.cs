using System.Xml.Linq;

namespace ReasonCodeExample.XPathTools.Writers
{
    internal interface IAttributeFilter
    {
        bool IsIncluded(XAttribute attribute);
    }
}