﻿using System.Text;
using Xunit.Abstractions;

namespace DrillSergeant.Reporting;

/// <summary>
/// Defines a decoy output helper for xunit.
/// </summary>
/// <remarks>
/// By default the output to an instance of <see cref="ITestOutputHelper"/> is written to a messagebus.  The decoy is used to cache output at the step level so that it can be aggregated in the report.
/// </remarks>
public class DecoyTestOutputHelper : ITestOutputHelper
{
    private readonly StringBuilder buffer = new();

    /// <inheritdoc />
    public void WriteLine(string message) =>
        buffer.AppendLine(message);

    /// <inheritdoc />
    public void WriteLine(string format, params object[] args) =>
        buffer.AppendLine(string.Format(format, args));

    /// <summary>
    /// Gets the current buffer content, clearing it in the process.
    /// </summary>
    /// <returns>The current buffer content.</returns>
    public string GetAndClear()
    {
        var result = buffer.ToString();
        buffer.Clear();
        return result;
    }
}