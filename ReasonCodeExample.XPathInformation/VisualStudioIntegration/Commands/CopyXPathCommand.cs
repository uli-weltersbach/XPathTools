using System;
using System.Windows;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal class CopyXPathCommand : CopyCommand
    {
        private readonly XPathWriter _writer;
        private string _xpath = string.Empty;

        public CopyXPathCommand(int id, XmlRepository repository, XPathWriter writer)
            : base(id, repository)
        {
            _writer = writer;
        }

        protected override void OnInvoke(object sender, EventArgs e)
        {
            Clipboard.SetText(_xpath);
        }

        protected override void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            var xml = Repository.Get();
            _xpath = _writer.Write(xml);
            if(string.IsNullOrEmpty(_xpath))
            {
                Command.Visible = false;
                return;
            }
            var elementCount = Repository.GetNodeCount(xml, _xpath);
            if(elementCount == 0)
            {
                Command.Visible = false;
                return;
            }
            Command.Visible = true;
            Command.Text = CommandTextFormatter.Format(_xpath, elementCount);
        }
    }
}