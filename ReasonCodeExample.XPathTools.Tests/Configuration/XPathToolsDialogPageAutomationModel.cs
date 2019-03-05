using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration;
using ReasonCodeExample.XPathTools.VisualStudioIntegration;

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
            // "Alt + t" to open "Tools"-menu
            SendKeys.SendWait("%{t}");
            // "o" to open "Options..."
            SendKeys.SendWait("o");
            // "Ctrl + E" to activate option search
            SendKeys.SendWait("^{e}");
            // Search for "XPath Tools"
            SendKeys.SendWait("XPath Tools");
            SendKeys.SendWait("{ENTER}");
            // Interact with the XPath Tools settings
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}");
            SendKeys.SendWait("{TAB}");
            var firstLetterOfDesiredStatusbarXPathFormatOption = XPathFormat.Distinct.ToString()[0].ToString();
            SendKeys.SendWait(firstLetterOfDesiredStatusbarXPathFormatOption);
            // Close the dialog
            //SendKeys.SendWait("{ENTER}");
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
