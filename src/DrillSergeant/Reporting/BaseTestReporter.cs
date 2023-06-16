using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Reporting;

/// <summary>
/// Defines the base type for test output reporters.
/// </summary>
public abstract class BaseTestReporter : ITestReporter
{
    protected readonly TestOutputHelper sink;
    protected readonly DecoyTestOutputHelper decoy;
    protected readonly ITest test;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTestReporter"/> class.
    /// </summary>
    /// <param name="sink">The output sink for the test.</param>
    /// <param name="decoy">The decoy output for individual steps.</param>
    /// <param name="test">The current test being executed.</param>
    public BaseTestReporter(TestOutputHelper sink, DecoyTestOutputHelper decoy, ITest test)
    {
        this.sink = sink;
        this.decoy = decoy;
        this.test = test;
    }

    /// <inheritdoc />
    public virtual string Output => sink.Output;

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose() => sink.Uninitialize();
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize

    /// <inheritdoc />
    public abstract void WriteBlock(string label, object content);

    /// <inheritdoc />
    public abstract void WriteStepResult(string verb, string name, bool skipped, decimal elapsed, bool success, object? context);
}
