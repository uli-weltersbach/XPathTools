using Ninject;

namespace ReasonCodeExample.XPathInformation
{
    internal static class Registry
    {
        static Registry()
        {
            Current = new StandardKernel();
        }

        public static IKernel Current
        {
            get;
            private set;
        }
    }
}