using JetBrains.Annotations;
using System.Runtime.CompilerServices;

namespace DrillSergeant;

/// <summary>
/// Defines a step result that can be resolved asynchronously.
/// </summary>
/// <typeparam name="T">The result type to resolve.</typeparam>
public class AsyncStepResult<T> : BaseStepResult
{
    private AsyncLazy<T>? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncStepResult{T}"/> class.
    /// </summary>
    /// <param name="name">The name of the result.</param>
    public AsyncStepResult(string? name)
        : base(name)
    {
    }

    internal AsyncStepResult(string? name, Func<Task<T>>? func = null, Func<bool>? isExecuting = null)
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
    public async Task<T> Resolve()
    {
        if (IsExecuting == false)
        {
            throw new EagerStepResultEvaluationException(Name);
        }

        if (_value == null)
        {
            throw new StepResultNotSetException(Name);
        }

        return await _value;
    }

    /// <summary>
    /// Gets an awaiter to resolve this <see cref="Task{T}"/>.
    /// </summary>
    /// <returns>An awaiter for this instance.</returns>
    public TaskAwaiter<T> GetAwaiter() =>
        Resolve().GetAwaiter();

    internal void SetResult(Func<Task<T>> func) => _value = new AsyncLazy<T>(func);

    /// <summary>
    /// Converts an async step result to an awaitable task.
    /// </summary>
    /// <param name="stepResult">The step result to convert.</param>
    public static implicit operator Task<T>(AsyncStepResult<T> stepResult)
        => stepResult.Resolve();
}
