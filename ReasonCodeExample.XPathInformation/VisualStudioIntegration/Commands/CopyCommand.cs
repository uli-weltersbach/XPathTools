using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal abstract class CopyCommand
    {
        protected CopyCommand(int commandID, XmlRepository repository)
        {
            Repository = repository;
            Command = new OleMenuCommand(OnInvoke, null, OnBeforeQueryStatus, new CommandID(Guid.Parse(Symbols.PackageID), commandID));
        }

        public XmlRepository Repository
        {
            get;
            private set;
        }

        protected OleMenuCommand Command
        {
            get;
            private set;
        }

        protected abstract void OnInvoke(object sender, EventArgs e);

        protected abstract void OnBeforeQueryStatus(object sender, EventArgs e);

        public static implicit operator OleMenuCommand(CopyCommand command)
        {
            return command.Command;
        }
    }
}