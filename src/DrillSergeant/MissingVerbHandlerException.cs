using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DrillSergeant;

/// <summary>
/// Defines an exception that is thrown when no DrillSergeant is unable to find a handler to execute.
/// </summary>
[ExcludeFromCodeCoverage]
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

    /// <summary>
    /// Gets the verb associated with the exception.
    /// </summary>
    public string Verb { get; set; }
}
