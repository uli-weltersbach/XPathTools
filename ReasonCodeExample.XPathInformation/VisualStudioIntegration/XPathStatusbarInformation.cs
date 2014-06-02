using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    internal class XPathStatusbarInformation
    {
        private readonly XmlRepository _repository;
        private readonly IVsStatusbar _statusbar;
        private readonly XPathWriter _writer;

        public XPathStatusbarInformation(XmlRepository repository, XPathWriter writer, IVsStatusbar statusbar)
        {
            _repository = repository;
            _writer = writer;
            _statusbar = statusbar;
        }

        public void UpdateXPath(object sender, CaretPositionChangedEventArgs e)
        {
            try
            {
                var xml = _repository.Get();
                var xpath = _writer.Write(xml);
                _statusbar.SetText(xpath);
            }
            catch(Exception ex)
            {
                _statusbar.SetText(ex.Message);
            }
        }
    }
}