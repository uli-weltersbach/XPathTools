using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using ReasonCodeExample.XPathTools.Writers;
using Task = System.Threading.Tasks.Task;

namespace ReasonCodeExample.XPathTools.VisualStudioIntegration
{
    internal class StatusbarAdapter
    {
        private readonly XmlRepository _repository;
        private readonly IVsStatusbar _statusbar;
        private readonly Func<IWriter> _writerProvider;

        public StatusbarAdapter(XmlRepository repository, Func<IWriter> writerProvider, IVsStatusbar statusbar)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _writerProvider = writerProvider ?? throw new ArgumentNullException(nameof(writerProvider));
            _statusbar = statusbar ?? throw new ArgumentNullException(nameof(statusbar));
        }

        public void SetText(object sender, CaretPositionChangedEventArgs e)
        {
            try
            {
                var xml = _repository.GetSelected();
                var writer = _writerProvider();
                var xpath = writer.Write(xml);
                SetText(xpath);
            }
            catch(Exception ex)
            {
                SetText(ex.Message);
            }
        }

        private void SetText(string text)
        {
            ThreadHelper.JoinableTaskFactory.Run(() =>
                                                 {
                                                     _statusbar.SetText(text);
                                                     return Task.CompletedTask;
                                                 });
        }
    }
}
