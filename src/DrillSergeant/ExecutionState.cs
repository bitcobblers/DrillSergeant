namespace DrillSergeant;

/// <summary>
/// Defines the current state of the executor.
/// </summary>
public enum ExecutionState
{
    /// <summary>
    /// Not currently executing a behavior.
    /// </summary>
    NotExecuting,

    /// <summary>
    /// Executing a behavior.
    /// </summary>
    Executing,
}