using System;

namespace ReasonCodeExample.XPathTools.Writers
{
    internal class CopyXPathCommand : CopyCommand
    {
        public CopyXPathCommand(int id, XmlRepository repository, ActiveDocument activeDocument, Func<IWriter> writerProvider, ICommandTextFormatter textFormatter)
            : base(id, repository, activeDocument, writerProvider, textFormatter)
        {
        }

        protected override void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            base.OnBeforeQueryStatus(sender, e);
            if(!Command.Visible)
            {
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
