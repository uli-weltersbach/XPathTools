using System;
using System.ComponentModel.Design;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal abstract class CopyCommand
    {
        protected CopyCommand(int commandID, XmlRepository repository, Func<IWriter> writerProvider)
        {
            Repository = repository;
            Command = new OleMenuCommand(OnInvoke, null, OnBeforeQueryStatus, new CommandID(Guid.Parse(Symbols.PackageID), commandID));
            WriterProvider = writerProvider;
        }

        protected XmlRepository Repository
        {
            get;
            private set;
        }

        protected OleMenuCommand Command
        {
            get;
            private set;
        }

        protected Func<IWriter> WriterProvider
        {
            get;
            private set;
        }

        protected string Output
        {
            get;
            set;
        }

        private void OnInvoke(object sender, EventArgs e)
        {
            Clipboard.SetText(Output);
        }

        protected abstract void OnBeforeQueryStatus(object sender, EventArgs e);

        public static implicit operator OleMenuCommand(CopyCommand command)
        {
            return command.Command;
        }
    }
}