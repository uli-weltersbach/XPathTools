using System.Collections.Generic;
using System.Windows.Automation;
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

        public AutomationElement ToolWindowPane => _mainWindow.FindDescendant<XPathWorkbench>();

        public bool IsVisible => ToolWindowPane != null;

        public string SearchResultCount
        {
            get
            {
                var searchResultCountElement = ToolWindowPane?.FindDescendant("SearchResultCount");
                return searchResultCountElement?.ToString();
            }
        }

        public void Run(string xpath)
        {
            
        }

        public IEnumerable<string> GetResults()
        {
            return null;
        }
    }
}