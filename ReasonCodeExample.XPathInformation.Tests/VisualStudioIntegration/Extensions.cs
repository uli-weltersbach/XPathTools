using System;
using System.Threading;
using System.Windows.Automation;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration
{
    internal static class Extensions
    {
        public static AutomationElement FindDescendant(this AutomationElement ancestor, string descendantElementName, double timeoutInSeconds = 5d)
        {
            var timeout = DateTime.UtcNow.AddSeconds(timeoutInSeconds);
            var retryInterval = TimeSpan.FromSeconds(timeoutInSeconds/5d);
            while (DateTime.UtcNow < timeout)
            {
                var match = ancestor.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, descendantElementName, PropertyConditionFlags.IgnoreCase));
                if (match == null)
                    Thread.Sleep(retryInterval);
                else
                    return match;
            }
            throw new TimeoutException(string.Format("Element \"{0}\" wasn't found within {1} seconds.", descendantElementName, timeoutInSeconds));
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