using JetBrains.Annotations;
using System;
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
        if (BehaviorExecutor.State.Value == ExecutionState.NotExecuting)
        {
            throw new EagerStepResultEvaluationException(Name);
        }

        if (_value == null)
        {
            throw new StepResultNotSetException(Name);
        }

        return await _value;
    }
}