using System;
using System.Threading.Tasks;

namespace DrillSergeant;

public class LambdaStep<TContext, TInput> : BaseStep
{
    private string? name;
    private Delegate handler = () => { };

    public LambdaStep(string verb)
    {
        this.Verb = verb;
    }

    public override string Name => this.name ?? this.handler?.Method?.GetType().FullName ?? nameof(LambdaStep<TContext, TInput>);

    public LambdaStep<TContext, TInput> Named(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return this;
        }

        this.name = name?.Trim();
        return this;
    }

    private LambdaStep<TContext, TInput> SetHandler(Delegate? handler)
    {
        if (handler != null)
        {
            this.handler = handler;
        }

        return this;
    }

    public LambdaStep<TContext, TInput> Handle(Delegate handler) => this.SetHandler(handler);

    public LambdaStep<TContext, TInput> Handle(Action? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle(Action<TContext>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle(Action<TContext, TInput>? handler) => this.SetHandler(handler);

    public LambdaStep<TContext, TInput> Handle(Func<Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle(Func<TContext, Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle(Func<TContext, TInput, Task>? handler) => this.SetHandler(handler);

    public override async Task Execute(object context, object input)
    {
        var parameters = ResolveParameters(context, input, this.handler.Method.GetParameters());
        var isAsync = IsAsync(this.handler.Method);
        dynamic r = this.handler.DynamicInvoke(parameters)!;

        if (isAsync)
        {
            await r;
        }
    }
}
