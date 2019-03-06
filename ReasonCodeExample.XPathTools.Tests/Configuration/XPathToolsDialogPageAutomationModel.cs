using ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ReasonCodeExample.XPathTools.Tests.Configuration
{
    public class XPathToolsDialogPageAutomationModel
    {
        private readonly AutomationElement _mainWindow;

        public XPathToolsDialogPageAutomationModel(AutomationElement mainWindow)
        {
            _mainWindow = mainWindow;
        }

        private bool IsOpen
        {
            get
            {
                return OptionsDialog != null;
            }
        }

        private AutomationElement OptionsDialog
        {
            get
            {
                return _mainWindow.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Options", PropertyConditionFlags.IgnoreCase));
            }
        }

        private void Open()
        {
            var toolsMenu = OpenToolsMenu();

            var optionsDialog = OpenOptionsDialog(toolsMenu);

            SetXPathToolsSettingsFocus(optionsDialog);
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

            var optionsDialog = _mainWindow.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Options"));
            return optionsDialog;
        }

        private void SetXPathToolsSettingsFocus(AutomationElement optionsDialog)
        {
            var xpathToolsSettings = optionsDialog.FindDescendantByText("XPath Tools");
            xpathToolsSettings.SetFocus();
            xpathToolsSettings.LeftClick();
            var propertiesWindow = optionsDialog.FindDescendant(new PropertyCondition(AutomationElement.NameProperty, "Properties Window", PropertyConditionFlags.IgnoreCase));
            propertiesWindow.LeftClick();
        }

        private void Close()
        {
            if (IsOpen)
            {
                OptionsDialog.FindDescendantByText("OK").LeftClick();
            }
        }

        public void SetStatusbarXPathFormat(XPathFormat format)
        {
            // Ensure options dialog is closed before starting interaction sequence
            Close();

            // Open the XPath options dialog page
            Open();

            // This assumes that the XPath Tools settings page has focus
            for (var i = 0; i < 6; i++)
            {
                // Move to the last setting
                SendKeys.SendWait("{DOWN}");
            }
            // Select the desired format - requires all formats to start with a different letter!
            var firstLetter = format.ToString()[0].ToString();
            SendKeys.SendWait(firstLetter);

            Close();
        }
    }
}
