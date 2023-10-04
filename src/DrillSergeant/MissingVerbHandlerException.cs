using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DrillSergeant;

/// <summary>
/// Defines an exception that is thrown when no DrillSergeant is unable to find a handler to execute.
/// </summary>
[Serializable, ExcludeFromCodeCoverage]
public class MissingVerbHandlerException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingVerbHandlerException"/> class.
    /// </summary>
    /// <param name="verb">The verb for the handler.</param>
    public MissingVerbHandlerException(string verb)
        : base($"Could not find any implementation for the verb ${verb}.")
    {
        Verb = verb;
    }

    protected MissingVerbHandlerException(SerializationInfo info, StreamingContext context)
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
    public string Verb { get; set; }
}