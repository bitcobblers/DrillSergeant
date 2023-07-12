using System;
using System.Threading;

namespace DrillSergeant;

public static class BehaviorBuilder
{
    private static readonly AsyncLocal<Behavior?> Instance = new();

    /// <summary>
    /// Gets the current behavior to test.
    /// </summary>
    public static Behavior? CurrentBehavior => Instance.Value;

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
    /// Marks an object as being owned by the current behavior.
    /// </summary>
    /// <param name="instance">The object to take ownership of.</param>
    public static void Owns(IDisposable? instance)
    {
        Instance.Value?.Owns(instance);
    }

    /// <summary>
    /// Clears the current behavior.
    /// </summary>
    internal static void Clear()
    {
        Instance.Value?.Dispose();
        Instance.Value = null;
    }
}