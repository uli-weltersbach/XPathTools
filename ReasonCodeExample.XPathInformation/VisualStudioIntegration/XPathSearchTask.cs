using System;
using Microsoft.VisualStudio.Shell;
using ReasonCodeExample.XPathInformation.Workbench;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System.Linq;

namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration
{
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
            ThreadHelper.Generic.Invoke(() =>
            {
                control.SearchResults.Clear();
            });
            uint resultCount = 0;
            this.ErrorCode = VSConstants.S_OK;
            try
            {
                string xpath = this.SearchQuery.SearchString;
                var searchResults = control.Search(xpath);
                SearchCallback.ReportProgress(this, (uint)searchResults.Count, (uint)searchResults.Count);
                resultCount = (uint)searchResults.Count;
                ThreadHelper.Generic.Invoke(() =>
                {
                    foreach (var searchResult in searchResults.Take(XPathWorkbench.MaxSearchResultCount))
                    {
                        control.SearchResults.Add(searchResult);
                    }
                    control.SetSearchResultCount(searchResults);
                });
            }
            catch (Exception e)
            {
                this.ErrorCode = VSConstants.E_FAIL;
                ThreadHelper.Generic.Invoke(() =>
                {
                    control.SearchResultText.Text = e.ToString();
                });
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
