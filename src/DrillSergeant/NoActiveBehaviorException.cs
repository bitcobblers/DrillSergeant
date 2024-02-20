using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant;

[ExcludeFromCodeCoverage, Serializable]
#pragma warning disable CA2229
public class NoActiveBehaviorException : Exception
#pragma warning restore CA2229
{
    public NoActiveBehaviorException()
        : base("No current behavior defined.  Accessing the Current behavior can only be done within the scope of a behavior.")
    {
    }
}