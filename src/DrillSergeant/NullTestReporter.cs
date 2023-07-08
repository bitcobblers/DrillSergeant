using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant;

// ReSharper disable once UnusedType.Global
[ExcludeFromCodeCoverage]
public class NullTestReporter : ITestReporter
{
    /// <inheritdoc />
#pragma warning disable CA1816
    public void Dispose()
#pragma warning restore CA1816
    {
    }

    /// <inheritdoc />
    public string Output => string.Empty;

    /// <inheritdoc />
    public void WriteBlock(string label, object content)
    {
    }

    /// <inheritdoc />
    public void WriteStepResult(StepResult result)
    {
    }
}