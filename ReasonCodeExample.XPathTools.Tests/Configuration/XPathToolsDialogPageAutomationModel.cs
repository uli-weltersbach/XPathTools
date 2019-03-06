using ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;
using System;
using System.Threading;
using System.Windows.Automation;

namespace ReasonCodeExample.XPathTools.Tests.Configuration
{
    public class XPathToolsDialogPageAutomationModel
    {
        private readonly AutomationElement _mainWindow;

        public XPathToolsDialogPageAutomationModel(AutomationElement mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public bool IsOpen
        {
            get;
        }

        public void Open()
        {
            var toolsMenu = OpenToolsMenu();
            var optionsDialog = OpenOptionsDialog(toolsMenu);
            var xpathToolsSettings = optionsDialog.FindDescendantByText("XPath Tools");
            xpathToolsSettings.SetFocus();
            xpathToolsSettings.LeftClick();
            var statusbarSetting = optionsDialog.FindDescendantByText("Statusbar XPath format");
            statusbarSetting.SetFocus();
        }

        private AutomationElement OpenToolsMenu()
        {
            var toolsMenu = FindMenuItem("Tools", _mainWindow);
            toolsMenu.LeftClick();
            return toolsMenu;
        }

        private AutomationElement FindMenuItem(string menuItemText, AutomationElement ancestor)
        {
            var className = "MenuItem";
            var classNameCondition = new PropertyCondition(AutomationElement.ClassNameProperty, className, PropertyConditionFlags.IgnoreCase);
            var nameCondition = new PropertyCondition(AutomationElement.NameProperty, menuItemText, PropertyConditionFlags.IgnoreCase);
            var condition = new AndCondition(classNameCondition, nameCondition);
            return ancestor.FindDescendant(condition);
        }

        private AutomationElement OpenOptionsDialog(AutomationElement toolsMenu)
        {
            var optionsMenuEntry = FindMenuItem("Options...", toolsMenu);
            optionsMenuEntry.LeftClick();

            var optionsDialog = AutomationElement.FocusedElement;
            return optionsDialog;
        }

        public void Close()
        {
        }

        public void SetStatusbarXPathFormat(XPathFormat format)
        {
            if (!IsOpen)
            {
                Open();
            }
            Close();
        }
    }
}
