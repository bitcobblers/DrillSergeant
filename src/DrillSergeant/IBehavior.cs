﻿using System;
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
public interface IBehavior : IEnumerable<IStep>, IDisposable
{
    /// <summary>
    /// Gets the context associated with the behavior.
    /// </summary>
    IDictionary<string, object?> Context { get; }

    /// <summary>
    /// Gets the input associated with the behavior.
    /// </summary>
    IDictionary<string, object?> Input { get; }

    /// <summary>
    /// Gets a value indicating whether the context should be logged between steps.
    /// </summary>
    bool LogContext { get; }

    /// <summary>
    /// Gets a value indicating whether the behavior has been frozen and unable to be configured further.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Attempting to call any of the configuration methods on a behavior once it has been thrown will result in a <see cref="BehaviorFrozenException"/> exception being thrown.
    /// </para>
    /// </remarks>
    bool IsFrozen { get; }
}
