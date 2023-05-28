using System;
using System.Threading.Tasks;

namespace DrillSergeant;

public class LambdaStep : BaseStep
{
    private string? name;
    private Delegate handler = () => { };

    public LambdaStep(string verb)
    {
        this.Verb = verb;
    }

    public override string Name => this.name ?? this.handler?.Method?.GetType().FullName ?? nameof(LambdaStep);

    public LambdaStep Named(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return this;
        }

        this.name = name?.Trim();
        return this;
    }

    private LambdaStep SetHandler(Delegate? handler)
    {
        if (handler != null)
        {
            this.handler = handler;
        }

        return this;
    }

    public LambdaStep Handle(Delegate handler) => this.SetHandler(handler);

    public LambdaStep Handle(Action? handler) => this.SetHandler(handler);
    public LambdaStep Handle(Action<dynamic>? handler) => this.SetHandler(handler);
    public LambdaStep Handle(Action<dynamic, dynamic>? handler) => this.SetHandler(handler);

    public LambdaStep Handle(Func<Task>? handler) => this.SetHandler(handler);
    public LambdaStep Handle(Func<dynamic, Task>? handler) => this.SetHandler(handler);
    public LambdaStep Handle(Func<dynamic, dynamic, Task>? handler) => this.SetHandler(handler);

    protected override Delegate PickHandler() => this.handler;
}
