using System.Collections.Generic;

namespace DrillSergeant;

/// <summary>
/// Defines the contract for a behavior.
/// </summary>
/// <remarks>
/// <para>
/// This is the base type for all behaviors.  In addition to providing common properties for all behaviors, it can also be enumerated for its associated steps.
/// </para>
/// </remarks>
public interface IBehavior : IEnumerable<IStep>
{
    /// <summary>
    /// Gets the context associated with the behavior.
    /// </summary>
    IDictionary<string,object?> Context { get; }

    /// <summary>
    /// Gets the input associted with the behavior.
    /// </summary>
    IDictionary<string,object?> Input { get; }

    /// <summary>
    /// Gets a value indicating whether the context should be logged between steps.
    /// </summary>
    bool LogContext { get; }
}
