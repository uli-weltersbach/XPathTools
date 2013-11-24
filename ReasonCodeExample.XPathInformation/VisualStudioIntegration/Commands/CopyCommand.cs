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
        private string _commandText;

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
            OleMenuCommand copyCommand = new OleMenuCommand(OnInvoke, null, OnBeforeQueryStatus, _commandID);
            _commandText = copyCommand.Text;
            service.AddCommand(copyCommand);
        }

        private void OnInvoke(object sender, EventArgs e)
        {
            Clipboard.SetText(XPath);
        }

        private void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = sender as OleMenuCommand;
            if (menuCommand == null)
                return;
            SetCommandVisibility(menuCommand);
            SetCommandText(menuCommand);
        }

        private void SetCommandVisibility(OleMenuCommand menuCommand)
        {
            menuCommand.Visible = _repository.Get() != null;
        }

        private void SetCommandText(OleMenuCommand menuCommand)
        {
            if (menuCommand.CommandID.Equals(_commandID))
                menuCommand.Text = string.Format("{0} ({1})", _commandText, XPath);
        }
    }
}
