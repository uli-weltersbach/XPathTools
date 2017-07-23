using System.Collections.Generic;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration;

namespace ReasonCodeExample.XPathInformation
{
    internal interface IConfiguration
    {
        XPathFormat StatusbarXPathFormat
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