using System;
using System.Threading.Tasks;

namespace DrillSergeant;

public class LambdaStep<TContext, TInput> : BaseStep<TContext, TInput>
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

    public LambdaStep<TContext, TInput> Handle(Func<TContext, TInput, TContext>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1>(Func<TContext, TInput, TArg1, TContext>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2>(Func<TContext, TInput, TArg1, TArg2, TContext>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2, TArg3>(Func<TContext, TInput, TArg1, TArg2, TArg3, TContext>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TContext>? handler) => this.SetHandler(handler);

    public LambdaStep<TContext, TInput> Handle(Func<TContext, TInput, Task<TContext>>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1>(Func<TContext, TInput, TArg1, Task<TContext>>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2>(Func<TContext, TInput, TArg1, TArg2, Task<TContext>>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2, TArg3>(Func<TContext, TInput, TArg1, TArg2, TArg3, Task<TContext>>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, Task<TContext>>? handler) => this.SetHandler(handler);

    public LambdaStep<TContext, TInput> Handle(Action<TContext, TInput>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1>(Action<TContext, TInput, TArg1>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2>(Action<TContext, TInput, TArg1, TArg2>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2, TArg3>(Action<TContext, TInput, TArg1, TArg2, TArg3>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4>(Action<TContext, TInput, TArg1, TArg2, TArg3, TArg4>? handler) => this.SetHandler(handler);

    public LambdaStep<TContext, TInput> Handle(Func<TContext, TInput, Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1>(Func<TContext, TInput, TArg1, Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2>(Func<TContext, TInput, TArg1, TArg2, Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2, TArg3>(Func<TContext, TInput, TArg1, TArg2, TArg3, Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, Task>? handler) => this.SetHandler(handler);

    public override async Task<object?> Execute(object context, object input, IDependencyResolver resolver)
    {
        var parameters = ResolveParameters(resolver, context, input, this.handler.Method.GetParameters());
        var isAsync = IsAsync(this.handler.Method);
        dynamic r = this.handler.DynamicInvoke(parameters)!;

        if (isAsync)
        {
            if (this.handler.Method.ReturnType.IsGenericType)
            {
                return await r;
            }
            else
            {
                await r;
                return null;
            }
        }
        else
        {
            return r;
        }
    }
}
