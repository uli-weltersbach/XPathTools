using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Constants = ReasonCodeExample.XPathTools.VisualStudioIntegration.Constants;

namespace ReasonCodeExample.XPathTools
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

        public string AbsolutePath
        {
            get
            {
                var dte = (DTE)Package.GetGlobalService(typeof(DTE));
                return dte?.ActiveDocument?.FullName;
            }
        }
    }
}