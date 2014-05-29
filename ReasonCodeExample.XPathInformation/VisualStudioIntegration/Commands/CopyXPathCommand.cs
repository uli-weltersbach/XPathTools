using System;
using System.Windows;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal class CopyXPathCommand : CopyCommand
    {
        private readonly XPathWriter _writer;
        private string _xpath = string.Empty;

        public CopyXPathCommand(int id, XObjectRepository repository, XPathWriter writer)
            : base(id, repository)
        {
            if(writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            _writer = writer;
        }

        protected override void OnInvoke(object sender, EventArgs e)
        {
            Clipboard.SetText(_xpath);
        }

        protected override void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            if(_writer != null)
            {
                _xpath = _writer.Write(Repository.Get());
            }
            if(string.IsNullOrEmpty(_xpath))
            {
                Command.Visible = false;
                return;
            }
            var elementCount = new XmlNodeRepository().GetNodeCount(Repository.Get(), _xpath);
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