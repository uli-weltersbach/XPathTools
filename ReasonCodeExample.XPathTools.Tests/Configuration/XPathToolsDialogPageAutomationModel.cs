using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
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

        }

        public void Close()
        {

        }

        public void SetStatusbarXPathFormat(XPathFormat format)
        {

        }
    }
}
