using System;
using System.Windows.Automation;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration
{
    internal static class Extensions
    {
        public static AutomationElement FindDescendant(this AutomationElement ancestor, string descendantElementName)
        {
            return ancestor.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, descendantElementName, PropertyConditionFlags.IgnoreCase));
        }

        public static AutomationElement LeftClick(this AutomationElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            InputAutomation.LeftClick(element.GetClickablePoint());
            return element;
        }
    }
}