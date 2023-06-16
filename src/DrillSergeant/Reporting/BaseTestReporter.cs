using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Reporting;

public abstract class BaseTestReporter : ITestReporter
{
    protected readonly TestOutputHelper sink;
    protected readonly DecoyTestOutputHelper decoy;
    protected readonly ITest test;

    public BaseTestReporter(TestOutputHelper sink, DecoyTestOutputHelper decoy, ITest test)
    {
        this.sink = sink;
        this.decoy = decoy;
        this.test = test;
    }

    public string Output => sink.Output;

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose() => sink.Uninitialize();
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize

    /// <inheritdoc />
    public abstract void WriteBlock(string label, object input);

    /// <inheritdoc />
    public abstract void WriteStepResult(string verb, string name, bool skipped, decimal elapsed, bool success, object? context);
}
