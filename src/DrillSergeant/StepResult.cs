using JetBrains.Annotations;
using System;

namespace DrillSergeant;

/// <summary>
/// Defines the evaluation of a step.
/// </summary>
/// <typeparam name="T">The type returned by the step.</typeparam>
public class StepResult<T>
{
    private Lazy<T>? _value;

    public StepResult(string? name) => Name = name ?? "<unnamed>";

    internal StepResult(string? name, Func<T> func) : this(name) =>
        SetResult(func);

    /// <summary>
    /// Gets the name of the step result.
    /// </summary>
    [PublicAPI]
    public string Name { get; }

    internal void SetResult(Func<T> func) => _value = new Lazy<T>(func);

    /// <summary>
    /// Gets the resolved value of the result.
    /// </summary>
    [PublicAPI]
    public T Resolve()
    {
        if(BehaviorExecutor.IsExecuting.Value==false)
        {
            throw new EagerStepResultEvaluationException(Name);
        }

        if(_value==null)
        {
            throw new StepResultNotSetException(Name);
        }

        return _value.Value;
    }

    /// <summary>
    /// Converts the step result to its resolved type.
    /// </summary>
    /// <param name="stepResult">The step result value to convert.</param>
    public static implicit operator T(StepResult<T> stepResult) => stepResult.Resolve();
}
