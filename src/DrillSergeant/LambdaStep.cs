using System;
using System.Threading.Tasks;

// ReSharper disable UnusedMember.Global

namespace DrillSergeant;

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
    public LambdaStep SetName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return this;
        }

        _name = name.Trim();
        return this;
    }

    // ReSharper disable once UnusedMethodReturnValue.Global
    public LambdaStep SetVerb(string? verb)
    {
        if (string.IsNullOrWhiteSpace(verb))
        {
            return this;
        }

        Verb = verb.Trim();
        return this;
    }

    public LambdaStep Skip(Func<bool>? shouldSkip = null)
    {
        if (shouldSkip != null)
        {
            _shouldSkip = shouldSkip;
        }

        return this;
    }

    public LambdaStep Handle(Delegate handler) => SetHandler(handler);

    // ---

    public LambdaStep Handle(Action? handler) => SetHandler(handler);
    public LambdaStep Handle(Action<dynamic>? handler) => SetHandler(handler);
    public LambdaStep Handle(Action<dynamic, dynamic>? handler) => SetHandler(handler);
    public LambdaStep Handle<TContext>(Action<TContext>? handler) => SetHandler(handler);
    public LambdaStep Handle<TInput>(Action<dynamic, TInput>? handler) => SetHandler(handler);
    public LambdaStep Handle<TContext, TInput>(Action<TContext, TInput>? handler) => SetHandler(handler);

    // ---

    public LambdaStep HandleAsync(Func<Task>? handler) => SetHandler(handler);
    public LambdaStep HandleAsync(Func<dynamic, Task>? handler) => SetHandler(handler);
    public LambdaStep HandleAsync(Func<dynamic, dynamic, Task>? handler) => SetHandler(handler);
    public LambdaStep HandleAsync<TContext>(Func<TContext, Task>? handler) => SetHandler(handler);
    public LambdaStep HandleAsync<TInput>(Func<dynamic, TInput, Task>? handler) => SetHandler(handler);
    public LambdaStep HandleAsync<TContext, TInput>(Func<TContext, TInput, Task>? handler) => SetHandler(handler);

    /// <inheritdoc />
    protected override Delegate PickHandler()
    {
        return _handler ?? new Action(() => { });
    }

    private LambdaStep SetHandler(Delegate? handler)
    {
        if (handler != null)
        {
            _handler = handler;
        }

        return this;
    }
}
