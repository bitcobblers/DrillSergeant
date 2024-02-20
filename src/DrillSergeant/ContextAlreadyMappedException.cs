using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant;

/// <summary>
/// Thrown whenever the context is mapped twice within a single step.
/// </summary>
[ExcludeFromCodeCoverage]
public class ContextAlreadyMappedException : Exception
{
    public ContextAlreadyMappedException()
        : base("Cannot remap context that has already been mapped")
    {
    }
}
