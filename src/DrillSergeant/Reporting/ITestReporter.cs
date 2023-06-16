using System;

namespace DrillSergeant.Reporting;

public interface ITestReporter : IDisposable
{
    void WriteBlock(string label, object input);
    void WriteStepResult(string verb, string name, bool skipped, decimal elapsed, bool success, object? context);
}
