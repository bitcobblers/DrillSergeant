using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace DrillSergeant.NUnit3.Reporting
{
    public class RawTestReporter : ITestReporter
    {
        private readonly TextWriter _writer;

        public RawTestReporter(TextWriter writer) => _writer = writer;

        [ExcludeFromCodeCoverage]
        public string Output => string.Empty;

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
        }

        public void WriteBlock(string label, object content)
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

        public void WriteStepResult(string verb, string name, bool skipped, decimal elapsed, bool success, object? context)
        {
            var icon = success ? "✅" : "❎";

            if (skipped)
            {
                _writer.WriteLine($"⏩ {verb} (skipped due to previous failure): {name}");
            }
            else
            {
                _writer.WriteLine($"{icon} {verb}: {name} took {elapsed:N2}s");

                if (context != null)
                {
                    WriteBlock("Context", context);
                }
            }
        }
    }
}