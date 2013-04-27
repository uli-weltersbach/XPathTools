using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;

namespace ReasonCodeExample.XPathInformation
{
    internal class XPathAdornment
    {
        private readonly IAdornmentLayer _adornmentLayer;
        private readonly IWpfTextView _view;
        private readonly UIElement _panel;
        private readonly TextBlock _xpathInformation = new TextBlock();

        public XPathAdornment(string adornmentLayerName, IWpfTextView view)
        {
            _adornmentLayer = view.GetAdornmentLayer(adornmentLayerName);
            _view = view;
            _view.ViewportHeightChanged += UpdateAdornmentPosition;
            _view.ViewportWidthChanged += UpdateAdornmentPosition;
            _view.Caret.PositionChanged += UpdateXPath;
            Border border = new Border { BorderThickness = new Thickness(2), BorderBrush = new SolidColorBrush(Colors.Silver) };
            border.Child = _xpathInformation;
            _panel = border;
        }

        private void UpdateAdornmentPosition(object sender, EventArgs eventArgs)
        {
            // Clear the adornment layer of previous adornments
            _adornmentLayer.RemoveAllAdornments();
            // Place the image in the top right hand corner of the Viewport
            Canvas.SetLeft(_panel, _view.ViewportRight - 360);
            Canvas.SetTop(_panel, _view.ViewportTop + 30);

            // Add the image to the adornment layer and make it relative to the viewport
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _panel, null);
        }

        private void UpdateXPath(object sender, CaretPositionChangedEventArgs e)
        {
            string xpath = TryGetXPath(e);
            _xpathInformation.Text = xpath;
            UpdateStatusBar(xpath);
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

        private void UpdateStatusBar(string xpath)
        {
            IVsStatusbar statusbar = (IVsStatusbar) ServiceProvider.GlobalProvider.GetService(typeof (IVsStatusbar));
            statusbar.SetText(xpath);
        }
    }
}