using System;

namespace DrillSergeant;

public interface IDependencyResolver
{
    object Resolve(Type type);
}