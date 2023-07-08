namespace DrillSergeant.MSTest;

[Serializable]
public class TestFailedException : Exception
{
    public TestFailedException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}