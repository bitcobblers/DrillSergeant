using System;

namespace JustBehave;

public interface IDependencyResolver
{
    void Register(object instance);
    void Register(Type type, object instance);

    T Resolve<T>();
    object Resolve(Type type);
}