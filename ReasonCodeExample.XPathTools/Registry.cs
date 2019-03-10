using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReasonCodeExample.XPathTools.Configuration;
using ReasonCodeExample.XPathTools.Statusbar;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;
using ReasonCodeExample.XPathTools.Workbench;
using ReasonCodeExample.XPathTools.Writers;

namespace ReasonCodeExample.XPathTools
{
    internal static class Registry
    {
        static Registry()
        {
            Current = new ServiceContainer();
            try
            {
                RegisterDefaultServices(Current);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(Registry)}: Error registering default services. {ex}");
                throw;
            }
        }

        private static void RegisterDefaultServices(ServiceContainer serviceContainer)
        {
            var xmlRepository = new XmlRepository();
            serviceContainer.Set(xmlRepository);

            var activeDocument = new ActiveDocument();
            serviceContainer.Set(activeDocument);

            var writerFactory = new XPathWriterFactory(GetCurrentConfiguration);
            serviceContainer.Set(writerFactory);

            var searchResultFactory = new SearchResultFactory(activeDocument);
            serviceContainer.Set(searchResultFactory);

            ThreadHelper.ThrowIfNotOnUIThread();
            var statusbarService = (IVsStatusbar)Package.GetGlobalService(typeof(IVsStatusbar));

            serviceContainer.Set(new StatusbarAdapter(xmlRepository, GetCurrentStatusbarXPathWriter, statusbarService));
        }

        private static IConfiguration GetCurrentConfiguration()
        {
            return Current.Get<IConfiguration>();
        }

        private static IWriter GetCurrentStatusbarXPathWriter()
        {
            var configuration = GetCurrentConfiguration();
            var xpathFormat = configuration.StatusbarXPathFormat ?? XPathFormat.Generic;
            var writerFactory = Current.Get<XPathWriterFactory>();
            return writerFactory.CreateForXPathFormat(xpathFormat);
        }

        public static ServiceContainer Current
        {
            get;
        }
    }
}
