using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DrillSergeant;

/// <summary>
/// Defines an exception that is thrown when DrillSergeant is unable to determine which handler to execute.
/// </summary>
[Serializable, ExcludeFromCodeCoverage]
public class AmbiguousVerbHandlerException : Exception, ISerializable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AmbiguousVerbHandlerException"/> class.
    /// </summary>
    /// <param name="verb">The verb for the handler.</param>
    public AmbiguousVerbHandlerException(string verb) : base($"Cannot pick an implementation for the verb ${verb}.  Two or more candidates have the same number of parameters.")
    {
        Verb = verb;
    }

    protected AmbiguousVerbHandlerException(SerializationInfo info, StreamingContext context)
    {
        Verb = info.GetString(nameof(Verb))!;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Verb), Verb);
    }

    /// <summary>
    /// Gets the verb associated with the exception.
    /// </summary>
    public string Verb { get; private set; }
}
