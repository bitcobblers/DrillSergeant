namespace DrillSergeant;

/// <summary>
/// Defines a reporter used to output behavior results to the test runner.
/// </summary>
public interface ITestReporter : IDisposable
{
    /// <summary>
    /// Writes a block of content to the report.
    /// </summary>
    /// <param name="label">The label for the content.</param>
    /// <param name="content">The content to write.</param>
    void WriteBlock(string? label, object? content);

    /// <summary>
    /// Writes the result for a single step to the report.
    /// </summary>
    /// <param name="result">The step result to write.</param>
    void WriteStepResult(StepExecutionResult? result);
}