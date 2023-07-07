namespace DrillSergeant.NUnit3.Reporting
{
    public class RawTestReporter : ITestReporter
    {
        private readonly TextWriter _writer;

        public RawTestReporter(TextWriter writer) => _writer = writer;

        public string Output => string.Empty;

        public void Dispose()
        {
        }

        public void WriteBlock(string label, object content)
        {
        }

        public void WriteStepResult(string verb, string name, bool skipped, decimal elapsed, bool success, object? context)
        {
        }
    }
}