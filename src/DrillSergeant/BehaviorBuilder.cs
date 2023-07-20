using System.Collections.Generic;
using System.Threading;

namespace DrillSergeant;

public static class BehaviorBuilder
{
    private static readonly AsyncLocal<Behavior?> Instance = new();

    /// <summary>
    /// Gets the current behavior to test.
    /// </summary>
    public static Behavior Current => Instance.Value ?? new Behavior();

    /// <summary>
    /// Creates a new behavior to build.
    /// </summary>
    /// <param name="input">The input parameters for the behavior.</param>
    /// <returns>The new behavior to build.</returns>
    internal static Behavior Reset(object? input = null)
    {
        Instance.Value?.Dispose();
        Instance.Value = new Behavior().SetInput(input);

        return Instance.Value;
    }

    /// <summary>
    /// Resets the current behavior builder.
    /// </summary>
    /// <param name="input">The dictionary to use for input data.</param>
    internal static Behavior Reset(IDictionary<string, object?> input)
    {
        Instance.Value?.Dispose();
        Instance.Value = new Behavior().SetInput(input);

        return Instance.Value;
    }
}
