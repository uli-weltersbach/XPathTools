using System.Xml.Linq;

namespace ReasonCodeExample.XPathTools.Writers
{
    internal interface IWriter
    {
        string Write(XObject node);
    }
}