using System;
using System.Collections.ObjectModel;
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
            if(string.IsNullOrWhiteSpace(SearchTextBox.Text))
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
                var matches = currentElement?.Document?.XPathEvaluate(SearchTextBox.Text);
                var searchResults = _searchResultFactory.Parse(matches);
                foreach(var searchResult in searchResults)
                {
                    SearchResults.Add(searchResult);
                }
                SearchResultList.Visibility = Visibility.Visible;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error evaluating XPath.", ex.ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}