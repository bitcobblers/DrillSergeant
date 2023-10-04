using System.Runtime.CompilerServices;

namespace DrillSergeant;

// Based on: https://devblogs.microsoft.com/pfxteam/asynclazyt/

/// <summary>
/// Defines an implementation of <see cref="Lazy{T}"/> that supports asynchronous resolution.
/// </summary>
/// <typeparam name="T">The type to represent</typeparam>
public class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<T> valueFactory)
        : base(() => Task.Factory.StartNew(valueFactory))
    {
    }

    public AsyncLazy(Func<Task<T>> taskFactory)
        : base(() => Task.Factory.StartNew(taskFactory).Unwrap())
    {
    }

    public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
}
