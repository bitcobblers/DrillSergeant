using System;
using System.IO;
using Newtonsoft.Json;

namespace DrillSergeant;

public class RawTestReporter : ITestReporter
{
    private readonly TextWriter _writer;

    public RawTestReporter(TextWriter writer) => _writer = writer;

    ~RawTestReporter() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public virtual string Output => string.Empty;

    /// <inheritdoc />
    public virtual void WriteBlock(string label, object content)
    {
        var serializationSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Error = (_, e) => e.ErrorContext.Handled = true
        };

        var serializedContent = JsonConvert.SerializeObject(content, serializationSettings);
        _writer.WriteLine($"{label}: {serializedContent}");
        _writer.WriteLine(string.Empty);
    }

    /// <inheritdoc />
    public virtual void WriteStepResult(string verb, string name, bool skipped, decimal elapsed, bool success, object? context)
    {
        WriteStepResult(new StepResult
        {
            Verb = verb,
            Name = name,
            Skipped = skipped,
            Success = success,
            PreviousStepsFailed = false,
            Elapsed = elapsed,
            Context = context
        });
    }

    protected virtual void WriteStepResult(StepResult result)
    {
        var icon = result.Success ? "✅" : "❎";

        if (result.Skipped)
        {
            _writer.WriteLine($"⏩ {result.Verb} (skipped due to previous failure): {result.Name}");
        }
        else
        {
            _writer.WriteLine($"{icon} {result.Verb}: {result.Name} took {result.Elapsed:N2}s");

            if (result.Context != null)
            {
                WriteBlock("Context", result.Context);
            }

            if (string.IsNullOrWhiteSpace(result.AdditionalOutput) == false)
            {
                WriteBlock("Context", result.AdditionalOutput);
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        // Nothing to do here.
    }
}