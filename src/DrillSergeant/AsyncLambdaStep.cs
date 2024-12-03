using JetBrains.Annotations;

namespace DrillSergeant;

/// <summary>
/// Defines a step that is defined as a method handler.
/// </summary>
public class AsyncLambdaStep : LambdaStepBuilder<AsyncLambdaStep>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLambdaStep"/> class.
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global
    public AsyncLambdaStep()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLambdaStep"/> class.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    public AsyncLambdaStep(string? name)
        : base(name)
    {
    }

    /// <summary>
    /// Sets the handler.
    /// </summary>
    /// <param name="handler">The handler to set.</param>
    /// <returns>The current step.</returns>
    [PublicAPI]
    public AsyncLambdaStep Handle(Func<Task> handler)
    {
        Handler = async () =>
        {
            await handler();
        };

        return this;
    }
}

/// <summary>
/// Defines a lambda step that returns a result when executed.
/// </summary>
/// <typeparam name="T">The return type for the step.</typeparam>
public class AsyncLambdaStep<T> : LambdaStepBuilder<AsyncLambdaStep<T>>
{
    private AsyncStepResult<T>? _asyncResult;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLambdaStep{T}"/> class.
    /// </summary>
    public AsyncLambdaStep()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLambdaStep{T}"/> class.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    public AsyncLambdaStep(string? name)
        : base(name)
    {
    }

    /// <summary>
    /// Caches the step result that will be set when the step is executed.
    /// </summary>
    /// <param name="asyncResult">The result to cache.</param>
    /// <returns>The current step.</returns>
    [PublicAPI]
    public AsyncLambdaStep<T> SetResult(AsyncStepResult<T> asyncResult)
    {
        _asyncResult = asyncResult;
        return this;
    }

    /// <summary>
    /// Sets an asynchronous handler.
    /// </summary>
    /// <param name="handler">The handler to set.</param>
    /// <returns>The current step.</returns>
    [PublicAPI]
    public AsyncLambdaStep<T> Handle(Func<Task<T>> handler)
    {
        Handler = async () =>
        {
            var value = await handler();
            _asyncResult?.SetResult(() => Task.FromResult(value));
        };

        return this;
    }
}