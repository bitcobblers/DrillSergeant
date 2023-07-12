using System.Text;
using Xunit.Sdk;

namespace DrillSergeant.Xunit2;

/// <summary>
/// Defines a wrapper for the xunit <see cref="TestOutputHelper" /> that implements <see cref="TextWriter"/>
/// </summary>
internal class WrappedTestOutputHelper : TextWriter
{
    private readonly TestOutputHelper _outputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="WrappedTestOutputHelper"/> class.
    /// </summary>
    /// <param name="outputHelper">The xunit output sink to write to.</param>
    public WrappedTestOutputHelper(TestOutputHelper outputHelper) => _outputHelper = outputHelper;

    /// <inheritdoc />
    public override void WriteLine(string? value)
    {
        _outputHelper.WriteLine(value);
    }

    /// <inheritdoc />
    public override Encoding Encoding => Encoding.UTF8;

    /// <summary>
    /// Gets the current output from the sink.
    /// </summary>
    public string Output => _outputHelper.Output;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _outputHelper.Uninitialize();
    }
}