using System.Xml.Linq;

namespace ReasonCodeExample.XPathInformation
{
    internal class XPathRepository
    {
        private static readonly object Lock = new object();
        private static XObject _stored;

        public virtual void Put(XObject obj)
        {
            lock (Lock)
            {
                _stored = obj;
            }
        }

        public virtual XObject Get()
        {
            lock (Lock)
            {
                return _stored;
            }
        }
    }
}