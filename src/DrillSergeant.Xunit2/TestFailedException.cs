using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant.Xunit2;

[Serializable, ExcludeFromCodeCoverage]
public class TestFailedException : Exception
{
    public TestFailedException(string message)
        : base(message)
    {
    }
}