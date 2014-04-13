using System.Collections.Generic;

namespace ReasonCodeExample.XPathInformation
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