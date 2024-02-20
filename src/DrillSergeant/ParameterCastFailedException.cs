using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DrillSergeant;

/// <summary>
/// Defines an exception that is thrown whenever the dynamic cast to a parameter type (e.g., context/input) fails.
/// </summary>
[ExcludeFromCodeCoverage]
public class ParameterCastFailedException : Exception
{
    public ParameterCastFailedException(Type targetType, string message)
        : base(message)
    {
        TargetType = targetType;
    }

    /// <summary>
    /// Gets the target type that the caster attempted to cast to.
    /// </summary>
    public Type TargetType { get; }
}
