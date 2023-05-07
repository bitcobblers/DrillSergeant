using System;

namespace JustBehave;

public interface IDependencyResolver
{
    object Resolve(Type type);
}