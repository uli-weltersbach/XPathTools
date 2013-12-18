using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.VisualStudio.Shell;
using ReasonCodeExample.XPathInformation.Formatters;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal class CopyXPathCommand
    {
        private readonly CommandID _commandID;
        private readonly IXPathFormatter _formatter;
        private readonly XPathRepository _repository;
        private OleMenuCommand _command;

        public CopyXPathCommand(int id, IXPathFormatter formatter, XPathRepository repository)
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
            try
            {
                if (string.IsNullOrEmpty(XPath))
                {
                    _command.Visible = false;
                    return;
                }
                int elementCount = GetElementCount(XPath);
                if (elementCount == 0)
                {
                    _command.Visible = false;
                    return;
                }
                _command.Visible = true;
                SetCommandText(elementCount);
            }
            catch (Exception ex)
            {
                // Ignore.
            }
        }

        private int GetElementCount(string xpath)
        {
            XObject current = _repository.Get();
            if (current == null)
                return 0;
            if (current.Document == null)
                return 0;
            if (current.Document.Root == null)
                return 0;
            return current.Document.XPathSelectElements(xpath, new SimpleXmlNamespaceResolver(current.Document)).Count();
        }

        private void SetCommandText(int elementCount)
        {
            _command.Text = XPath;
            string matchText = elementCount == 1 ? "match" : "matches";
            _command.Text = string.Format("({0} {1}) {2}", elementCount, matchText, XPath);
        }
    }
}