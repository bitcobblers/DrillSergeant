﻿using JetBrains.Annotations;

namespace DrillSergeant;

/// <summary>
/// Defines a step that is defined as a method handler.
/// </summary>
public class LambdaStep : LambdaStepBuilder<LambdaStep>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LambdaStep"/> class.
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global
    public LambdaStep()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LambdaStep"/> class.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    public LambdaStep(string? name)
        : base(name)
    {
    }

    /// <summary>
    /// Sets an asynchronous handler.
    /// </summary>
    /// <param name="handler">The handler to set.</param>
    /// <returns>The current step.</returns>
    [PublicAPI]
    public LambdaStep Handle(Action handler)
    {
        Handler = handler;

        return this;
    }
}

/// <summary>
/// Defines a lambda step that returns a result when executed.
/// </summary>
/// <typeparam name="T">The return type for the step.</typeparam>
public class LambdaStep<T> : LambdaStepBuilder<LambdaStep<T>>
{
    private StepResult<T>? _result;

    /// <summary>
    /// Initializes a new instance of the <see cref="LambdaStep{T}"/> class.
    /// </summary>
    public LambdaStep()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LambdaStep{T}"/> class.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    public LambdaStep(string? name)
        : base(name)
    {
    }

    /// <summary>
    /// Caches the step result that will be set when the step is executed.
    /// </summary>
    /// <param name="result">The result to cache.</param>
    /// <returns>The current step.</returns>
    [PublicAPI]
    public LambdaStep<T> SetResult(StepResult<T> result)
    {
        _result = result;
        return this;
    }

    /// <summary>
    /// Sets the handler.
    /// </summary>
    /// <param name="handler">The handler to set.</param>
    /// <returns>The current step.</returns>
    [PublicAPI]
    public LambdaStep<T> Handle(Func<T> handler)
    {
        Handler = () =>
        {
            var value = handler();
            _result?.SetResult(() => value);
        };

        return this;
    }
}
