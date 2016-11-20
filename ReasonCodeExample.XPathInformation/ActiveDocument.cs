using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace ReasonCodeExample.XPathInformation
{
    internal class ActiveDocument
    {
        public virtual bool IsXmlDocument
        {
            get
            {
                var dte = (DTE)Package.GetGlobalService(typeof(DTE));
                var isXmlDocument = string.Equals(dte?.ActiveDocument?.Language, "XML", StringComparison.InvariantCultureIgnoreCase);
                return isXmlDocument;
            }
        }
    }
}