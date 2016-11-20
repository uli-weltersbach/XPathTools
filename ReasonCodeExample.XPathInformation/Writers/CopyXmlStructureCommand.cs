using System;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class CopyXmlStructureCommand : CopyCommand
    {
        public CopyXmlStructureCommand(int id, XmlRepository repository, ActiveDocument activeDocument, Func<IWriter> writerProvider, ICommandTextFormatter textFormatter)
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
        }
    }
}