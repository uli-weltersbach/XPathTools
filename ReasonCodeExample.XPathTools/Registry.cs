using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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

            var configuration = new XPathToolsDialogPage();
            configuration.LoadSettingsFromStorage();
            serviceContainer.Set<IConfiguration>(configuration);

            var writerFactory = new XPathWriterFactory(configuration);
            serviceContainer.Set(writerFactory);

            var searchResultFactory = new SearchResultFactory();
            serviceContainer.Set<SearchResultFactory>(searchResultFactory);

            ThreadHelper.ThrowIfNotOnUIThread();
            var statusbarService = (IVsStatusbar)Package.GetGlobalService(typeof(IVsStatusbar));
            Func<IWriter> writerProvider = () =>
                                           {
                                               var xpathFormat = configuration.StatusbarXPathFormat ?? XPathFormat.Generic;
                                               return writerFactory.CreateForXPathFormat(xpathFormat);
                                           };
            serviceContainer.Set(new StatusbarAdapter(xmlRepository, writerProvider, statusbarService));
        }

        public static ServiceContainer Current
        {
            get;
        }
    }
}
