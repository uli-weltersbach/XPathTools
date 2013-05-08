using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace ReasonCodeExample.XPathInformation.Commands
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Guids.PackageID)]
    public sealed class CommandPackage : Package
    {
        private readonly XPathRepository _repository = new XPathRepository();

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            CommandID saveCommandID = new CommandID(Guid.Parse(Guids.MenuGroupID), PkgCmdID.SaveCommandID);
            MenuCommand saveCommand = new OleMenuCommand(CopyXPathToClipBoard, null, SetCommandVisibility, saveCommandID);
            IMenuCommandService service = (IMenuCommandService)GetService(typeof(IMenuCommandService));
            service.AddCommand(saveCommand);
        }

        private void CopyXPathToClipBoard(object sender, EventArgs e)
        {
            Clipboard.SetText(_repository.Get());
        }

        private void SetCommandVisibility(object sender, EventArgs e)
        {
            MenuCommand menuCommand = sender as MenuCommand;
            if (menuCommand != null)
                menuCommand.Visible = !string.IsNullOrEmpty(_repository.Get());
        }
    }
}