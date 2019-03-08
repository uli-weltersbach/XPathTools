using System;
using System.ComponentModel.Design;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools.Writers
{
    internal abstract class CopyCommand
    {
        private readonly ActiveDocument _activeDocument;

        protected CopyCommand(int id, XmlRepository repository, ActiveDocument activeDocument, Func<IWriter> writerProvider, ICommandTextFormatter textFormatter)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _activeDocument = activeDocument ?? throw new ArgumentNullException(nameof(activeDocument));
            Command = new OleMenuCommand(OnInvoke, null, OnBeforeQueryStatus, new CommandID(Guid.Parse(Symbols.PackageID), id));
            WriterProvider = writerProvider ?? throw new ArgumentNullException(nameof(writerProvider));
            TextFormatter = textFormatter ?? throw new ArgumentNullException(nameof(textFormatter));
        }

        protected XmlRepository Repository
        {
            get;
        }

        protected OleMenuCommand Command
        {
            get;
        }

        protected Func<IWriter> WriterProvider
        {
            get;
        }

        protected ICommandTextFormatter TextFormatter
        {
            get;
        }

        protected string Output
        {
            get;
            set;
        }

        private void OnInvoke(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Output))
            {
                Clipboard.SetText(Output);
            }
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
