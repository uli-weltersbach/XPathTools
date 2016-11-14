using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.XPath;

namespace ReasonCodeExample.XPathInformation.Workbench
{
    public partial class XPathWorkbench : UserControl
    {
        private const int MaxSearchResultCount = 50;
        private readonly XmlRepository _repository;
        private readonly SearchResultFactory _searchResultFactory;

        internal XPathWorkbench(XmlRepository repository, SearchResultFactory searchResultFactory)
        {
            InitializeComponent();
            _repository = repository;
            _searchResultFactory = searchResultFactory;
            SearchResults = new ObservableCollection<SearchResult>();
            SearchResultList.Visibility = Visibility.Hidden;
            SearchResultList.PreviewMouseWheel += ScrollSearchResults;
        }

        public ObservableCollection<SearchResult> SearchResults
        {
            get;
        }

        public event EventHandler<SearchResult> SearchResultSelected;

        private void OnSearchKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Search();
            }
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void Search()
        {
            SearchResultList.Visibility = Visibility.Hidden;
            SearchResults.Clear();
            SearchResultText.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                return;
            }

            var rootElement = _repository.GetRootElement();
            if (rootElement == null)
            {
                return;
            }

            try
            {
                var matches = rootElement.Document?.XPathEvaluate(SearchTextBox.Text);
                var searchResults = _searchResultFactory.Parse(matches);
                SearchResultText.Text = FormatSearchResultCount(searchResults);
                foreach (var searchResult in searchResults.Take(MaxSearchResultCount))
                {
                    SearchResults.Add(searchResult);
                }
                SearchResultList.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                SearchResultText.Text = "Error evaluating XPath." + Environment.NewLine + Environment.NewLine + ex;
            }
        }

        private string FormatSearchResultCount(ICollection<SearchResult> searchResults)
        {
            var countText = Math.Min(searchResults.Count, MaxSearchResultCount).ToString();
            if (searchResults.Count > MaxSearchResultCount)
            {
                countText += " of " + searchResults.Count;
            }
            var resultText = searchResults.Count == 1 ? "result" : "results";
            return $"Showing {countText} {resultText}.";
        }

        private void OnSearchResultClicked(object sender, MouseButtonEventArgs e)
        {
            var listViewItem = sender as ListViewItem;
            if (listViewItem == null)
            {
                return;
            }
            var searchResult = (SearchResult)listViewItem.DataContext;
            OnSearchResultSelected(searchResult);
        }

        private void OnSearchResultSelected(SearchResult searchResult)
        {
            SearchResultSelected?.Invoke(this, searchResult);
        }

        private void ScrollSearchResults(object sender, MouseWheelEventArgs e)
        {
            if (e.Handled)
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