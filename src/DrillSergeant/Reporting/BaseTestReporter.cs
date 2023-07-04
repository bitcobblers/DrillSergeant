using System;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Reporting;

/// <summary>
/// Defines the base type for test output reporters.
/// </summary>
public abstract class BaseTestReporter : ITestReporter
{
    protected TestOutputHelper Sink { get; }
    protected DecoyTestOutputHelper Decoy { get; }
    protected ITest Test { get; }

    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTestReporter"/> class.
    /// </summary>
    /// <param name="sink">The output sink for the test.</param>
    /// <param name="decoy">The decoy output for individual steps.</param>
    /// <param name="test">The current test being executed.</param>
    protected BaseTestReporter(TestOutputHelper sink, DecoyTestOutputHelper decoy, ITest test)
    {
        Sink = sink;
        Decoy = decoy;
        Test = test;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="BaseTestReporter"/> class.
    /// </summary>
    ~BaseTestReporter()
    {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public virtual string Output => Sink.Output;

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public abstract void WriteBlock(string label, object content);

    /// <inheritdoc />
    public abstract void WriteStepResult(string verb, string name, bool skipped, decimal elapsed, bool success, object? context);

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            Sink.Uninitialize();
        }

        _disposed = true;
    }
}
