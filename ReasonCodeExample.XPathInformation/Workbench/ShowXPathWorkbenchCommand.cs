using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration;

namespace ReasonCodeExample.XPathInformation.Workbench
{
    internal class ShowXPathWorkbenchCommand
    {
        private readonly Package _package;

        public ShowXPathWorkbenchCommand(Package package, IMenuCommandService commandService, int id)
        {
            _package = package;
            if(commandService == null)
            {
                throw new ArgumentNullException(nameof(commandService));
            }
            var menuCommandID = new CommandID(Guid.Parse(Symbols.PackageID), id);
            var menuCommand = new OleMenuCommand(ShowToolWindow, menuCommandID, PackageResources.ShowXPathWorkbenchCommandText);
            commandService.AddCommand(menuCommand);
        }

        private void ShowToolWindow(object sender, EventArgs e)
        {
            var toolWindowPane = _package.FindToolWindow(typeof(XPathWorkbenchWindow), 0, true);
            if(toolWindowPane?.Frame == null)
            {
                throw new NotSupportedException(string.Format("{0}: Cannot create tool window.", GetType()));
            }
            var windowFrame = (IVsWindowFrame)toolWindowPane.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}