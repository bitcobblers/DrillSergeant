using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant;

/// <summary>
/// Defines an exception that is thrown whenever attempting to access the current behavior outside of a behavior test.
/// </summary>
[ExcludeFromCodeCoverage]
public class NoCurrentBehaviorException : Exception
{
    public NoCurrentBehaviorException(string memberName)
        : base($"Cannot access CurrentBehavior outside of execution.  The member '{memberName}' was called")
    {
        MemberName = memberName;
    }

    /// <summary>
    /// Gets the name of the member that was called that triggered this exception.
    /// </summary>
    public string MemberName { get; set; }
}
