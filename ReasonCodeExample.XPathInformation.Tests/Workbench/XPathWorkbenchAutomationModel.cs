using System.Windows.Automation;
using System.Windows.Forms;
using ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration;
using ReasonCodeExample.XPathInformation.VisualStudioIntegration;
using TextBox = System.Windows.Controls.TextBox;

namespace ReasonCodeExample.XPathInformation.Tests.Workbench
{
    public class XPathWorkbenchAutomationModel
    {
        private readonly AutomationElement _mainWindow;

        public XPathWorkbenchAutomationModel(AutomationElement mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public AutomationElement ToolWindowPane => _mainWindow.FindDescendantByText(PackageResources.XPathWorkbenchWindowTitle);

        public bool IsVisible => ToolWindowPane != null;

        public string SearchResultText
        {
            get
            {
                var searchResultCountElement = ToolWindowPane.FindDescendantByAutomationId("SearchResultText");
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

        public void Search(string xpath)
        {
            SearchText = xpath;
            SendKeys.SendWait("{Enter}");
        }
    }
}