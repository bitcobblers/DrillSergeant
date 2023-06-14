using System;
using System.Collections.Generic;

namespace DrillSergeant;

/// <summary>
/// Defines an interface to encapsulate different implementation of parameter casting.
/// </summary>
public interface IParameterCaster
{
    /// <summary>
    /// Casts a source dictionary to the specified type.
    /// </summary>
    /// <param name="source">The source data to cast.</param>
    /// <param name="type">The type to cast to.</param>
    /// <returns>The casted object.</returns>
    /// <remarks>
    /// <para>
    /// This interface is used by <see cref="BaseStep"/> for parameter resolution before executing a step.
    /// </para>
    /// <para>
    /// There are two implementations of this interface: <see cref="JsonParameterCaster"/>(obsolete) and <see cref="ReflectionParameterCaster"/>
    /// </para>
    /// <para>
    /// It's likely that this interface (along with <see cref="JsonParameterCaster"/> will be removed in the future.
    /// </para>
    /// </remarks>
    object Cast(IDictionary<string, object?> source, Type type);
}
