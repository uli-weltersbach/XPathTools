using System;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text.Editor;

namespace ReasonCodeExample.XPathInformation
{
    /// <summary>
    /// Adornment class that draws a square box in the top right hand corner of the viewport
    /// </summary>
    internal class XPathInformation
    {
        private const string XmlContentTypeName = "XML";
        private IAdornmentLayer _adornmentLayer;
        private IWpfTextView _view;
        private TextBlock _xpathInformation;

        /// <summary>
        /// Creates a square image and attaches an event handler to the layout changed event that
        /// adds the the square in the upper right-hand corner of the TextView via the adornment layer
        /// </summary>
        /// <param name="view">The <see cref="IWpfTextView"/> upon which the adornment will be drawn</param>
        public XPathInformation(IWpfTextView view)
        {
            if (view.TextBuffer.ContentType.IsOfType(XmlContentTypeName))
                InitializeInformationView(view);
        }

        private void InitializeInformationView(IWpfTextView view)
        {
            _view = view;
            _xpathInformation = new TextBlock();
            _adornmentLayer = view.GetAdornmentLayer("ReasonCodeExample.XPathInformation");
            _view.ViewportHeightChanged += OnSizeChange;
            _view.ViewportWidthChanged += OnSizeChange;
            _view.Caret.PositionChanged += CaretPositionChanged;
        }

        private void CaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            string xml = e.TextView.TextSnapshot.GetText();
            int position = e.NewPosition.BufferPosition.Position;
            int nodeEnd = GetNodeEnd(xml, position);
            string relevantXml = xml.Substring(0, nodeEnd);
            _xpathInformation.Text = new XPathParser().Parse(relevantXml);
        }

        private int GetNodeEnd(string xml, int position)
        {
            for (int i = position; i < xml.Length; i++)
            {
                if (xml[i] == '>')
                    return i;
            }
            return xml.Length;
        }

        private void OnSizeChange(object sender, EventArgs eventArgs)
        {
            //clear the adornment layer of previous adornments
            _adornmentLayer.RemoveAllAdornments();

            //Place the image in the top right hand corner of the Viewport
            Canvas.SetLeft(_xpathInformation, _view.ViewportRight - 360);
            Canvas.SetTop(_xpathInformation, _view.ViewportTop + 30);

            //add the image to the adornment layer and make it relative to the viewport
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _xpathInformation, null);
        }
    }
}