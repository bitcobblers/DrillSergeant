namespace DrillSergeant;

/// <summary>
/// Defines the contract for all behavior steps.
/// </summary>
public interface IStep : IDisposable
{
    /// <summary>
    /// Gets the verb associated with the step.
    /// </summary>
    string Verb { get; }

    /// <summary>
    /// Gets the name of the step.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a value indicating whether the step should be skipped.
    /// </summary>
    bool ShouldSkip { get; }

    /// <summary>
    /// Executes the step.
    /// </summary>
    /// <returns>An awaitable task for the execution.</returns>
    Task Execute();
}
