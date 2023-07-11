using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant.MSTest;

[Serializable, ExcludeFromCodeCoverage]
public class TestFailedException : Exception
{
    public TestFailedException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}