using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal interface IWriter
    {
        string Write(XObject node);
    }
}