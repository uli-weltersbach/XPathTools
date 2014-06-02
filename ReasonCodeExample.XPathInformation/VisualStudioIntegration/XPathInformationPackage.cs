using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Ninject;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Configuration;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(SolutionExists)]
    [ProvideMenuResource(MenuResourceID, 1)]
    [Guid(Symbols.PackageID)]
    [ProvideOptionPage(typeof(XPathInformationConfiguration), "XPath Information", "General", 0, 0, true)]
    internal class XPathInformationPackage : Package
    {
        private const string SolutionExists = "{f1536ef8-92ec-443c-9ed7-fdadf150da82}";
        private const string MenuResourceID = "CommandFactory.ctmenu";
        private readonly IKernel _container;

        public XPathInformationPackage()
            : this(Registry.Current)
        {
        }

        public XPathInformationPackage(IKernel container)
        {
            _container = container;
        }

        protected override void Initialize()
        {
            try
            {
                base.Initialize();

                _container.Bind<IVsStatusbar>().ToConstant((IVsStatusbar)GetService(typeof(IVsStatusbar)));

                _container.Bind<IList<XPathSetting>>().ToMethod(context => context.Kernel.Get<IConfiguration>().AlwaysDisplayedAttributes);

                _container.Bind<AttributeFilter>().ToSelf();

                _container.Bind<XPathStatusbarInformation>().ToSelf();
                
                _container.Bind<XPathWriter>().ToSelf();

                var repository = new XmlRepository();
                _container.Bind<XmlRepository>().ToConstant(repository);
                
                var configuration = (XPathInformationConfiguration)GetDialogPage(typeof(XPathInformationConfiguration));
                _container.Bind<IConfiguration>().ToConstant(configuration);

                var commandService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
                _container.Bind<IMenuCommandService>().ToConstant(commandService);

                InitializeCommands(repository, configuration, commandService);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error in " + GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeCommands(XmlRepository repository, IConfiguration configuration, IMenuCommandService commandService)
        {
            var alwaysDisplayedAttributes = configuration.AlwaysDisplayedAttributes;

            var attributeFilter = new AttributeFilter(alwaysDisplayedAttributes);

            var copyGenericXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyGenericXPath, repository, new XPathWriter(new[] { attributeFilter }));
            commandService.AddCommand(copyGenericXPathCommand);

            var copyAbsoluteXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyAbsoluteXPath, repository, new AbsoluteXPathWriter(new[] { attributeFilter }));
            commandService.AddCommand(copyAbsoluteXPathCommand);

            var preferredAttributeCandidates = configuration.PreferredAttributeCandidates;
            var distinctAttributeFilter = new DistinctAttributeFilter(preferredAttributeCandidates.Union(alwaysDisplayedAttributes));
            var copyDistinctXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyDistinctXPath, repository, new XPathWriter(new[] { distinctAttributeFilter }));
            commandService.AddCommand(copyDistinctXPathCommand);

            var copyXmlStructureCommand = new CopyXmlStructureCommand(Symbols.CommandIDs.CopyXmlStructure, repository, new XmlStructureWriter());
            commandService.AddCommand(copyXmlStructureCommand);
        }
    }
}