using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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
        private readonly ServiceContainer _container;

        public XPathInformationPackage()
            : this(Registry.Current)
        {
        }

        public XPathInformationPackage(ServiceContainer container)
        {
            _container = container;
        }

        protected override void Initialize()
        {
            try
            {
                base.Initialize();

                var repository = new XmlRepository();
                _container.Set<XmlRepository>(repository);

                _container.Set<SearchResultFactory>(new SearchResultFactory());

                var configuration = (XPathInformationDialogPage)GetDialogPage(typeof(XPathInformationDialogPage));
                _container.Set<IConfiguration>(configuration);

                _container.Set<StatusbarAdapter>(new StatusbarAdapter(repository, () => new XPathWriter(new[] { new AttributeFilter(configuration.AlwaysDisplayedAttributes) }), (IVsStatusbar)GetService(typeof(IVsStatusbar))));

                var commandService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
                _container.Set<IMenuCommandService>(commandService);

                InitializeCommands(repository, configuration, commandService);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error in " + GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeCommands(XmlRepository repository, IConfiguration configuration, IMenuCommandService commandService)
        {
            var activeDocument = new ActiveDocument();

            var subMenu = CreateSubMenu(activeDocument);
            commandService.AddCommand(subMenu);

            var copyGenericXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyGenericXPath, repository, activeDocument, () => new XPathWriter(new[] {new AttributeFilter(configuration.AlwaysDisplayedAttributes)}), new CommandTextFormatter());
            commandService.AddCommand(copyGenericXPathCommand);

            var copyAbsoluteXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyAbsoluteXPath, repository, activeDocument, () => new AbsoluteXPathWriter(new[] {new AttributeFilter(configuration.AlwaysDisplayedAttributes)}), new CommandTextFormatter());
            commandService.AddCommand(copyAbsoluteXPathCommand);

            var copyDistinctXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyDistinctXPath, repository, activeDocument, () => new XPathWriter(new[] {new AttributeFilter(configuration.AlwaysDisplayedAttributes), new DistinctAttributeFilter(configuration.PreferredAttributeCandidates)}), new CommandTextFormatter());
            commandService.AddCommand(copyDistinctXPathCommand);

            var copySimplifiedXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopySimplifiedXPath, repository, activeDocument, () => new SimplifiedXPathWriter(new[] { new AttributeFilter(configuration.AlwaysDisplayedAttributes) }), new TrimCommandTextFormatter());
            commandService.AddCommand(copySimplifiedXPathCommand);

            var showXPathWorkbenchCommand = new ShowXPathWorkbenchCommand(this, commandService, Symbols.CommandIDs.ShowXPathWorkbench, repository, activeDocument);
            commandService.AddCommand(showXPathWorkbenchCommand.Command);

            var copyXmlStructureCommand = new CopyXmlStructureCommand(Symbols.CommandIDs.CopyXmlStructure, repository, activeDocument, () => new XmlStructureWriter(), new CommandTextFormatter());
            commandService.AddCommand(copyXmlStructureCommand);
        }

        private OleMenuCommand CreateSubMenu(ActiveDocument activeDocument)
        {
            EventHandler onBeforeQuery = (sender, args) => { ((OleMenuCommand)sender).Visible = activeDocument.IsXmlDocument; };
            var subMenuCommandId = new CommandID(Guid.Parse(Symbols.PackageID), Symbols.MenuIDs.SubMenu);
            return new OleMenuCommand(null, null, onBeforeQuery, subMenuCommandId);
        }
    }
}