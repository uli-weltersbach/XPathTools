using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;

namespace ReasonCodeExample.XPathInformation
{
    internal class XPathStatusbarInformation
    {
        private readonly XPathParserComposite _service = new XPathParserComposite();
        private readonly XPathRepository _repository = new XPathRepository();
        private readonly IVsStatusbar _statusbar;

        public XPathStatusbarInformation(ITextView textView)
            : this(textView, (IVsStatusbar)ServiceProvider.GlobalProvider.GetService(typeof(IVsStatusbar)))
        {
        }

        public XPathStatusbarInformation(ITextView textView, IVsStatusbar statusbar)
        {
            if (textView == null)
                throw new ArgumentNullException("textView");
            if (statusbar == null)
                throw new ArgumentNullException("statusbar");
            textView.Caret.PositionChanged += UpdateXPath;
            _statusbar = statusbar;
        }

        private void UpdateXPath(object sender, CaretPositionChangedEventArgs e)
        {
            try
            {
                string xpath = _service.GetXPath(e.TextView);
                _statusbar.SetText(xpath);
                _repository.Put(xpath);
            }
            catch (Exception ex)
            {
                _statusbar.SetText(ex.Message);
            }
        }
    }
}