using System;

namespace DrillSergeant.Reporting;

public interface ITestReporter : IDisposable
{
    /// <summary>
    /// Gets the output of the test.
    /// </summary>
    string Output { get; }

    /// <summary>
    /// Writes a block of content to the report.
    /// </summary>
    /// <param name="label">The label for the content.</param>
    /// <param name="content">The content to write.</param>
    void WriteBlock(string label, object content);

    /// <summary>
    /// Writes the result for a single step to the report.
    /// </summary>
    /// <param name="verb">The verb of the step.</param>
    /// <param name="name">The name of the step.</param>
    /// <param name="skipped">True if the step was skipped.</param>
    /// <param name="elapsed">The duration of the step (in ms).</param>
    /// <param name="success">True if the step executed successfully.</param>
    /// <param name="context">The current context of the behavior.</param>
    void WriteStepResult(string verb, string name, bool skipped, decimal elapsed, bool success, object? context);
}
