using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DrillSergeant;

[Serializable, ExcludeFromCodeCoverage]
public class BehaviorTimeoutException : Exception, ISerializable
{
    public BehaviorTimeoutException(int timeout)
        : base($"The behavior timed out after {timeout:N0}ms")
    {
        Timeout = timeout;
    }

    protected BehaviorTimeoutException(SerializationInfo info, StreamingContext context)
    {
        Timeout = info.GetInt32(nameof(Timeout));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Timeout), Timeout);
    }

    public int Timeout { get; private set; }
}