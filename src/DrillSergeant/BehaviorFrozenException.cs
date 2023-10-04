using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DrillSergeant;

[ExcludeFromCodeCoverage, Serializable]
public class BehaviorFrozenException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BehaviorFrozenException"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method that </param>
    public BehaviorFrozenException(string methodName)
        : base($"Cannot modify a behavior once it has been frozen.  The method '{methodName}' was called.")
    {
        MethodName = methodName;
    }

    protected BehaviorFrozenException(SerializationInfo info, StreamingContext context)
    {
        MethodName = info.GetString(nameof(MethodName))!;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(MethodName), MethodName);
    }

    /// <summary>
    /// Gets the name of the method that was called that triggered this exception.
    /// </summary>
    public string MethodName { get; set; }
}
