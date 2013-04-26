using System;
using System.Linq;
using System.Windows.Controls;
using System.Xml;
using Microsoft.VisualStudio.Text.Editor;

namespace ReasonCodeExample.XPathInformation
{
    internal class XPathAdornment
    {
        private readonly IAdornmentLayer _adornmentLayer;
        private readonly IWpfTextView _view;
        private readonly TextBlock _xpathInformation;

        public XPathAdornment(string adornmentLayerName, IWpfTextView view)
        {
            _view = view;
            _xpathInformation = new TextBlock();
            _adornmentLayer = view.GetAdornmentLayer(adornmentLayerName);
            _view.ViewportHeightChanged += UpdateAdornmentPosition;
            _view.ViewportWidthChanged += UpdateAdornmentPosition;
            _view.Caret.PositionChanged += UpdateXPath;
        }

        private void UpdateAdornmentPosition(object sender, EventArgs eventArgs)
        {
            //clear the adornment layer of previous adornments
            _adornmentLayer.RemoveAllAdornments();

            //Place the image in the top right hand corner of the Viewport
            Canvas.SetLeft(_xpathInformation, _view.ViewportRight - 360);
            Canvas.SetTop(_xpathInformation, _view.ViewportTop + 30);

            //add the image to the adornment layer and make it relative to the viewport
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _xpathInformation, null);
        }

        private void UpdateXPath(object sender, CaretPositionChangedEventArgs e)
        {
            _xpathInformation.Text = TryGetXPath(e);
        }

        private string TryGetXPath(CaretPositionChangedEventArgs e)
        {
            string xml = e.TextView.TextSnapshot.GetText();
            int lineNumber = e.TextView.TextSnapshot.GetLineNumberFromPosition(e.NewPosition.BufferPosition);
            int lineStart = e.TextView.TextSnapshot.GetLineFromLineNumber(lineNumber).Start.Position;
            int absoluteCaretPosition = e.NewPosition.BufferPosition.Position;
            int caretPosition = absoluteCaretPosition - lineStart;
            try
            {
                const int lineNumberOffset = 1;
                return new XPathParser().GetPath(xml, lineNumber + lineNumberOffset, caretPosition);
            }
            catch (XmlException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}