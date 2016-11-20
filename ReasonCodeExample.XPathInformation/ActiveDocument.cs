using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Constants = ReasonCodeExample.XPathInformation.VisualStudioIntegration.Constants;

namespace ReasonCodeExample.XPathInformation
{
    internal class ActiveDocument
    {
        public bool IsXmlDocument
        {
            get
            {
                var dte = (DTE)Package.GetGlobalService(typeof(DTE));
                var isXmlDocument = string.Equals(dte?.ActiveDocument?.Language, Constants.XmlContentTypeName, StringComparison.InvariantCultureIgnoreCase);
                return isXmlDocument;
            }
        }
    }
}