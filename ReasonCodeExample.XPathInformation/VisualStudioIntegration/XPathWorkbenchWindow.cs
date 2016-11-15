using System;
using System.Runtime.InteropServices;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Ninject;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands;
using ReasonCodeExample.XPathInformation.Workbench;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Linq;

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

        public override bool SearchEnabled
        {
            get { return true; }
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
        }

        public override void ProvideSearchSettings(IVsUIDataSource pSearchSettings)
        {
            Utilities.SetValue(pSearchSettings,
                SearchSettingsDataSource.SearchStartTypeProperty.Name,
                (uint)VSSEARCHSTARTTYPE.SST_ONDEMAND);
            //Utilities.SetValue(pSearchSettings,
            //    SearchSettingsDataSource.RestartSearchIfUnchangedProperty.Name,
            //    (uint)1);
        }

        internal class XPathSearchTask : VsSearchTask
        {
            private XPathWorkbenchWindow m_toolWindow;

            public XPathSearchTask(uint dwCookie, IVsSearchQuery pSearchQuery, IVsSearchCallback pSearchCallback, XPathWorkbenchWindow toolwindow)
                : base(dwCookie, pSearchQuery, pSearchCallback)
            {
                m_toolWindow = toolwindow;
            }

            protected override void OnStartSearch()
            {
                XPathWorkbench control = (XPathWorkbench)m_toolWindow.Content;

                uint resultCount = 0;
                this.ErrorCode = VSConstants.S_OK;
                try
                {
                    string xpath = this.SearchQuery.SearchString;
                    var results = control.Search(xpath);
                    SearchCallback.ReportProgress(this, (uint)results.Count, (uint)results.Count);
                    resultCount = (uint)results.Count;
                    ThreadHelper.Generic.Invoke(() =>
                    {
                        foreach (var searchResult in results.Take(XPathWorkbench.MaxSearchResultCount))
                        {
                            control.SearchResults.Add(searchResult);
                        }
                    });
                }
                catch (Exception e)
                {
                    this.ErrorCode = VSConstants.E_FAIL;
                }
                finally
                {
                    this.SearchResults = resultCount;
                }

                // Call the implementation of this method in the base class.   
                // This sets the task status to complete and reports task completion.   
                base.OnStartSearch();
            }

            protected override void OnStopSearch()
            {
                this.SearchResults = 0;
            }
        }

    }
}