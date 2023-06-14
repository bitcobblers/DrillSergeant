using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DrillSergeant;

/// <summary>
/// Defines an exception that is thrown when no DrillSergeant is unable to find a handler to execute.
/// </summary>
[Serializable, ExcludeFromCodeCoverage]
public class MissingVerbHandlerException : Exception, ISerializable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingVerbHandlerException"/> class.
    /// </summary>
    /// <param name="verb">The verb for the handler.</param>
    public MissingVerbHandlerException(string verb) : base($"Could not find any implementation for the verb ${verb}.")
    {
        this.Verb = verb;
    }

    protected MissingVerbHandlerException(SerializationInfo info, StreamingContext context)
    {
        this.Verb = info.GetString(nameof(this.Verb))!;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(this.Verb), this.Verb);
    }

    /// <summary>
    /// Gets the verb associated with the exception.
    /// </summary>
    public string Verb { get; set; }
}