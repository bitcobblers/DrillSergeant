using System;
using System.Threading.Tasks;

namespace DrillSergeant;

/// <summary>
/// Defines a step that is defined as a method handler.
/// </summary>
public class LambdaStep : BaseStep
{
    private string? name;
    private Delegate handler = () => { };

    /// <summary>
    /// Initializes a new instance of the <see cref="LambdaStep"/> class.
    /// </summary>
    /// <param name="verb">The verb for the step.</param>
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
        this.Named(name);
    }

    /// <inheritdoc />
    public override string Name => this.name ?? this.handler?.Method?.GetType().FullName ?? nameof(LambdaStep);

    /// <summary>
    /// Sets the name of the step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <returns>The current step.</returns>
    public LambdaStep Named(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return this;
        }

        this.name = name.Trim();
        return this;
    }

    public LambdaStep SetVerb(string? verb)
    {
        if(string.IsNullOrWhiteSpace(verb))
        {
            return this;
        }

        this.Verb = verb.Trim();
        return this;
    }

    public LambdaStep Handle(Delegate handler) => this.SetHandler(handler);

    // ---

    public LambdaStep Handle(Action? handler) => this.SetHandler(handler);
    public LambdaStep Handle(Action<dynamic>? handler) => this.SetHandler(handler);
    public LambdaStep Handle(Action<dynamic, dynamic>? handler) => this.SetHandler(handler);
    public LambdaStep Handle<TContext>(Action<TContext>? handler) => this.SetHandler(handler);
    public LambdaStep Handle<TInput>(Action<dynamic, TInput>? handler) => this.SetHandler(handler);
    public LambdaStep Handle<TContext, TInput>(Action<TContext, TInput>? handler) => this.SetHandler(handler);

    // ---

    public LambdaStep HandleAsync(Func<Task>? handler) => this.SetHandler(handler);
    public LambdaStep HandleAsync(Func<dynamic, Task>? handler) => this.SetHandler(handler);
    public LambdaStep HandleAsync(Func<dynamic, dynamic, Task>? handler) => this.SetHandler(handler);
    public LambdaStep HandleAsync<TContext>(Func<TContext, Task>? handler) => this.SetHandler(handler);
    public LambdaStep HandleAsync<TInput>(Func<dynamic, TInput, Task>? handler) => this.SetHandler(handler);
    public LambdaStep HandleAsync<TContext, TInput>(Func<TContext, TInput, Task>? handler) => this.SetHandler(handler);

    /// <inheritdoc />
    protected override Delegate PickHandler() => this.handler;

    private LambdaStep SetHandler(Delegate? handler)
    {
        if (handler != null)
        {
            this.handler = handler;
        }

        return this;
    }
}
