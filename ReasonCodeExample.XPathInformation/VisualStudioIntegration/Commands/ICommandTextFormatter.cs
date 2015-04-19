namespace ReasonCodeExample.XPathInformation.VisualStudioIntegration.Commands
{
    internal interface ICommandTextFormatter
    {
        string Format(string xpath, int? elementCount);
    }
}