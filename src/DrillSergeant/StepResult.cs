using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant
{
    [ExcludeFromCodeCoverage]
    public record StepResult
    {
        public string Verb { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public bool Skipped { get; init; }
        public bool Success { get; init; }
        public bool PreviousStepsFailed { get; init; }
        public decimal Elapsed { get; init; }
        public string AdditionalOutput { get; init; } = string.Empty;
        public object? Context { get; init; }
    }
}