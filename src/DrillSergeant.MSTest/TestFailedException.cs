using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant.MSTest;

[Serializable, ExcludeFromCodeCoverage]
public class TestFailedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestFailedException"/> class.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <param name="innerException">An optional inner exception.</param>
    public TestFailedException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
