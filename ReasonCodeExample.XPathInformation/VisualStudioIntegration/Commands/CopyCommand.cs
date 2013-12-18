using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal abstract class CopyCommand
    {
        public XObjectRepository Repository { get; private set; }

        protected OleMenuCommand Command { get; private set; }

        protected CopyCommand(int commandID, XObjectRepository repository)
        {
            Repository = repository;
            Command = new OleMenuCommand(OnInvoke, null, OnBeforeQueryStatus, new CommandID(Guid.Parse(Symbols.PackageID), commandID));
        }

        protected abstract void OnInvoke(object sender, EventArgs e);

        protected abstract void OnBeforeQueryStatus(object sender, EventArgs e);

        public static implicit operator OleMenuCommand(CopyCommand command)
        {
            return command.Command;
        }
    }
}
