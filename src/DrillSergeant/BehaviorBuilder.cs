using System;
using System.Collections.Generic;
using System.Threading;

namespace DrillSergeant;

public static class BehaviorBuilder
{
    private static readonly AsyncLocal<Behavior?> Instance = new();

    /// <summary>
    /// Gets the current behavior to test.
    /// </summary>
    public static Behavior? Current => Instance.Value;

    /// <summary>
    /// Creates a new behavior to build.
    /// </summary>
    /// <param name="input">The input parameters for the behavior.</param>
    /// <returns>The new behavior to build.</returns>
    public static Behavior New(object? input = null)
    {
        Instance.Value = new Behavior(input);

        return Instance.Value;
    }

    /// <summary>
    /// Clears the current behavior.
    /// </summary>
    internal static void Reset(IDictionary<string, object?> input)
    {
        Instance.Value?.Dispose();
        Instance.Value = null;
    }
}
