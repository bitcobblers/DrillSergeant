using System;
using System.Threading.Tasks;

namespace JustBehave;

public class LambdaStep<TContext, TInput> : VerbStep<TContext, TInput> //, IStep
{
    private string? name;
    private Delegate handler = () => { };

    protected LambdaStep(string verb)
        : base(verb)
    {
    }

    ~LambdaStep()
    {
        this.Dispose(disposing: false);
    }

    public override string Name => this.name ?? this.handler?.Method?.GetType().FullName ?? nameof(LambdaGivenStep<TContext, TInput>);

    public LambdaStep<TContext, TInput> Named(string name)
    {
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

    public LambdaStep<TContext, TInput> Handle(Func<TContext>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle(Func<TContext, TContext>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle(Func<TContext, TInput, TContext>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1>(Func<TContext, TInput, TArg1, TContext>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2>(Func<TContext, TInput, TArg1, TArg2, TContext>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2, TArg3>(Func<TContext, TInput, TArg1, TArg2, TArg3, TContext>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TContext>? handler) => this.SetHandler(handler);

    public LambdaStep<TContext, TInput> Handle(Func<Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle(Func<TContext, Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle(Func<TContext, TInput, Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1>(Func<TContext, TInput, TArg1, Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2>(Func<TContext, TInput, TArg1, TArg2, Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2, TArg3>(Func<TContext, TInput, TArg1, TArg2, TArg3, Task>? handler) => this.SetHandler(handler);
    public LambdaStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, Task>? handler) => this.SetHandler(handler);

    public override object? Execute(object context, object input, IDependencyResolver resolver)
    {
        var parameters = ResolveParameters(resolver, context, input, this.handler.Method.GetParameters());

        dynamic r = this.handler.DynamicInvoke(parameters)!;

        return IsAsync(this.handler.Method) ? r.Result : r;
    }
}
