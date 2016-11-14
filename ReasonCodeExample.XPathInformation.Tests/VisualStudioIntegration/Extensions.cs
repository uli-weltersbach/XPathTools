using System;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Controls;

namespace ReasonCodeExample.XPathInformation.Tests.VisualStudioIntegration
{
    internal static class Extensions
    {
        public static AutomationElement FindDescendantByType<T>(this AutomationElement ancestor, double timeoutInSeconds = 5d) where T : Control
        {
            var propertyCondition = new PropertyCondition(AutomationElement.ClassNameProperty, typeof(T).Name, PropertyConditionFlags.IgnoreCase);
            return FindDescendant(ancestor, timeoutInSeconds, propertyCondition);
        }


        public static AutomationElement FindDescendantByAutomationId(this AutomationElement ancestor, string automationId, double timeoutInSeconds = 5d)
        {
            var propertyCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, automationId, PropertyConditionFlags.IgnoreCase);
            return FindDescendant(ancestor, timeoutInSeconds, propertyCondition);
        }

        public static AutomationElement FindDescendantByText(this AutomationElement ancestor, string descendantElementText, double timeoutInSeconds = 5d)
        {
            var propertyCondition = new PropertyCondition(AutomationElement.NameProperty, descendantElementText, PropertyConditionFlags.IgnoreCase);
            return FindDescendant(ancestor, timeoutInSeconds, propertyCondition);
        }

        private static AutomationElement FindDescendant(AutomationElement ancestor, double timeoutInSeconds, PropertyCondition propertyCondition)
        {
            var timeout = DateTime.UtcNow.AddSeconds(timeoutInSeconds);
            var retryInterval = TimeSpan.FromSeconds(timeoutInSeconds / 5d);
            while(DateTime.UtcNow < timeout)
            {
                var match = ancestor.FindFirst(TreeScope.Descendants, propertyCondition);
                if(match == null)
                {
                    Thread.Sleep(retryInterval);
                }
                else
                {
                    return match;
                }
            }
            throw new TimeoutException(string.Format("Element \"{0}\" wasn't found within {1} seconds.", propertyCondition.Value, timeoutInSeconds));
        }

        public static AutomationElement LeftClick(this AutomationElement element)
        {
            if(element == null)
            {
                throw new ArgumentNullException("element");
            }
            Mouse.LeftClick(element.GetClickablePoint());
            return element;
        }

        public static string GetText(this AutomationElement element)
        {
            object patternObj;
            if (element.TryGetCurrentPattern(ValuePattern.Pattern, out patternObj))
            {
                var valuePattern = (ValuePattern)patternObj;
                return valuePattern.Current.Value;
            }
            if (element.TryGetCurrentPattern(TextPattern.Pattern, out patternObj))
            {
                var textPattern = (TextPattern)patternObj;
                return textPattern.DocumentRange.GetText(-1).TrimEnd('\r'); // often there is an extra '\r' hanging off the end.
            }
            else
            {
                return element.Current.Name;
            }
        }
    }
}