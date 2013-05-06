using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ReasonCodeExample.XPathInformation.Commands
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(Constants.PackageID)]
    public sealed class CommandsPackage : Package
    {
        private readonly IMenuCommandService _menuCommandService;
        private readonly IVsStatusbar _statusbar;

        public CommandsPackage()
            : this(null, null)
        {
        }

        public CommandsPackage(IMenuCommandService menuCommandService, IVsStatusbar statusbar)
        {
            _menuCommandService = menuCommandService ?? (IMenuCommandService)GetService(typeof(IMenuCommandService));
            _statusbar = statusbar ?? (IVsStatusbar)GetService(typeof(IVsStatusbar));
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            CommandID menuCommandID = new CommandID(Guid.Parse(Constants.MenuGroupID), Constants.SaveCommandSortOrder);
            MenuCommand menuCommand = new OleMenuCommand(CopyStatusBarTextToClipBoard, menuCommandID);
            _menuCommandService.AddCommand(menuCommand);
        }

        private void CopyStatusBarTextToClipBoard(object sender, EventArgs e)
        {
            string text;
            _statusbar.GetText(out text);
            Clipboard.SetText(text);
        }
    }
}