using System;
using System.Runtime.InteropServices;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Ninject;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;
using ReasonCodeExample.XPathInformation.Workbench;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    [Guid(Symbols.ToolWindowIDs.XPathWorkbench)]
    internal sealed class XPathWorkbenchWindow : ToolWindowPane
    {
        public XPathWorkbenchWindow()
            : base(null)
        {
            Caption = "XPath Information - Workbench";
            var workbench = new XPathWorkbench(Registry.Current.Get<XmlRepository>(), Registry.Current.Get<SearchResultFactory>());
            workbench.SearchResultSelected += GoToSearchResult;
            Content = workbench;
        }

        private void GoToSearchResult(object sender, SearchResult searchResult)
        {
            try
            {
                if(!searchResult.LineNumber.HasValue)
                {
                    return;
                }
                if(!searchResult.LinePosition.HasValue)
                {
                    return;
                }
                var dte = (DTE)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
                var textSelection = (TextSelection)(dte?.ActiveDocument?.Selection);
                var lineNumber = searchResult.LineNumber.Value;
                var selectionStart = searchResult.LinePosition.Value;
                textSelection?.MoveTo(lineNumber, selectionStart, false);
                if(searchResult.SelectionLength.HasValue)
                {
                    var selectionEnd = selectionStart + searchResult.SelectionLength.Value;
                    textSelection?.MoveToLineAndOffset(lineNumber, selectionEnd, true);
                }
            }
            catch(Exception)
            {
                MessageBox.Show("Error navigating to matching node.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}