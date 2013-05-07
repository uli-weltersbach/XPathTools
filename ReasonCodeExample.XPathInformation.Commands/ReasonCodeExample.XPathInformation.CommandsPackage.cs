using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace ReasonCodeExample.XPathInformation.Commands
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Constants.PackageID)]
    public sealed class CommandsPackage : Package
    {
        private readonly XPathRepository _repository = new XPathRepository();

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            CommandID menuCommandID = new CommandID(Guid.Parse(Constants.MenuGroupID), Constants.SaveCommandID);
            MenuCommand menuCommand = new OleMenuCommand(CopyXPathToClipBoard, null, SetCommandVisibility, menuCommandID);
            IMenuCommandService service = (IMenuCommandService)GetService(typeof(IMenuCommandService));
            service.AddCommand(menuCommand);
        }

        private void CopyXPathToClipBoard(object sender, EventArgs e)
        {
            MenuCommand menuCommand = sender as MenuCommand;
            if (menuCommand == null)
                return;
            Clipboard.SetText(_repository.Get());
        }

        private void SetCommandVisibility(object sender, EventArgs e)
        {
            MenuCommand menuCommand = sender as MenuCommand;
            if (menuCommand == null)
                return;
            menuCommand.Visible = !string.IsNullOrEmpty(_repository.Get());
        }
    }
}