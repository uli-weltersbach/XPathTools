using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Ninject;
using ReasonCodeExample.XPathInformation.Workbench;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(UIContextGuids.NoSolution)]
    [ProvideMenuResource(MenuResourceID, 1)]
    [Guid(Symbols.PackageID)]
    [ProvideOptionPage(typeof(XPathInformationDialogPage), "XPath Information", "General", 0, 0, true)]
    [ProvideToolWindow(typeof(XPathWorkbenchWindow))]
    [ProvideToolWindowVisibility(typeof(XPathWorkbenchWindow), VSConstants.UICONTEXT.SolutionExists_string)]
    internal class XPathInformationPackage : Package
    {
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

                var repository = new XmlRepository();
                _container.Bind<XmlRepository>().ToConstant(repository);

                _container.Bind<SearchResultFactory>().ToSelf();

                var configuration = (XPathInformationDialogPage)GetDialogPage(typeof(XPathInformationDialogPage));
                _container.Bind<IConfiguration>().ToConstant(configuration);

                _container.Bind<StatusbarAdapter>().ToConstant(new StatusbarAdapter(repository, () => new XPathWriter(new[] { new AttributeFilter(configuration.AlwaysDisplayedAttributes) }), (IVsStatusbar)GetService(typeof(IVsStatusbar))));

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
            var copyGenericXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyGenericXPath, repository, () => new XPathWriter(new[] {new AttributeFilter(configuration.AlwaysDisplayedAttributes)}), new CommandTextFormatter());
            commandService.AddCommand(copyGenericXPathCommand);

            var copyAbsoluteXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyAbsoluteXPath, repository, () => new AbsoluteXPathWriter(new[] {new AttributeFilter(configuration.AlwaysDisplayedAttributes)}), new CommandTextFormatter());
            commandService.AddCommand(copyAbsoluteXPathCommand);

            var copyDistinctXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyDistinctXPath, repository, () => new XPathWriter(new[] {new AttributeFilter(configuration.AlwaysDisplayedAttributes), new DistinctAttributeFilter(configuration.PreferredAttributeCandidates)}), new CommandTextFormatter());
            commandService.AddCommand(copyDistinctXPathCommand);

            var copySimplifiedXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopySimplifiedXPath, repository, () => new SimplifiedXPathWriter(new[] { new AttributeFilter(configuration.AlwaysDisplayedAttributes) }), new TrimCommandTextFormatter());
            commandService.AddCommand(copySimplifiedXPathCommand);

            var showXPathWorkbenchCommand = new ShowXPathWorkbenchCommand(this, commandService, Symbols.CommandIDs.ShowXPathWorkbench, repository, new ActiveDocument());

            var copyXmlStructureCommand = new CopyXmlStructureCommand(Symbols.CommandIDs.CopyXmlStructure, repository, () => new XmlStructureWriter(), new CommandTextFormatter());
            commandService.AddCommand(copyXmlStructureCommand);
        }
    }
}