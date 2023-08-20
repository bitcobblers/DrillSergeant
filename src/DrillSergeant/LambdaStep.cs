using JetBrains.Annotations;
using System;
using System.Threading.Tasks;

namespace DrillSergeant;

/// <summary>
/// Defines a lambda step that sets returns a result when executed.
/// </summary>
/// <typeparam name="T">The return type for the step.</typeparam>
public class LambdaStep<T> : LambdaStep
{
    private StepResult<T>? _result;
    private AsyncStepResult<T>? _asyncResult;

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

    /// <inheritdoc cref="LambdaStep.SetName(string?)" />
    [PublicAPI]
    public new LambdaStep<T> SetName(string? name)
    {
        base.SetName(name);
        return this;
    }

    /// <inheritdoc cref="LambdaStep.SetVerb(string?)" />
    [PublicAPI]
    public new LambdaStep<T> SetVerb(string? verb)
    {
        base.SetVerb(verb);
        return this;
    }

    /// <inheritdoc cref="LambdaStep.Skip(Func{bool}?)" />
    [PublicAPI]
    public new LambdaStep<T> Skip(Func<bool>? shouldSkip)
    {
        base.Skip(shouldSkip);
        return this;
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
    /// Caches the step result that will be set when the step is executed.
    /// </summary>
    /// <param name="asyncResult">The result to cache.</param>
    /// <returns>The current step.</returns>
    [PublicAPI]
    public LambdaStep<T> SetResultAsync(AsyncStepResult<T> asyncResult)
    {
        _asyncResult = asyncResult;
        return this;
    }

    /// <inheritdoc cref="LambdaStep.Handle(Action)" />
    [PublicAPI]
    public LambdaStep<T> Handle(Func<T> handler)
    {
        SetHandler(() =>
        {
            var value = handler();
            _result?.SetResult(() => value);
        });

        return this;
    }

    /// <inheritdoc cref="LambdaStep.HandleAsync(Func{Task})" />
    [PublicAPI]
    public LambdaStep<T> HandleAsync(Func<Task<T>> handler)
    {
        SetHandler(async () =>
        {
            var value = await handler();
            _asyncResult?.SetResult(() => Task.FromResult(value));
        });

        return this;
    }
}

/// <summary>
/// Defines a step that is defined as a method handler.
/// </summary>
public class LambdaStep : BaseStep
{
    private string? _name;
    private Delegate? _handler;
    private Func<bool> _shouldSkip = () => false;

    /// <summary>
    /// Initializes a new instance of the <see cref="LambdaStep"/> class.
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global
    public LambdaStep()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LambdaStep"/> class.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    public LambdaStep(string? name)
    {
        SetName(name);
    }

    /// <inheritdoc />
    public override string Name => _name ?? _handler?.Method.Name ?? GetType().Name;

    /// <inheritdoc />
    public override bool ShouldSkip => _shouldSkip();

    /// <summary>
    /// Sets the name of the step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <returns>The current step.</returns>
    [PublicAPI]
    public LambdaStep SetName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return this;
        }

        _name = name.Trim();
        return this;
    }

    /// <summary>
    /// Sets the verb for the lambda step.
    /// </summary>
    /// <param name="verb">The verb of the step.</param>
    /// <returns>The current step.</returns>
    [PublicAPI]
    public LambdaStep SetVerb(string? verb)
    {
        if (string.IsNullOrWhiteSpace(verb))
        {
            return this;
        }

        Verb = verb.Trim();
        return this;
    }

    /// <summary>
    /// Sets a flag indicating whether the step should be skipped.
    /// </summary>
    /// <param name="shouldSkip">An optional delegate to determine if the step should be skipped.</param>
    /// <returns>The current step.</returns>
    [PublicAPI]
    public LambdaStep Skip(Func<bool>? shouldSkip = null)
    {
        _shouldSkip = shouldSkip ?? new Func<bool>(() => true);
        return this;
    }

    /// <summary>
    /// Sets synchronous handler.
    /// </summary>
    /// <param name="handler">The handler to set.</param>
    /// <returns>The current step.</returns>
    [PublicAPI]
    public LambdaStep Handle(Action? handler) => SetHandler(handler);

    /// <summary>
    /// Sets an asynchronous handler.
    /// </summary>
    /// <param name="handler">The handler to set.</param>
    /// <returns>The current step.</returns>
    [PublicAPI]
    public LambdaStep HandleAsync(Func<Task>? handler) => SetHandler(handler);

    /// <inheritdoc />
    protected override Delegate PickHandler()
    {
        return _handler ?? new Action(() => { });
    }

    // ReSharper disable once VirtualMemberNeverOverridden.Global
    protected virtual LambdaStep SetHandler(Delegate? handler)
    {
        if (handler != null)
        {
            _handler = handler;
        }

        return this;
    }
}
