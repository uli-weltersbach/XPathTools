using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Text.RegularExpressions;

namespace ReasonCodeExample.XPathInformation
{
    /// <summary>
    /// Adornment class that draws a square box in the top right hand corner of the viewport
    /// </summary>
    internal class XPathInformation
    {
        private const string XmlContentTypeName = "XML";
        private TextBlock _xpathInformation;
        private IWpfTextView _view;
        private IAdornmentLayer _adornmentLayer;

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
            _view.ViewportHeightChanged += delegate { this.onSizeChange(); };
            _view.ViewportWidthChanged += delegate { this.onSizeChange(); };
            _view.Caret.PositionChanged += Caret_PositionChanged;
        }

        private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            string xml = e.TextView.TextSnapshot.GetText();
            int position = e.NewPosition.BufferPosition.Position;
            int nodeStart = GetNodeStart(xml, position);
            string relevantXml = xml.Substring(0, nodeStart);
            int count = CountSpacesAndNewLines(relevantXml);
            Regex xpathRegex = new Regex(@"<(?'NodeName'\w+)[\w\s]+?/>|<\1.*?>.*?</\1>");
            StringBuilder xpath = new StringBuilder();
            foreach (Match match in xpathRegex.Matches(relevantXml))
            {
                string nodeName = match.Groups["NodeName"].Value;
                xpath.AppendFormat("/{0}", nodeName);
            }
            _xpathInformation.Text = xpath.ToString();
        }

        private static int CountSpacesAndNewLines(string s)
        {
            int count = 0;
            foreach (char c in s)
            {
                if (c == '\n') count++;
                if (c == ' ') count++;
            }
            return count + 1;
        }

        private int GetNodeStart(string xml, int position)
        {
            for (int i = position; i >= 0; i--)
            {
                if (xml[i] == '<')
                    return i;
            }
            return -1;
        }

        private void onSizeChange()
        {
            //clear the adornment layer of previous adornments
            _adornmentLayer.RemoveAllAdornments();

            //Place the image in the top right hand corner of the Viewport
            Canvas.SetLeft(_xpathInformation, _view.ViewportRight - 60);
            Canvas.SetTop(_xpathInformation, _view.ViewportTop + 30);

            //add the image to the adornment layer and make it relative to the viewport
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _xpathInformation, null);
        }
    }
}
