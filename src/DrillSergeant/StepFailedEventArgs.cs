using System.Diagnostics.CodeAnalysis;

namespace DrillSergeant;

[ExcludeFromCodeCoverage]
public class StepFailedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StepFailedEventArgs"/> class.
    /// </summary>
    /// <param name="exception">The exception that caused the step to fail.</param>
    public StepFailedEventArgs(Exception exception)
    {
        Exception = exception;
    }

    public Exception Exception { get; }
}
