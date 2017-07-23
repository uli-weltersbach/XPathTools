using System;
using System.Collections.Generic;

namespace ReasonCodeExample.XPathTools
{
  internal class ServiceContainer
  {
    private readonly IDictionary<Type, object> _services = new Dictionary<Type, object>();

    public void Set<T>(T instance)
    {
      _services.Add(typeof(T), instance);
    }

    public T Get<T>()
    {
      if(_services.ContainsKey(typeof(T)))
      {
        return (T)_services[typeof(T)];
      }
      return default(T);
    }
  }
}