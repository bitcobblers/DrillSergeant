using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant.Xunit2;

[Serializable, ExcludeFromCodeCoverage]
public class TestFailedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestFailedException" /> class.
    /// </summary>
    /// <param name="message">The failure message.</param>
    public TestFailedException(string message)
        : base(message)
    {
    }
}