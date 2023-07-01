using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Reporting;

/// <summary>
/// Defines the base type for test output reporters.
/// </summary>
public abstract class BaseTestReporter : ITestReporter
{
    protected readonly TestOutputHelper _sink;
    protected readonly DecoyTestOutputHelper _decoy;
    protected readonly ITest _test;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTestReporter"/> class.
    /// </summary>
    /// <param name="sink">The output sink for the test.</param>
    /// <param name="decoy">The decoy output for individual steps.</param>
    /// <param name="test">The current test being executed.</param>
    protected BaseTestReporter(TestOutputHelper sink, DecoyTestOutputHelper decoy, ITest test)
    {
        _sink = sink;
        _decoy = decoy;
        _test = test;
    }

    /// <inheritdoc />
    public virtual string Output => _sink.Output;

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose() => _sink.Uninitialize();
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize

    /// <inheritdoc />
    public abstract void WriteBlock(string label, object content);

    /// <inheritdoc />
    public abstract void WriteStepResult(string verb, string name, bool skipped, decimal elapsed, bool success, object? context);
}
