using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Constants = ReasonCodeExample.XPathTools.VisualStudioIntegration.Constants;

namespace ReasonCodeExample.XPathTools
{
    internal class ActiveDocument
    {
        public virtual Document Current
        {
            get
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                try
                {
                    var dte = (DTE)Package.GetGlobalService(typeof(DTE));
                    return dte?.ActiveDocument;
                }
                catch(Exception ex)
                {
                    Console.Error.WriteLine($"Error retrieving '{nameof(DTE.ActiveDocument)}': {ex}");
                    return null;
                }
            }
        }

        public bool IsXmlDocument
        {
            get
            {
                var isXmlDocument = string.Equals(Current?.Language, Constants.XmlContentTypeName, StringComparison.InvariantCultureIgnoreCase);
                return isXmlDocument;
            }
        }

        public string AbsolutePath
        {
            get
            {
                return Current?.FullName;
            }
        }
    }
}
