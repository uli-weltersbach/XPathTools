using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.XPath;

namespace ReasonCodeExample.XPathInformation.Workbench
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
            if(string.IsNullOrWhiteSpace(xpath))
            {
                return new SearchResult[0];
            }

            var rootElement = _repository.GetRootElement();
            if(rootElement == null)
            {
                return new SearchResult[0];
            }

            try
            {
                var matches = rootElement.Document?.XPathEvaluate(xpath);
                var searchResults = _searchResultFactory.Parse(matches);
                return searchResults;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Error evaluating XPath.", ex);
            }
        }

        public void SetSearchResultCount(ICollection<SearchResult> searchResults)
        {
            if(searchResults == null)
            {
                SearchResultText.Text = null;
                return;
            }
            if(searchResults.Count == 0)
            {
                SearchResultText.Text = "No results.";
                return;
            }
            var countText = Math.Min(searchResults.Count, MaxSearchResultCount).ToString();
            if(searchResults.Count > MaxSearchResultCount)
            {
                countText += " of " + searchResults.Count;
            }
            var resultText = searchResults.Count == 1 ? "result" : "results";
            SearchResultText.Text = $"Showing {countText} {resultText}. Click to navigate.";
        }

        private void OnSearchResultClicked(object sender, MouseButtonEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            if(frameworkElement == null)
            {
                return;
            }
            var searchResult = (SearchResult)frameworkElement.DataContext;
            SearchResultSelected?.Invoke(this, searchResult);
        }

        private void ScrollSearchResults(object sender, MouseWheelEventArgs e)
        {
            if(e.Handled)
            {
                return;
            }
            e.Handled = true;
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = MouseWheelEvent;
            eventArg.Source = sender;
            var parent = (UIElement)((Control)sender).Parent;
            parent.RaiseEvent(eventArg);
        }
    }
}