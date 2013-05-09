using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    internal class XPathStatusbarInformation
    {
        private readonly IVsStatusbar _statusbar;
        private readonly XPathParserComposite _parser;
        private readonly XPathRepository _repository;

        public XPathStatusbarInformation(ITextView textView)
            : this(textView, (IVsStatusbar)ServiceProvider.GlobalProvider.GetService(typeof(IVsStatusbar)))
        {
        }

        public XPathStatusbarInformation(ITextView textView, IVsStatusbar statusbar)
            : this(textView, statusbar, new XPathParserComposite(), new XPathRepository())
        {
        }

        public XPathStatusbarInformation(ITextView textView, IVsStatusbar statusbar, XPathParserComposite parser, XPathRepository repository)
        {
            if (textView == null)
                throw new ArgumentNullException("textView");
            if (statusbar == null)
                throw new ArgumentNullException("statusbar");
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (repository == null)
                throw new ArgumentNullException("repository");

            textView.Caret.PositionChanged += UpdateXPath;
            textView.LostAggregateFocus += ClearXPath;
            _statusbar = statusbar;
            _parser = parser;
            _repository = repository;
        }

        private void UpdateXPath(object sender, CaretPositionChangedEventArgs e)
        {
            try
            {
                string xpath = _parser.GetXPath(e.TextView);
                _statusbar.SetText(xpath);
                _repository.Put(xpath);
            }
            catch (Exception ex)
            {
                _statusbar.SetText(ex.Message);
            }
        }

        private void ClearXPath(object sender, EventArgs e)
        {
            _repository.Clear();
        }
    }
}