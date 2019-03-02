using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;
using ReasonCodeExample.XPathTools.Writers;

namespace ReasonCodeExample.XPathTools
{
    internal static class Registry
    {
        static Registry()
        {
            Current = new ServiceContainer();
            RegisterDefaultServices();
        }

        private static void RegisterDefaultServices()
        {
            var xmlRepository = new XmlRepository();
            Current.Set(xmlRepository);

            var activeDocument = new ActiveDocument();
            Current.Set(activeDocument);

            ThreadHelper.ThrowIfNotOnUIThread();
            var statusbarService = (IVsStatusbar)Package.GetGlobalService(typeof(IVsStatusbar));
            var configuration = new XPathToolsDialogPage();
            configuration.LoadSettingsFromStorage();
            var writerFactory = new XPathWriterFactory(configuration);
            Func<IWriter> writerProvider = () =>
                                           {
                                               var xpathFormat = configuration.StatusbarXPathFormat ?? XPathFormat.Generic;
                                               return writerFactory.CreateForXPathFormat(xpathFormat);
                                           };
            Current.Set(new StatusbarAdapter(xmlRepository, writerProvider, statusbarService));
        }

        public static ServiceContainer Current
        {
            get;
        }
    }
}
