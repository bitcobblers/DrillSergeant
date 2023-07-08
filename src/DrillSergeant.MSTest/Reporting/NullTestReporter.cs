using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant.MSTest.Reporting
{
    [ExcludeFromCodeCoverage]
    public class NullTestReporter : ITestReporter
    {
        public void Dispose()
        {
        }

        public string Output => string.Empty;
        public void WriteBlock(string label, object content)
        {
        }

        public void WriteStepResult(string verb, string name, bool skipped, decimal elapsed, bool success, object? context)
        {
        }
    }
}