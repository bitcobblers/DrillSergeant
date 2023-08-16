using Xunit.Sdk;

namespace DrillSergeant.Xunit2;

internal class XunitRawTestReporter : RawTestReporter
{
    private readonly WrappedTestOutputHelper _sink;
    private readonly DecoyTestOutputHelper _decoy;

    /// <summary>
    /// Initializes a new instance of the <see cref="XunitRawTestReporter"/> class.
    /// </summary>
    /// <param name="sink">The writer for the sink to write to.</param>
    /// <param name="decoy">A decoy sink to write step output to.</param>
    public XunitRawTestReporter(TestOutputHelper sink, DecoyTestOutputHelper decoy)
        : this(new WrappedTestOutputHelper(sink), decoy)
    {
    }

    private XunitRawTestReporter(WrappedTestOutputHelper sink, DecoyTestOutputHelper decoy)
        : base(sink)
    {
        _sink = sink;
        _decoy = decoy;
    }

    /// <summary>
    /// Gets the current output from the sink.
    /// </summary>
    public string Output => _sink.Output;

    /// <inheritdoc />
    public override void WriteStepResult(StepExecutionResult? result)
    {
        if (result == null)
        {
            return;
        }

        base.WriteStepResult(result with
        {
            AdditionalOutput = _decoy.GetAndClear()
        });
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _sink.Dispose();
    }
}