using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DrillSergeant;

/// <summary>
/// Defines an exception that is thrown whenever the dynamic cast to a parameter type (e.g., context/input) fails.
/// </summary>
[Serializable, ExcludeFromCodeCoverage]
public class ParameterCastFailedException : Exception
{
    public ParameterCastFailedException(Type targetType, string message) 
        : base(message)
    {
        TargetType = targetType;
    }

    protected ParameterCastFailedException(SerializationInfo info, StreamingContext context)
    {
        TargetType = (Type)info.GetValue(nameof(TargetType), typeof(Type))!;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(TargetType), TargetType);
    }

    /// <summary>
    /// Gets the target type that the caster attempted to cast to.
    /// </summary>
    public Type TargetType { get; }
}