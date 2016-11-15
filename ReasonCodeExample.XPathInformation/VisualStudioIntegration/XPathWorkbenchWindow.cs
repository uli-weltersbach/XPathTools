using System;
using System.Runtime.InteropServices;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Ninject;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;
using ReasonCodeExample.XPathInformation.Workbench;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    [Guid(Symbols.ToolWindowIDs.XPathWorkbench)]
    internal sealed class XPathWorkbenchWindow : ToolWindowPane
    {
        public override bool SearchEnabled
        {
            get { return true; }
        }

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
                if (!searchResult.LineNumber.HasValue)
                {
                    return;
                }
                if (!searchResult.LinePosition.HasValue)
                {
                    return;
                }
                var dte = (DTE)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE));
                var textSelection = (TextSelection)(dte?.ActiveDocument?.Selection);
                var lineNumber = searchResult.LineNumber.Value;
                var selectionStart = searchResult.LinePosition.Value;
                textSelection?.MoveTo(lineNumber, selectionStart, false);
                if (searchResult.SelectionLength.HasValue)
                {
                    var selectionEnd = selectionStart + searchResult.SelectionLength.Value;
                    textSelection?.MoveToLineAndOffset(lineNumber, selectionEnd, true);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error navigating to matching node.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public override IVsSearchTask CreateSearch(uint dwCookie, IVsSearchQuery pSearchQuery, IVsSearchCallback pSearchCallback)
        {
            if (pSearchQuery == null || pSearchCallback == null)
                return null;
            return new XPathSearchTask(dwCookie, pSearchQuery, pSearchCallback, this);
        }

        public override void ClearSearch()
        {
            XPathWorkbench control = (XPathWorkbench)this.Content;
            control.SearchResults.Clear();
            control.SearchResultText.Text = null;
        }

        public override void ProvideSearchSettings(IVsUIDataSource pSearchSettings)
        {
            Utilities.SetValue(pSearchSettings,
                SearchSettingsDataSource.SearchStartTypeProperty.Name,
                (uint)VSSEARCHSTARTTYPE.SST_ONDEMAND);

            Utilities.SetValue(pSearchSettings,
                SearchSettingsDataSource.RestartSearchIfUnchangedProperty.Name, true);

            Utilities.SetValue(pSearchSettings,
                SearchSettingsDataSource.SearchWatermarkProperty.Name, "Enter XPath...");
        }

    }
}