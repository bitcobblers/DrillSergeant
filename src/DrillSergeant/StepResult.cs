using JetBrains.Annotations;
using System;

namespace DrillSergeant;

/// <summary>
/// Defines the evaluation of a step.
/// </summary>
/// <typeparam name="T">The type returned by the step.</typeparam>
public class StepResult<T>
{
    private readonly Lazy<T> _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="StepResult{T}"/> class.
    /// </summary>
    /// <param name="func">The function to call to evaluate the result.</param>
    public StepResult(Func<T> func) => _value = new Lazy<T>(func);

    /// <summary>
    /// Gets the resolved value of the result.
    /// </summary>
    [PublicAPI]
    public T Value
    {
        get
        {
            if (BehaviorExecutor.State.Value == ExecutionState.NotExecuting)
            {
                throw new EagerStepResultEvaluationException();
            }

            return _value.Value;
        }
    }

    /// <summary>
    /// Converts the step result to its resolved type.
    /// </summary>
    /// <param name="stepResult">The step result value to convert.</param>
    public static implicit operator T(StepResult<T> stepResult) => stepResult.Value;
}
