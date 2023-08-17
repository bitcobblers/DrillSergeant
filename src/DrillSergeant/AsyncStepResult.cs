using JetBrains.Annotations;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DrillSergeant;

public class AsyncStepResult<T>
{
    private AsyncLazy<T>? _value;

    public AsyncStepResult(string? name) => Name = name ?? "<unnamed>";

    internal AsyncStepResult(string? name, Func<Task<T>> func) : this(name) =>
        SetResult(func);

    internal AsyncStepResult(string? name, Func<T> func) : this(name) =>
        SetResult(func);

    internal void SetResult(Func<T> func) => _value = new AsyncLazy<T>(func);

    internal void SetResult(Func<Task<T>> func) => _value = new AsyncLazy<T>(func);

    /// <summary>
    /// Gets the name of the step result.
    /// </summary>
    [PublicAPI]
    public string Name { get; }

    /// <summary>
    /// Gets the resolved value of the result.
    /// </summary>
    [PublicAPI]
    public async Task<T> Resolve()
    {
        if (BehaviorExecutor.IsExecuting.Value == false)
        {
            throw new EagerStepResultEvaluationException(Name);
        }

        if (_value == null)
        {
            throw new StepResultNotSetException(Name);
        }

        return await _value;
    }

    public TaskAwaiter<T> GetAwaiter()
    {
        return _value != null ?
            _value!.GetAwaiter() :
#pragma warning disable CS8604
            Task.FromResult<T>(default).GetAwaiter();
#pragma warning restore CS8604
    }

    /// <summary>
    /// Converts an async step result to an awaitable task.
    /// </summary>
    /// <param name="stepResult">The step result to convert.</param>
    public static implicit operator Task<T>(AsyncStepResult<T> stepResult)
    {
        return stepResult._value != null ? 
            stepResult._value.Value : 
#pragma warning disable CS8604
            Task.FromResult<T>(default);
#pragma warning restore CS8604
    }
}
