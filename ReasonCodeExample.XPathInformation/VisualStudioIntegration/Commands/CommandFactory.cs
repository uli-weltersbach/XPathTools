using System.Windows;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideMenuResource("CommandFactory.ctmenu", 1)]
    [Guid(PackageID)]
    internal class CommandFactory : Package
    {
        public const string PackageID = "253aa665-a779-4716-9ded-5b0c2cb66710";
        public const string MenuGroupID = "2a859db4-750c-4267-b96f-844f20ce9e7b";
        public const int CopyXPathCommandID = 0x100;
        private readonly XPathRepository _repository;

        public CommandFactory()
            : this(new XPathRepository())
        {
        }

        public CommandFactory(XPathRepository repository)
        {
            _repository = repository;
        }

        protected override void Initialize()
        {
            base.Initialize();
            CommandID copyXPathCommandID = new CommandID(Guid.Parse(MenuGroupID), CopyXPathCommandID);
            MenuCommand copyXPathCommand = new OleMenuCommand(CopyXPathToClipBoard, null, SetCopyXPathCommandVisibility, copyXPathCommandID);
            IMenuCommandService service = (IMenuCommandService)GetService(typeof(IMenuCommandService));
            service.AddCommand(copyXPathCommand);
        }

        private void CopyXPathToClipBoard(object sender, EventArgs e)
        {
            string xpath = _repository.Get();
            Clipboard.SetText(xpath);
        }

        private void SetCopyXPathCommandVisibility(object sender, EventArgs e)
        {
            MenuCommand menuCommand = sender as MenuCommand;
            if (menuCommand != null)
                menuCommand.Visible = !string.IsNullOrEmpty(_repository.Get());
        }
    }
}