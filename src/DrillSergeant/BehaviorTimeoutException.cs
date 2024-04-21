using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant;

[ExcludeFromCodeCoverage]
public class BehaviorTimeoutException : Exception
{
    public BehaviorTimeoutException(int timeout)
        : base($"The behavior timed out after {timeout:N0}ms")
    {
        Timeout = timeout;
    }

    public int Timeout { get; private set; }
}
