using System.Windows.Automation;
using ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration;

namespace ReasonCodeExample.XPathTools.Tests.Statusbar
{
    internal class StatusbarAutomationModel
    {
        private readonly AutomationElement _mainWindow;

        public StatusbarAutomationModel(AutomationElement mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public string GetText()
        {
            var liveTextBlock = _mainWindow.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "LiveTextBlock"));
            return liveTextBlock.GetText();
        }
    }
}
