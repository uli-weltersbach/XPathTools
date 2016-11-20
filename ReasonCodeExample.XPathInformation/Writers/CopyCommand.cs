using System;
using System.ComponentModel.Design;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal abstract class CopyCommand
    {
        private readonly ActiveDocument _activeDocument;

        protected CopyCommand(int id, XmlRepository repository, ActiveDocument activeDocument, Func<IWriter> writerProvider, ICommandTextFormatter textFormatter)
        {
            _activeDocument = activeDocument;
            Repository = repository;
            Command = new OleMenuCommand(OnInvoke, null, OnBeforeQueryStatus, new CommandID(Guid.Parse(Symbols.PackageID), id));
            WriterProvider = writerProvider;
            TextFormatter = textFormatter;
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

        protected ICommandTextFormatter TextFormatter
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

        protected virtual void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            Command.Visible = Repository.HasContent && _activeDocument.IsXmlDocument;
        }

        public static implicit operator OleMenuCommand(CopyCommand command)
        {
            return command.Command;
        }
    }
}