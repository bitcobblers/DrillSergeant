using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant;

/// <summary>
/// Thrown whenever the context is mapped twice within a single step.
/// </summary>
[ExcludeFromCodeCoverage, Serializable]
#pragma warning disable CA2229
public class ContextAlreadyMappedException : Exception
#pragma warning restore CA2229
{
    public ContextAlreadyMappedException()
        : base("Cannot remap context that has already been mapped")
    {
    }
}