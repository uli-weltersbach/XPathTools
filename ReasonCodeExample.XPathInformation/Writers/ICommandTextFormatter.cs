namespace ReasonCodeExample.XPathInformation.Writers
{
    internal interface ICommandTextFormatter
    {
        string Format(string xpath, int? elementCount);
    }
}