using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.VisualStudio.Shell;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal class CopyPathCommand
    {
        private readonly CommandID _commandID;
        private readonly IPathFormatter _formatter;
        private readonly XPathRepository _repository;
        private OleMenuCommand _command;

        public CopyPathCommand(int id, IPathFormatter formatter, XPathRepository repository)
        {
            if (formatter == null)
                throw new ArgumentNullException("formatter");
            if (repository == null)
                throw new ArgumentNullException("repository");
            _commandID = new CommandID(Guid.Parse(Symbols.PackageID), id);
            _formatter = formatter;
            _repository = repository;
        }

        public string XPath
        {
            get { return _formatter.Format(_repository.Get()); }
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
            if (string.IsNullOrEmpty(XPath))
                _command.Visible = false;
            else
            {
                _command.Visible = true;
                SetCommandText();
            }
        }

        private void SetCommandText()
        {
            _command.Text = XPath;
            XObject current = _repository.Get();
            if (current == null)
                return;
            if (current.Document == null)
                return;
            XmlReader reader = current.Document.CreateReader();
            if (reader.NameTable == null)
                return;
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(reader.NameTable);
            int elementCount = current.Document.XPathSelectElements(XPath, namespaceManager).Count();
            string matchText = elementCount == 1 ? "match" : "matches";
            _command.Text = string.Format("({0} {1}) {2}", elementCount, matchText, XPath);
        }
    }
}