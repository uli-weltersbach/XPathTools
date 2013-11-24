using Microsoft.VisualStudio.Shell;
using ReasonCodeExample.XPathInformation.Formatters;
using System;
using System.ComponentModel.Design;
using System.Windows;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal class CopyCommand
    {
        private readonly CommandID _commandID;
        private readonly IPathFormatter _formatter;
        private readonly XPathRepository _repository;
        private OleMenuCommand _command;

        public string XPath
        {
            get
            {
                return _formatter.Format(_repository.Get());
            }
        }

        public CopyCommand(int id, IPathFormatter formatter, XPathRepository repository)
        {
            if (formatter == null)
                throw new ArgumentNullException("formatter");
            if (repository == null)
                throw new ArgumentNullException("repository");
            _commandID = new CommandID(Guid.Parse(Symbols.PackageID), id);
            _formatter = formatter;
            _repository = repository;
        }

        public void Register(IMenuCommandService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");
            _command = new OleMenuCommand(OnInvoke, null, OnBeforeQueryStatus, _commandID);
            service.AddCommand(_command);
        }

        private void OnInvoke(object sender, EventArgs e)
        {
            Clipboard.SetText(XPath);
        }

        private void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            _command.Visible = !string.IsNullOrEmpty(XPath);
            _command.Text = XPath;
        }
    }
}
