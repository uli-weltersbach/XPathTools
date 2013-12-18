using ReasonCodeExample.XPathInformation.Formatters;
using System;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal class CopyXPathCommand : CopyCommand
    {
        private readonly IXPathFormatter _formatter;
        private string _xpath = string.Empty;

        public CopyXPathCommand(int id, XObjectRepository repository, IXPathFormatter formatter)
            : base(id, repository)
        {
            if (formatter == null)
                throw new ArgumentNullException("formatter");
            _formatter = formatter;
        }

        protected override void OnInvoke(object sender, EventArgs e)
        {
            Clipboard.SetText(_xpath);
        }

        protected override void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            _xpath = _formatter.Format(Repository.Get());
            if (string.IsNullOrEmpty(_xpath))
            {
                Command.Visible = false;
                return;
            }
            int elementCount = GetElementCount(_xpath);
            if (elementCount == 0)
            {
                Command.Visible = false;
                return;
            }
            Command.Visible = true;
            SetCommandText(_xpath, elementCount);
        }

        private int GetElementCount(string xpath)
        {
            XObject current = Repository.Get();
            if (current == null)
                return 0;
            if (current.Document == null)
                return 0;
            if (current.Document.Root == null)
                return 0;
            try
            {
                return current.Document.XPathSelectElements(xpath, new SimpleXmlNamespaceResolver(current.Document)).Count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void SetCommandText(string xpath, int elementCount)
        {
            Command.Text = xpath;
            string matchText = elementCount == 1 ? "match" : "matches";
            Command.Text = string.Format("({0} {1}) {2}", elementCount, matchText, _xpath);
        }
    }
}