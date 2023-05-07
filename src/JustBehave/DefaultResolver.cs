using System;

namespace JustBehave;

public class DefaultResolver : IDependencyResolver
{
    public object Resolve(Type type)
    {
        return Activator.CreateInstance(type)!;
    }
}
