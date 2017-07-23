namespace ReasonCodeExample.XPathTools
{
    internal static class Registry
    {
        static Registry()
        {
            Current = new ServiceContainer();
        }

        public static ServiceContainer Current
        {
            get;
        }
    }
}