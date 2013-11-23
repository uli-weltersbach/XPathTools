using System.Windows;
using System.Xml.Linq;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideMenuResource("CommandFactory.ctmenu", 1)]
    [Guid(PackageID)]
    internal class CommandFactory : Package
    {
        public const string PackageID = "253aa665-a779-4716-9ded-5b0c2cb66710";
        public const string CommandsID = "2a859db4-750c-4267-b96f-844f20ce9e7b";
        public const int CopyPathCommandID = 0x1022;
        public const int CopyAbsolutePathCommandID = 0x1023;
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
            IMenuCommandService service = (IMenuCommandService)GetService(typeof(IMenuCommandService));

            CommandID copyPathCommandID = new CommandID(Guid.Parse(CommandsID), CopyPathCommandID);
            OleMenuCommand copyPathCommand = new OleMenuCommand(CopyPathToClipBoard, null, OnBeforeQueryStatus, copyPathCommandID);
            service.AddCommand(copyPathCommand);

            CommandID copyAbsolutePathCommandID = new CommandID(Guid.Parse(CommandsID), CopyAbsolutePathCommandID);
            OleMenuCommand copyAbsolutePathCommand = new OleMenuCommand(CopyAbsolutePathToClipBoard, null, OnBeforeQueryStatus, copyAbsolutePathCommandID);
            service.AddCommand(copyAbsolutePathCommand);
        }

        private void CopyPathToClipBoard(object sender, EventArgs e)
        {
            SetClipBoardText<PathFormatter>();
        }
        
        private void CopyAbsolutePathToClipBoard(object sender, EventArgs e)
        {
            SetClipBoardText<AbsolutePathFormatter>();
        }

        private void SetClipBoardText<T>() where T : IPathFormatter, new()
        {
            T formatter = new T();
            string xpath = formatter.Format(_repository.Get());
            Clipboard.SetText(xpath);
        }

        private void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = sender as OleMenuCommand;
            if(menuCommand == null)
                return;
            menuCommand.Visible = _repository.Get() != null;
            if (menuCommand.CommandID.Equals(new CommandID(Guid.Parse(CommandsID), CopyPathCommandID)))
            {
                // TODO: Create command object type to avoid code duplication and type switches.
            }

            if (menuCommand.CommandID.Equals(new CommandID(Guid.Parse(CommandsID), CopyAbsolutePathCommandID)))
            {
                
            }
        }
    }
}