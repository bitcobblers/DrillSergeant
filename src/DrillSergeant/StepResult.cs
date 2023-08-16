using JetBrains.Annotations;
using System;

namespace DrillSergeant;

/// <summary>
/// Defines the evaluation of a step.
/// </summary>
/// <typeparam name="T">The type returned by the step.</typeparam>
public class StepResult<T> : AsyncStepResult<T>
{
    public StepResult(string name)
        : base(name)
    {
    }

    internal StepResult(string name, Func<T> func)
        : base(name, func)
    {
    }

    [PublicAPI]
    public new T Resolve()
    {
        try
        {
            return base.Resolve().Result;
        }
        catch (AggregateException ex) when (ex.InnerException != null)
        {
            throw ex.InnerException;
        }
    }

    /// <summary>
    /// Converts the step result to its resolved type.
    /// </summary>
    /// <param name="stepResult">The step result value to convert.</param>
    public static implicit operator T(StepResult<T> stepResult) => stepResult.Resolve();
}
