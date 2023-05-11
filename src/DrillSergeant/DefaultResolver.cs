using System;

namespace DrillSergeant;

public class DefaultResolver : IDependencyResolver
{
    public object Resolve(Type type)
    {
        return Activator.CreateInstance(type)!;
    }
}
