using System;
using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant;

[ExcludeFromCodeCoverage]
public class NoCurrentBehaviorException : Exception
{
    public NoCurrentBehaviorException() :
        base("No current behavior defined.  Accessing the Current behavior can only be done within the scope of a behavior.")
    {
    }
}
