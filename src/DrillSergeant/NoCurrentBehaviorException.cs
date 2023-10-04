using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DrillSergeant;

/// <summary>
/// Defines an exception that is thrown whenever attempting to access the current behavior outside of a behavior test.
/// </summary>
[ExcludeFromCodeCoverage, Serializable]
public class NoCurrentBehaviorException : Exception
{
    public NoCurrentBehaviorException(string memberName)
        : base($"Cannot access CurrentBehavior outside of execution.  The member '{memberName}' was called")
    {
        MemberName = memberName;
    }

    protected NoCurrentBehaviorException(SerializationInfo info, StreamingContext context)
    {
        MemberName = info.GetString(nameof(MemberName))!;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(MemberName), MemberName);
    }

    /// <summary>
    /// Gets the name of the member that was called that triggered this exception.
    /// </summary>
    public string MemberName { get; set; }
}