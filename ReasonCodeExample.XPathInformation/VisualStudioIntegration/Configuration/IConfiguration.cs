using System.Collections.Generic;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration
{
    internal interface IConfiguration
    {
        bool ShowAttributeXPath
        {
            get;
        }

        IList<string> IncludedAttributes
        {
            get;
        }

        IList<string> ExcludedAttributes
        {
            get;
        }

        IList<string> PreferredAttributeCandidateNames
        {
            get;
        }
    }
}