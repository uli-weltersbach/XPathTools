using System.Collections.Generic;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration
{
    internal interface IConfiguration
    {
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