using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Controls;
using ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration;
using ReasonCodeExample.XPathInformation.Workbench;

namespace ReasonCodeExample.XPathInformation.Tests.Workbench
{
    public class XPathWorkbenchAutomationModel
    {
        private readonly AutomationElement _mainWindow;

        public XPathWorkbenchAutomationModel(AutomationElement mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public AutomationElement ToolWindowPane => _mainWindow.FindDescendantByType<XPathWorkbench>();

        public bool IsVisible => ToolWindowPane != null;

        public string SearchResultCount
        {
            get
            {
                var searchResultCountElement = ToolWindowPane.FindDescendantByAutomationId("SearchResultCount");
                return searchResultCountElement.GetText();
            }
        }

        public string SearchText
        {
            get
            {
                var searchTextBox = ToolWindowPane.FindDescendantByType<TextBox>();
                var valuePattern = (ValuePattern)searchTextBox.GetCurrentPattern(ValuePattern.Pattern);
                return valuePattern.Current.Value;
            }
            set
            {
                var searchTextBox = ToolWindowPane.FindDescendantByType<TextBox>();
                var valuePattern = (ValuePattern)searchTextBox.GetCurrentPattern(ValuePattern.Pattern);
                valuePattern.SetValue(value);
            }
        }

        public void ClickSearchButton()
        {
            ToolWindowPane.FindDescendantByType<Button>().LeftClick();
        }

        public void Run(string xpath)
        {
            SearchText = xpath;
            ClickSearchButton();
        }

        public IEnumerable<string> GetResults()
        {
            return null;
        }
    }
}