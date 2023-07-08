using Xunit.Sdk;

namespace DrillSergeant.Xunit2;

public class XunitRawTestReporter : RawTestReporter
{
    private readonly TextWriter _sink;
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

    private XunitRawTestReporter(TextWriter sink, DecoyTestOutputHelper decoy)
        : base(sink)
    {
        _sink = sink;
        _decoy = decoy;
    }

    protected override void WriteStepResult(StepResult result)
    {
        base.WriteStepResult(result with
        {
            AdditionalOutput = _decoy.GetAndClear()
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _sink.Dispose();
    }
}