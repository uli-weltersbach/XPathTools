using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.XPath;
using ReasonCodeExample.XPathInformation.Writers;

namespace ReasonCodeExample.XPathInformation.Workbench
{
    /// <summary>
    /// Interaction logic for XPathWorkbench.xaml
    /// </summary>
    public partial class XPathWorkbench : UserControl
    {
        private readonly XmlRepository _repository;

        internal XPathWorkbench(XmlRepository repository)
        {
            InitializeComponent();
            _repository = repository;
            SearchResults = new ObservableCollection<SearchResult>();
        }

        public ObservableCollection<SearchResult> SearchResults
        {
            get;
            set;
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
                var searchResults = Parse(matches);
                foreach(var searchResult in searchResults)
                {
                    SearchResults.Add(searchResult);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error evaluating XPath.", ex.ToString(), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private IList<SearchResult> Parse(object matches)
        {
            var searchResults = new List<SearchResult>();
            if (matches == null)
            {
                return searchResults;
            }
            if(matches is IEnumerable<XElement>)
            {
                var elements = (IEnumerable<XElement>)matches;
                foreach(var element in elements)
                {
                    var searchResult = new SearchResult {XPath = new AbsoluteXPathWriter().Write(element)};
                    searchResults.Add(searchResult);
                }
            }
            return searchResults;
        }
    }

    public class SearchResult
    {
        public string XPath
        {
            get;
            set;
        }

        public int? LineNumber
        {
            get;
            set;
        }

        public int? LinePosition
        {
            get;
            set;
        }
    }
}