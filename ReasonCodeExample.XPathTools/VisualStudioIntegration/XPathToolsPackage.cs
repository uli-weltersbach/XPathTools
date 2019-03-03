using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReasonCodeExample.XPathTools.Workbench;
using ReasonCodeExample.XPathTools.Writers;

namespace ReasonCodeExample.XPathTools.VisualStudioIntegration
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(UIContextGuids.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource(MenuResourceID, 1)]
    [Guid(Symbols.PackageID)]
    [ProvideOptionPage(typeof(XPathToolsDialogPage), "XPath Tools", "General", 0, 0, true)]
    [ProvideToolWindow(typeof(XPathWorkbenchWindow), Transient = true)]
    [ProvideToolWindowVisibility(typeof(XPathWorkbenchWindow), VSConstants.UICONTEXT.CodeWindow_string)]
    internal class XPathToolsPackage : AsyncPackage
    {
        private const string MenuResourceID = "CommandFactory.ctmenu";

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            try
            {
                var configuration = (IConfiguration)GetDialogPage(typeof(XPathToolsDialogPage));
                Registry.Current.Set(configuration);
                var menuCommandService = (IMenuCommandService)await GetServiceAsync(typeof(IMenuCommandService));
                InitializeCommands(Registry.Current.Get<ActiveDocument>(), Registry.Current.Get<XmlRepository>(), Registry.Current.Get<XPathWriterFactory>(), menuCommandService);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void InitializeCommands(ActiveDocument activeDocument, XmlRepository repository, XPathWriterFactory writerFactory, IMenuCommandService commandService)
        {
            if (activeDocument == null)
                throw new ArgumentNullException(nameof(activeDocument));
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (writerFactory == null)
                throw new ArgumentNullException(nameof(writerFactory));
            if (commandService == null)
                throw new ArgumentNullException(nameof(commandService));

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

            var showXPathWorkbenchCommand = new ShowXPathWorkbenchCommand(this, Symbols.CommandIDs.ShowXPathWorkbench, repository, activeDocument);
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
