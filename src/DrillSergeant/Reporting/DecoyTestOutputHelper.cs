using System.Text;
using Xunit.Abstractions;

namespace DrillSergeant.Reporting;

public class DecoyTestOutputHelper : ITestOutputHelper
{
    private readonly StringBuilder buffer = new();

    public void WriteLine(string message) =>
        buffer.AppendLine(message);

    public void WriteLine(string format, params object[] args) =>
        buffer.AppendLine(string.Format(format, args));

    public string GetAndClear()
    {
        var result = buffer.ToString();
        buffer.Clear();
        return result;
    }
}
