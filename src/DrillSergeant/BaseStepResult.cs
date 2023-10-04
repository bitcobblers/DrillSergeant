using JetBrains.Annotations;

namespace DrillSergeant;

/// <summary>
/// Describe a step result with a name and scope.
/// </summary>
public abstract class BaseStepResult
{
    private readonly Func<bool> _isExecuting;

    internal BaseStepResult(string? name, Func<bool>? isExecuting = null)
    {
        Name = name?.Trim() ?? "<unnamed>";
        _isExecuting = isExecuting ?? IsBehaviorExecuting;
    }

    /// <summary>
    /// Gets the name of the step result.
    /// </summary>
    [PublicAPI]
    public string Name { get; }

    /// <summary>
    /// Returns true if the the result is currently within an execution scope.
    /// </summary>
    protected bool IsExecuting => _isExecuting();

    private static bool IsBehaviorExecuting() =>
        BehaviorExecutor.IsExecuting.Value;
}