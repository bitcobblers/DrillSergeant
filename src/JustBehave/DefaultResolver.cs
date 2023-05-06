using System;
using System.Collections.Generic;

namespace JustBehave;

public class DefaultResolver : IDependencyResolver
{
    private readonly Dictionary<Type, object> _registrations = new();

    public void Register(object instance)
    {
        _registrations[instance.GetType()] = instance;
    }

    public void Register(Type type, object instance)
    {
        _registrations[type] = instance;
    }

    public T Resolve<T>()
    {
        if(_registrations.TryGetValue(typeof(T), out var instance))
        {
            return (T)instance;
        }

        return Activator.CreateInstance<T>();
    }

    public object Resolve(Type type)
    {
        if (_registrations.TryGetValue(type, out var instance))
        {
            return instance;
        }

        return Activator.CreateInstance(type)!;
    }
}
