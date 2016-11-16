using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class CopyXPathCommand : CopyCommand
    {
        public CopyXPathCommand(int id, XmlRepository repository, Func<IWriter> writerProvider, ICommandTextFormatter textFormatter)
            : base(id, repository, writerProvider, textFormatter)
        {
        }

        protected override void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            if (!Repository.HasContent)
            {
                Command.Visible = false;
                return;
            }
            var dte = (DTE)Package.GetGlobalService(typeof(DTE));
            var isXmlDocument = string.Equals(dte?.ActiveDocument?.Language, "XML", StringComparison.InvariantCultureIgnoreCase);
            if (!isXmlDocument)
            {
                Command.Visible = false;
                return;
            }
            var xml = Repository.GetSelected();
            var writer = WriterProvider();
            Output = writer.Write(xml);
            Command.Visible = !string.IsNullOrEmpty(Output);
            if(string.IsNullOrEmpty(Output))
            {
                return;
            }
            var elementCount = Repository.GetNodeCount(xml, Output);
            Command.Text = TextFormatter.Format(Output, elementCount);
        }
    }
}