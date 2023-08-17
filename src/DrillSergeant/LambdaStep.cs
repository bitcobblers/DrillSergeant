using JetBrains.Annotations;
using System;
using System.Threading.Tasks;

namespace DrillSergeant;

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

    [PublicAPI]
    public LambdaStep<T> SetResult(StepResult<T> result)
    {
        _result = result;
        return this;
    }

    [PublicAPI]
    public LambdaStep<T> SetResultAsync(AsyncStepResult<T> asyncResult)
    {
        _asyncResult = asyncResult;
        return this;
    }

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

    [PublicAPI]
    public LambdaStep<T> HandleAsync(Func<Task<T>> handler)
    {
        SetHandler(async () =>
        {
            var value = await handler();
            _asyncResult?.SetResult(() => value);
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

    [PublicAPI]
    public LambdaStep Handle(Action? handler) => SetHandler(handler);

    [PublicAPI]
    public LambdaStep HandleAsync(Func<Task>? handler) => SetHandler(handler);

    /// <inheritdoc />
    protected override Delegate PickHandler()
    {
        return _handler ?? new Action(() => { });
    }

    protected virtual LambdaStep SetHandler(Delegate? handler)
    {
        if (handler != null)
        {
            _handler = handler;
        }

        return this;
    }
}
