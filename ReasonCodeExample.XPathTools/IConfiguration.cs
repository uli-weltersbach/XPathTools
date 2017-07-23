using System.Collections.Generic;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools
{
    internal interface IConfiguration
    {
        XPathFormat? StatusbarXPathFormat
        {
            get;
        }

        IList<XPathSetting> AlwaysDisplayedAttributes
        {
            get;
        }

        IList<XPathSetting> PreferredAttributeCandidates
        {
            get;
        }
    }
}