using System.Globalization;
using System.Text;
using Xunit.Abstractions;

namespace DrillSergeant.Xunit2;

/// <summary>
/// Defines a decoy output helper for xunit.
/// </summary>
/// <remarks>
/// By default the output to an instance of <see cref="ITestOutputHelper"/> is written to a messagebus.  The decoy is used to cache output at the step level so that it can be aggregated in the report.
/// </remarks>
internal class DecoyTestOutputHelper : ITestOutputHelper
{
    private readonly StringBuilder _buffer = new();

    /// <inheritdoc />
    public void WriteLine(string message) =>
        _buffer.AppendLine(message);

    /// <inheritdoc />
    public void WriteLine(string format, params object[] args) =>
        _buffer.AppendLine(string.Format(CultureInfo.CurrentCulture, format, args));

    /// <summary>
    /// Gets the current buffer content, clearing it in the process.
    /// </summary>
    /// <returns>The current buffer content.</returns>
    public string GetAndClear()
    {
        var result = _buffer.ToString();
        _buffer.Clear();
        return result;
    }
}
