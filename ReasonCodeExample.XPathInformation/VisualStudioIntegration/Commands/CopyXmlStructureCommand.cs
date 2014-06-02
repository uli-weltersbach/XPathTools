using System;
using System.Windows;
using System.Xml.Linq;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal class CopyXmlStructureCommand : CopyCommand
    {
        public CopyXmlStructureCommand(int id, XmlRepository repository)
            : base(id, repository)
        {
        }

        protected override void OnInvoke(object sender, EventArgs e)
        {
            string xml = GetXml();
            Clipboard.SetText(xml);
        }

        private string GetXml()
        {
            XObject obj = Repository.Get();
            XElement element = null;
            if (obj is XElement)
                element = obj as XElement;
            else if (obj is XAttribute)
                element = obj.Parent;
            return new XmlStructureWriter().Write(element).ToString(SaveOptions.None);
        }

        protected override void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            Command.Visible = Repository.Get() != null;
        }
    }
}