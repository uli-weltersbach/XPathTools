using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ReasonCodeExample.XPathInformation.Workbench
{
    public partial class XPathWorkbench : UserControl
    {
        private readonly XmlRepository _repository;
        private readonly SearchResultFactory _searchResultFactory;

        internal XPathWorkbench(XmlRepository repository, SearchResultFactory searchResultFactory)
        {
            InitializeComponent();
            _repository = repository;
            _searchResultFactory = searchResultFactory;
            SearchResults = new ObservableCollection<SearchResult>();
            SearchResultList.Visibility = Visibility.Hidden;
        }

        public ObservableCollection<SearchResult> SearchResults
        {
            get;
        }

        public event EventHandler<SearchResult> SearchResultSelected;

        private void OnSearchKeyDownHandler(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
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
            SearchResultCount.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                return;
            }

            var currentElement = _repository.Get() as XNode;
            if(currentElement == null)
            {
                return;
            }

            try
            {
                SearchResultCount.Text = "Working...";
                var matches = currentElement?.Document?.XPathEvaluate(SearchTextBox.Text);
                var searchResults = _searchResultFactory.Parse(matches);
                var resultText = searchResults.Count == 1 ? "result" : "results";
                SearchResultCount.Text = $"{searchResults.Count} {resultText}.";
                foreach (var searchResult in searchResults)
                {
                    SearchResults.Add(searchResult);
                }
                SearchResultList.Visibility = Visibility.Visible;
            }
            catch(Exception ex)
            {
                SearchResultCount.Text = string.Empty;
                MessageBox.Show("Error evaluating XPath.", ex.ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OnSearchResultClicked(object sender, MouseButtonEventArgs e)
        {
            var listViewItem = sender as ListViewItem;
            if(listViewItem == null)
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
    }
}