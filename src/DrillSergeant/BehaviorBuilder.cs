using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DrillSergeant;

public static class BehaviorBuilder
{
    private static readonly AsyncLocal<Stack<Behavior>> CurrentStack = new();

    /// <summary>
    /// Gets the current behavior to test.
    /// </summary>
    public static Behavior Current
    {
        get
        {
            var stack = GetCurrentStack();

            if (stack.Any())
            {
                return stack.Peek();
            }

            throw new NoCurrentBehaviorException();
        }
    }

    public static Behavior Build(Action<Behavior> configure)
    {
        var behavior = new Behavior();
        var stack = GetCurrentStack();

        stack.Push(behavior);

        try
        {
            configure?.Invoke(behavior);
        }
        finally
        {
            stack.Pop();
        }

        return behavior;
    }

    public static async Task<Behavior> BuildAsync(Func<Behavior, Task<Behavior>> configure)
    {
        var behavior = new Behavior();
        var stack = GetCurrentStack();

        stack.Push(behavior);

        try
        {
            await configure(behavior);
        }
        finally
        {
            stack.Pop();
        }

        return behavior;
    }

    internal static async Task PushAsync(Behavior behavior, Func<Task> action)
    {
        var stack = GetCurrentStack();

        stack.Push(behavior);

        try
        {
            await action();
        }
        finally
        {
            stack.Pop();
        }
    }

    internal static Stack<Behavior> GetCurrentStack()
    {
        Stack<Behavior>? value = CurrentStack.Value;

        if (value != null)
        {
            return value;
        }

        return CurrentStack.Value = new();
    }
}
