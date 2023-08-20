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

            throw new NoActiveBehaviorException();
        }
    }

    /// <summary>
    /// Builds a new behavior.
    /// </summary>
    /// <param name="configure">The callback to execute to configure the behavior.</param>
    /// <returns>The configured behavior.</returns>
    public static Behavior Build(Action<Behavior>? configure)
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

    /// <summary>
    /// Builds a new behavior asynchronously.
    /// </summary>
    /// <param name="configure">The callback to execute to configure the behavior.</param>
    /// <returns>The configured behavior.</returns>
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

    internal static Stack<Behavior> GetCurrentStack()
    {
        Stack<Behavior>? value = CurrentStack.Value;

        if (value != null)
        {
            return value;
        }

        return CurrentStack.Value = new Stack<Behavior>();
    }
}