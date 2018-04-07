using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.XPath;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools.Workbench
{
    public partial class XPathWorkbench : UserControl
    {
        public const int MaxSearchResultCount = int.MaxValue;
        private readonly XmlRepository _repository;
        private readonly SearchResultFactory _searchResultFactory;

        internal XPathWorkbench(XmlRepository repository, SearchResultFactory searchResultFactory)
        {
            InitializeComponent();
            _repository = repository;
            _searchResultFactory = searchResultFactory;
            SearchResults = new ObservableCollection<SearchResult>();
        }

        public ObservableCollection<SearchResult> SearchResults
        {
            get;
        }

        public event EventHandler<SearchResult> SearchResultSelected;

        public IList<SearchResult> Search(string xpath)
        {
            if (string.IsNullOrWhiteSpace(xpath))
            {
                return new SearchResult[0];
            }

            var rootElement = _repository.GetRootElement();
            if (rootElement == null)
            {
                return new SearchResult[0];
            }

            try
            {
                var namespaceResolver = new SimpleXmlNamespaceResolver(rootElement.Document);
                var matches = rootElement.Document?.XPathEvaluate(xpath, namespaceResolver);
                var searchResults = _searchResultFactory.Parse(matches);
                return searchResults;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PackageResources.XPathEvaluationErrorText, ex);
            }
        }

        public void UpdateSearchResultText(ICollection<SearchResult> searchResults)
        {
            if (searchResults == null)
            {
                SearchResultText.Text = null;
                return;
            }
            if (searchResults.Count == 0)
            {
                SearchResultText.Text = PackageResources.NoResultsText;
                return;
            }
            if (searchResults.Count == 1)
            {
                SearchResultText.Text = string.Format(PackageResources.SingleResultText, searchResults.Count);
                return;
            }
            SearchResultText.Text = string.Format(PackageResources.MultipleResultText, searchResults.Count);
        }

        private void OnSearchResultClicked(object sender, MouseButtonEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement == null)
            {
                return;
            }
            var searchResult = (SearchResult)frameworkElement.DataContext;
            SearchResultSelected?.Invoke(this, searchResult);
        }

        private void OnCopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            // TODO: Handle multi-select somehow.
            if (e.Item is SearchResult)
            {
                Clipboard.SetText(((SearchResult)e.Item).Value);
            }
        }
    }
}