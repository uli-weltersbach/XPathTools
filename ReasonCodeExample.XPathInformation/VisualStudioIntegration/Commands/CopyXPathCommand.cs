using System;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal class CopyXPathCommand : CopyCommand
    {
        public CopyXPathCommand(int id, XmlRepository repository, Func<IWriter> writerProvider, ICommandTextFormatter textFormatter)
            : base(id, repository, writerProvider, textFormatter)
        {
        }

        protected override void OnBeforeQueryStatus(object sender, EventArgs e)
        {
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