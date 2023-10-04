using JetBrains.Annotations;

namespace DrillSergeant;

/// <summary>
/// Defines a step result that can be resolved synchronously.
/// </summary>
/// <typeparam name="T">The result type to resolve.</typeparam>
public class StepResult<T> : BaseStepResult
{
    private Lazy<T>? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="StepResult{T}"/> class.
    /// </summary>
    /// <param name="name">The name of the result.</param>
    public StepResult(string? name)
        : base(name)
    {
    }

    internal StepResult(string? name, Func<T>? func, Func<bool>? isExecuting = null)
        : base(name, isExecuting)
    {
        if (func != null)
        {
            SetResult(func);
        }
    }

    /// <summary>
    /// Resolves the step result.
    /// </summary>
    /// <returns>The resolved value of the result.</returns>
    /// <exception cref="EagerStepResultEvaluationException">Thrown when attempting to resolve the value outside of the behavior execution scope.</exception>
    /// <exception cref="StepResultNotSetException">Thrown when the result has not been set.</exception>
    [PublicAPI]
    public T Resolve()
    {
        if (IsExecuting == false)
        {
            throw new EagerStepResultEvaluationException(Name);
        }

        if (_value == null)
        {
            throw new StepResultNotSetException(Name);
        }

        return _value.Value;
    }

    internal void SetResult(Func<T> func) => _value = new Lazy<T>(func);

    /// <summary>
    /// Converts the step result to its resolved type.
    /// </summary>
    /// <param name="stepResult">The step result value to convert.</param>
    public static implicit operator T(StepResult<T> stepResult) => stepResult.Resolve();
}
