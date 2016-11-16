using System;
using System.Runtime.InteropServices;
using System.Windows;
using EnvDTE;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Ninject;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Resources;
using ReasonCodeExample.XPathInformation.Workbench;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
    [Guid(Symbols.ToolWindowIDs.XPathWorkbench)]
    internal sealed class XPathWorkbenchWindow : ToolWindowPane
    {
        public XPathWorkbenchWindow()
            : base(null)
        {
            Caption = VSPackage.XPathWorkbenchWindowTitle;
            var workbench = new XPathWorkbench(Registry.Current.Get<XmlRepository>(), Registry.Current.Get<SearchResultFactory>());
            workbench.SearchResultSelected += GoToSearchResult;
            Content = workbench;
        }

        public override bool SearchEnabled => true;

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
                MessageBox.Show(VSPackage.XPathWorkbenchNavigationErrorText, VSPackage.XPathWorkbenchNavigationErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public override IVsSearchTask CreateSearch(uint dwCookie, IVsSearchQuery pSearchQuery, IVsSearchCallback pSearchCallback)
        {
            if(pSearchQuery == null || pSearchCallback == null)
            {
                return null;
            }
            return new XPathSearchTask(dwCookie, pSearchQuery, pSearchCallback, this);
        }

        public override void ClearSearch()
        {
            var control = (XPathWorkbench)Content;
            control.SearchResults.Clear();
            control.SearchResultText.Text = null;
        }

        public override void ProvideSearchSettings(IVsUIDataSource searchSettings)
        {
            Utilities.SetValue(searchSettings, SearchSettingsDataSource.SearchStartTypeProperty.Name, (uint)VSSEARCHSTARTTYPE.SST_ONDEMAND);

            Utilities.SetValue(searchSettings, SearchSettingsDataSource.RestartSearchIfUnchangedProperty.Name, true);

            Utilities.SetValue(searchSettings, SearchSettingsDataSource.SearchWatermarkProperty.Name, VSPackage.XPathWorkbenchSearchWatermark);
        }
    }
}