﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DrillSergeant;

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
