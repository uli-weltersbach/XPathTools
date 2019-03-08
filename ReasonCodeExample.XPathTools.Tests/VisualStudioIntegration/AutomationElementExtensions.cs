using System;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Controls;

namespace ReasonCodeExample.XPathTools.Tests.VisualStudioIntegration
{
    internal static class AutomationElementExtensions
    {
        private const int DefaultTimeoutInSeconds = 15;

        public static AutomationElement FindDescendantByType<T>(this AutomationElement ancestor, int timeoutInSeconds = DefaultTimeoutInSeconds) where T : Control
        {
            var propertyCondition = new PropertyCondition(AutomationElement.ClassNameProperty, typeof(T).Name, PropertyConditionFlags.IgnoreCase);
            return FindDescendant(ancestor, propertyCondition, timeoutInSeconds);
        }

        public static AutomationElement FindDescendantByAutomationId(this AutomationElement ancestor, string automationId, int timeoutInSeconds = DefaultTimeoutInSeconds)
        {
            var propertyCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, automationId, PropertyConditionFlags.IgnoreCase);
            return FindDescendant(ancestor, propertyCondition, timeoutInSeconds);
        }

        public static AutomationElement FindDescendantByText(this AutomationElement ancestor, string descendantElementText, int timeoutInSeconds = DefaultTimeoutInSeconds)
        {
            var propertyCondition = new PropertyCondition(AutomationElement.NameProperty, descendantElementText, PropertyConditionFlags.IgnoreCase);
            return FindDescendant(ancestor, propertyCondition, timeoutInSeconds);
        }

        public static AutomationElement FindDescendant(this AutomationElement ancestor, Condition condition, int timeoutInSeconds = DefaultTimeoutInSeconds)
        {
            var timeout = DateTime.UtcNow.AddSeconds(timeoutInSeconds);
            var retryInterval = TimeSpan.FromSeconds(1);
            while(DateTime.UtcNow < timeout)
            {
                var match = ancestor.FindFirst(TreeScope.Descendants, condition);
                if(match == null)
                {
                    Thread.Sleep(retryInterval);
                }
                else
                {
                    return match;
                }
            }
            throw new TimeoutException($"Element \"{condition}\" wasn't found within {timeoutInSeconds} seconds.");
        }

        public static AutomationElement LeftClick(this AutomationElement element)
        {
            if(element == null)
            {
                throw new ArgumentNullException(nameof(element));
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
