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
    public LambdaStep(string verb) => this.Verb = verb;

    /// <inheritdoc />
    public override string Name => this.name ?? this.handler?.Method?.GetType().FullName ?? nameof(LambdaStep);

    /// <summary>
    /// Sets the name of the step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <returns>The current step.</returns>
    public LambdaStep Named(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return this;
        }

        this.name = name?.Trim();
        return this;
    }

    /// <summary>
    /// Sets the handler for the step.
    /// </summary>
    /// <param name="handler">The handler delegate to set.</param>
    /// <returns>The current step.</returns>
    public LambdaStep Handle(Delegate handler) => this.SetHandler(handler);

    /// <inheritdoc cref="Handle(Delegate)" />
    public LambdaStep Handle(Action? handler) => this.SetHandler(handler);

    /// <inheritdoc cref="Handle(Delegate)" />
    public LambdaStep Handle(Action<dynamic>? handler) => this.SetHandler(handler);

    /// <inheritdoc cref="Handle(Delegate)" />
    public LambdaStep Handle(Action<dynamic, dynamic>? handler) => this.SetHandler(handler);

    /// <inheritdoc cref="Handle(Delegate)" />
    public LambdaStep Handle(Func<Task>? handler) => this.SetHandler(handler);

    /// <inheritdoc cref="Handle(Delegate)" />
    public LambdaStep Handle(Func<dynamic, Task>? handler) => this.SetHandler(handler);

    /// <inheritdoc cref="Handle(Delegate)" />
    public LambdaStep Handle(Func<dynamic, dynamic, Task>? handler) => this.SetHandler(handler);

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
