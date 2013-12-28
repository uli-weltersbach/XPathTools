using System;
using System.Windows.Automation;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration
{
    internal static class Extensions
    {
        public static AutomationElement FindDescendant(this AutomationElement ancestor, string descendantElementName)
        {
            AutomationElement match = ancestor.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, descendantElementName, PropertyConditionFlags.IgnoreCase));
            if (match == null)
                throw new NullReferenceException(string.Format("Didn't find element \"{0}\".", descendantElementName));
            return match;
        }

        public static AutomationElement LeftClick(this AutomationElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            Mouse.LeftClick(element.GetClickablePoint());
            return element;
        }
    }
}