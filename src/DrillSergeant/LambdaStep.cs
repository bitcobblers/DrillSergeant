using System;
using System.Threading.Tasks;

namespace DrillSergeant;

public class LambdaStep<TInput> : BaseStep
{
    private string? name;
    private Delegate handler = () => { };

    public LambdaStep(string verb)
    {
        this.Verb = verb;
    }

    public override string Name => this.name ?? this.handler?.Method?.GetType().FullName ?? nameof(LambdaStep<TInput>);

    public LambdaStep<TInput> Named(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return this;
        }

        this.name = name?.Trim();
        return this;
    }

    private LambdaStep<TInput> SetHandler(Delegate? handler)
    {
        if (handler != null)
        {
            this.handler = handler;
        }

        return this;
    }

    public LambdaStep<TInput> Handle(Delegate handler) => this.SetHandler(handler);

    public LambdaStep<TInput> Handle(Action? handler) => this.SetHandler(handler);
    public LambdaStep<TInput> Handle(Action<dynamic>? handler) => this.SetHandler(handler);
    public LambdaStep<TInput> Handle(Action<dynamic, TInput>? handler) => this.SetHandler(handler);

    public LambdaStep<TInput> Handle(Func<Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TInput> Handle(Func<dynamic, Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TInput> Handle(Func<dynamic, TInput, Task>? handler) => this.SetHandler(handler);

    protected override Delegate PickHandler() => this.handler;
}
