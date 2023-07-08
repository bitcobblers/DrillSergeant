using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DrillSergeant.MSTest.Reporting;

public class RawTestReporter : ITestReporter
{
    private readonly TestContext _context;

    public RawTestReporter(TestContext context) => _context = context;

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
        _context.WriteLine($"{label}: {serializedContent}");
        _context.WriteLine(string.Empty);
    }

    public void WriteStepResult(string verb, string name, bool skipped, decimal elapsed, bool success, object? context)
    {
        var icon = success ? "✅" : "❎";

        if (skipped)
        {
            _context.WriteLine($"⏩ {verb} (skipped due to previous failure): {name}");
        }
        else
        {
            _context.WriteLine($"{icon} {verb}: {name} took {elapsed:N2}s");

            if (context != null)
            {
                WriteBlock("Context", context);
            }
        }
    }
}