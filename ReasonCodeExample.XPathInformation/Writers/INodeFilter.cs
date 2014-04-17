using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal interface INodeFilter
    {
        bool IsIncluded(XObject node);
    }
}