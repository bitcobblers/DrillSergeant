using Newtonsoft.Json;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace DrillSergeant.Reporting;

public class RawTestReporter : BaseTestReporter
{
    public RawTestReporter(TestOutputHelper sink, DecoyTestOutputHelper decoy, ITest test)
        : base(sink, decoy, test)
    {
    }

    /// <inheritdoc />
    public override void WriteBlock(string label, object context)
    {
        var serializationSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Error = (s, e) =>
            {
                e.ErrorContext.Handled = true;
            }
        };

        var serializedContext = JsonConvert.SerializeObject(context, serializationSettings);
        sink.WriteLine($"{label}: {serializedContext}");
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
