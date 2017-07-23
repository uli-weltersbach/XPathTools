using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReasonCodeExample.XPathTools.Workbench;
using ReasonCodeExample.XPathTools.Writers;

namespace ReasonCodeExample.XPathTools.VisualStudioIntegration
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideAutoLoad(UIContextGuids.NoSolution)]
    [ProvideMenuResource(MenuResourceID, 1)]
    [Guid(Symbols.PackageID)]
    [ProvideOptionPage(typeof(XPathToolsDialogPage), "XPath Tools", "General", 0, 0, true)]
    [ProvideToolWindow(typeof(XPathWorkbenchWindow), Transient = true)]
    [ProvideToolWindowVisibility(typeof(XPathWorkbenchWindow), VSConstants.UICONTEXT.CodeWindow_string)]
    internal class XPathToolsPackage : Package
    {
        private const string MenuResourceID = "CommandFactory.ctmenu";
        private readonly ServiceContainer _container;

        public XPathToolsPackage()
            : this(Registry.Current)
        {
        }

        public XPathToolsPackage(ServiceContainer container)
        {
            _container = container;
        }

        protected override void Initialize()
        {
            try
            {
                base.Initialize();
                var configuration = (XPathToolsDialogPage)GetDialogPage(typeof(XPathToolsDialogPage));
                var commandService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
                var statusbar = (IVsStatusbar)GetService(typeof(IVsStatusbar));
                Initialize(configuration, commandService, statusbar);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void Initialize(IConfiguration configuration, IMenuCommandService commandService, IVsStatusbar statusbar)
        {
            _container.Set<IConfiguration>(configuration);
            _container.Set<IMenuCommandService>(commandService);
            var activeDocument = new ActiveDocument();
            _container.Set<ActiveDocument>(activeDocument);
            var repository = new XmlRepository();
            _container.Set<XmlRepository>(repository);
            _container.Set<SearchResultFactory>(new SearchResultFactory());
            var writerFactory = new XPathWriterFactory(configuration);
            InitializeStatusbar(configuration, writerFactory, repository, statusbar);
            InitializeCommands(activeDocument, repository, writerFactory, commandService);
        }

        private void InitializeStatusbar(IConfiguration configuration, XPathWriterFactory writerFactory, XmlRepository repository, IVsStatusbar statusbar)
        {
            Func<IWriter> writerProvider = () =>
            {
                var xpathFormat = configuration.StatusbarXPathFormat ?? XPathFormat.Generic;
                return writerFactory.CreateForXPathFormat(xpathFormat);
            };
            _container.Set<StatusbarAdapter>(new StatusbarAdapter(repository, writerProvider, statusbar));
        }

        private void InitializeCommands(ActiveDocument activeDocument, XmlRepository repository, XPathWriterFactory writerFactory, IMenuCommandService commandService)
        {
            var subMenu = CreateSubMenu(activeDocument);
            commandService.AddCommand(subMenu);

            var copyGenericXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyGenericXPath, repository, activeDocument,
                () => writerFactory.CreateForCommandId(Symbols.CommandIDs.CopyGenericXPath), new CommandTextFormatter());
            commandService.AddCommand(copyGenericXPathCommand);

            var copyAbsoluteXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyAbsoluteXPath, repository, activeDocument,
                () => writerFactory.CreateForCommandId(Symbols.CommandIDs.CopyAbsoluteXPath), new CommandTextFormatter());
            commandService.AddCommand(copyAbsoluteXPathCommand);

            var copyDistinctXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopyDistinctXPath, repository, activeDocument,
                () => writerFactory.CreateForCommandId(Symbols.CommandIDs.CopyDistinctXPath), new CommandTextFormatter());
            commandService.AddCommand(copyDistinctXPathCommand);

            var copySimplifiedXPathCommand = new CopyXPathCommand(Symbols.CommandIDs.CopySimplifiedXPath, repository, activeDocument,
                () => writerFactory.CreateForCommandId(Symbols.CommandIDs.CopySimplifiedXPath), new TrimCommandTextFormatter());
            commandService.AddCommand(copySimplifiedXPathCommand);

            var showXPathWorkbenchCommand = new ShowXPathWorkbenchCommand(this, commandService, Symbols.CommandIDs.ShowXPathWorkbench, repository, activeDocument);
            commandService.AddCommand(showXPathWorkbenchCommand.Command);

            var copyXmlStructureCommand = new CopyXmlStructureCommand(Symbols.CommandIDs.CopyXmlStructure, repository, activeDocument,
                () => new XmlStructureWriter(), new CommandTextFormatter());
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