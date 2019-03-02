using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace ReasonCodeExample.XPathTools.Workbench
{
    internal class XPathSearchTask : VsSearchTask
    {
        private readonly XPathWorkbenchWindow _workbenchWindow;

        public XPathSearchTask(uint taskIdCookie, IVsSearchQuery searchQuery, IVsSearchCallback searchCallback, XPathWorkbenchWindow workbenchWindow)
            : base(taskIdCookie, searchQuery, searchCallback)
        {
            _workbenchWindow = workbenchWindow;
        }

        protected override void OnStartSearch()
        {
            Task.WaitAll(SearchAsync());
        }

        private async System.Threading.Tasks.Task SearchAsync()
        {
            var workbench = (XPathWorkbench)_workbenchWindow.Content;
            ThreadHelper.Generic.Invoke(() => { workbench.SearchResults.Clear(); });
            ErrorCode = VSConstants.S_OK;
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var xpath = SearchQuery.SearchString;
                var searchResults = workbench.Search(xpath);
                SearchResults = (uint)searchResults.Count;
                ThreadHelper.Generic.Invoke(() => { UpdateSearchResults(searchResults, workbench); });
            }
            catch(Exception e)
            {
                ErrorCode = VSConstants.E_FAIL;
                ThreadHelper.Generic.Invoke(() => { workbench.SearchResultText.Text = e.Message; });
            }

            // Call the implementation of this method in the base class.   
            // This sets the task status to complete and reports task completion.   
            base.OnStartSearch();
        }

        private static void UpdateSearchResults(IList<SearchResult> searchResults, XPathWorkbench workbench)
        {
            foreach(var searchResult in searchResults.Take(XPathWorkbench.MaxSearchResultCount))
            {
                workbench.SearchResults.Add(searchResult);
            }
            workbench.UpdateSearchResultText(searchResults);
        }

        protected override void OnStopSearch()
        {
            this.SearchResults = 0;
        }
    }
}
