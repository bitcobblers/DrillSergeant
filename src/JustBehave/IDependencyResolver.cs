using System;

namespace JustBehave
{
    public interface IDependencyResolver
    {
        T Resolve<T>();

        object Resolve(Type type);
    }
}