using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant.MSTest.Reporting;

public class RawTestReporter : ITestReporter
{
    [ExcludeFromCodeCoverage]
    public void Dispose()
    {
    }

    [ExcludeFromCodeCoverage]
    public string Output => string.Empty;

    public void WriteBlock(string label, object content)
    {
        var serializationSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Error = (_, e) => e.ErrorContext.Handled = true
        };

        var serializedContent = JsonConvert.SerializeObject(content, serializationSettings);
        Console.WriteLine($"{label}: {serializedContent}");
        Console.WriteLine(string.Empty);
    }

    public void WriteStepResult(string verb, string name, bool skipped, decimal elapsed, bool success, object? context)
    {
        var icon = success ? "✅" : "❎";

        if (skipped)
        {
            Console.WriteLine($"⏩ {verb} (skipped due to previous failure): {name}");
        }
        else
        {
            Console.WriteLine($"{icon} {verb}: {name} took {elapsed:N2}s");

            if (context != null)
            {
                WriteBlock("Context", context);
            }
        }
    }
}