using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    internal class StatusbarAdapter
    {
        private readonly XmlRepository _repository;
        private readonly IVsStatusbar _statusbar;
        private readonly Func<XPathWriter> _writerProvider;

        public StatusbarAdapter(XmlRepository repository, Func<XPathWriter> writerProvider, IVsStatusbar statusbar)
        {
            _repository = repository;
            _writerProvider = writerProvider;
            _statusbar = statusbar;
        }

        public void SetText(object sender, CaretPositionChangedEventArgs e)
        {
            try
            {
                var xml = _repository.Get();
                var writer = _writerProvider();
                var xpath = writer.Write(xml);
                SetText(xpath);
            }
            catch(Exception ex)
            {
                SetText(ex.Message);
            }
        }

        public void SetText(string text)
        {
            _statusbar.SetText(text);
        }
    }
}