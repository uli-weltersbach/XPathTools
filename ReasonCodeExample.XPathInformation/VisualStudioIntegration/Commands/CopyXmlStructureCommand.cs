using System;
using System.Windows;
using System.Xml.Linq;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal class CopyXmlStructureCommand : CopyCommand
    {
        private readonly XmlStructureWriter _writer;
        private string _xml = string.Empty;

        public CopyXmlStructureCommand(int id, XmlRepository repository, XmlStructureWriter writer)
            : base(id, repository)
        {
            _writer = writer;
        }

        protected override void OnInvoke(object sender, EventArgs e)
        {
            Clipboard.SetText(_xml);
        }

        protected override void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            var xml = Repository.Get();
            XElement element = null;
            if(xml is XElement)
            {
                element = xml as XElement;
            }
            else if(xml is XAttribute)
            {
                element = xml.Parent;
            }
            _xml = _writer.Write(element);
            Command.Visible = !string.IsNullOrEmpty(_xml);
        }
    }
}