﻿using Newtonsoft.Json;

namespace DrillSergeant;

/// <summary>
/// Defines a test reporter that writes unstructured text to the writer provided.
/// </summary>
public class RawTestReporter : ITestReporter
{
    private readonly TextWriter _writer;

    /// <summary>
    /// Initializes a new instance of the <see cref="RawTestReporter"/> class.
    /// </summary>
    /// <param name="writer">The writer to write test output to.</param>
    public RawTestReporter(TextWriter writer) => _writer = writer;

    ~RawTestReporter() => Dispose(disposing: false);

    /// <summary>
    /// Disposes resources used by the instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public virtual void WriteBlock(string? label, object? content)
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
    public virtual void WriteStepResult(StepExecutionResult? result)
    {
        if (result == null)
        {
            return;
        }

        var icon = result.Success ? "✅" : "❎";

        switch (result)
        {
            case { CancelPending: true }:
                _writer.WriteLine($"⏩ {result.Verb} (skipped due to test abort): {result.Name}");
                break;
            case { Skipped: true, PreviousStepsFailed: true }:
                _writer.WriteLine($"⏩ {result.Verb} (skipped due to previous failure): {result.Name}");
                break;
            case { Skipped: true, PreviousStepsFailed: false }:
                _writer.WriteLine($"⏩ {result.Verb} (skipped): {result.Name}");
                break;
            default:
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

                    break;
                }
        }
    }

    // ReSharper disable once UnusedParameter.Global
    protected virtual void Dispose(bool disposing)
    {
        // Nothing to do here.
    }
}
