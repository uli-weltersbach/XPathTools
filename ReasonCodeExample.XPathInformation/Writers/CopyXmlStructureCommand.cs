using System;

namespace ReasonCodeExample.XPathInformation.Writers
{
    internal class CopyXmlStructureCommand : CopyCommand
    {
        public CopyXmlStructureCommand(int id, XmlRepository repository, Func<IWriter> writerProvider, ICommandTextFormatter textFormatter)
            : base(id, repository, writerProvider, textFormatter)
        {
        }

        protected override void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            var xml = Repository.GetSelected();
            var writer = WriterProvider();
            Output = writer.Write(xml);
            Command.Visible = !string.IsNullOrEmpty(Output);
        }
    }
}