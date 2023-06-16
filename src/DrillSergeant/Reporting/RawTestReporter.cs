using Newtonsoft.Json;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Reporting;

/// <summary>
/// Defines a test output reporter that writes raw text to the output.
/// </summary>
public class RawTestReporter : BaseTestReporter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RawTestReporter"/> class.
    /// </summary>
    /// <param name="sink">The output sink for the test.</param>
    /// <param name="decoy">The decoy output for individual steps.</param>
    /// <param name="test">The current test being executed.</param>
    public RawTestReporter(TestOutputHelper sink, DecoyTestOutputHelper decoy, ITest test)
        : base(sink, decoy, test)
    {
    }

    /// <inheritdoc />
    public override void WriteBlock(string label, object content)
    {
        var serializationSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Error = (s, e) =>
            {
                e.ErrorContext.Handled = true;
            }
        };

        var serializedContent = JsonConvert.SerializeObject(content, serializationSettings);
        sink.WriteLine($"{label}: {serializedContent}");
        sink.WriteLine(string.Empty);
    }

    /// <inheritdoc />
    public override void WriteStepResult(string verb, string name, bool skipped, decimal elapsed, bool success, object? context)
    {
        var icon = success ? "✅" : "❎";
        var output = decoy.GetAndClear();

        if (skipped)
        {
            sink.WriteLine($"☐ {verb} (skipped due to previous failure): {name}");
        }
        else
        {
            sink.WriteLine($"{icon} {verb}: {name} took {elapsed:N2}s");

            if (context != null)
            {
                WriteBlock("Context", context);
            }

            if (string.IsNullOrWhiteSpace(output) == false)
            {
                sink.WriteLine("Output:");
                sink.WriteLine(output);
            }
        }
    }
}
