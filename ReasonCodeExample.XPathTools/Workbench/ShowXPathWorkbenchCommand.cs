using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools.Workbench
{
    internal class ShowXPathWorkbenchCommand
    {
        private readonly ActiveDocument _activeDocument;
        private readonly Package _package;
        private readonly XmlRepository _repository;

        public OleMenuCommand Command
        {
            get;
        }

        public ShowXPathWorkbenchCommand(Package package, IMenuCommandService commandService, int id, XmlRepository repository, ActiveDocument activeDocument)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            if (commandService == null)
                throw new ArgumentNullException(nameof(commandService));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _activeDocument = activeDocument ?? throw new ArgumentNullException(nameof(activeDocument));
            if(commandService == null)
            {
                throw new ArgumentNullException(nameof(commandService));
            }
            var menuCommandID = new CommandID(Guid.Parse(Symbols.PackageID), id);
            Command = new OleMenuCommand(ShowToolWindow, null, OnBeforeQueryStatus, menuCommandID, PackageResources.ShowXPathWorkbenchCommandText);
        }

        private void OnBeforeQueryStatus(object sender, EventArgs eventArgs)
        {
            var command = (OleMenuCommand)sender;
            command.Visible = _repository.HasContent && _activeDocument.IsXmlDocument;
        }

        private void ShowToolWindow(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var toolWindowPane = _package.FindToolWindow(typeof(XPathWorkbenchWindow), 0, true);
            if(toolWindowPane?.Frame == null)
            {
                throw new NotSupportedException($"{GetType()}: Cannot create tool window.");
            }
            var windowFrame = (IVsWindowFrame)toolWindowPane.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
