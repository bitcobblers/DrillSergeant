using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JustBehave;

public class LambdaStep<TContext, TInput> : IStep
{
    public record VerbMethod(MethodInfo Method, object Target, bool IsAsync);

    private string? name;
    private Delegate handler = () => { };

    protected LambdaStep(string verb)
    {
        this.Verb = verb;
    }

    ~LambdaStep()
    {
        this.Dispose(disposing: false);
    }

    public virtual string Name => this.name ?? this.handler?.Method?.GetType().FullName ?? nameof(LambdaGivenStep<TContext, TInput>);

    public string Verb { get; }

    public LambdaStep<TContext, TInput> Named(string name)
    {
        this.name = name?.Trim();
        return this;
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
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

    public virtual object Execute(IDependencyResolver resolver)
    {
        var parameters = ResolveParameters(resolver, this.handler.Method.GetParameters());

        dynamic r = this.handler.DynamicInvoke(parameters)!;

        return this.handler.Method.IsAsync() ? r.Result : r;
    }

    protected object?[] ResolveParameters(IDependencyResolver resolver, ParameterInfo[] parameters)
    {
        return (from p in parameters
                select resolver.Resolve(p.ParameterType)).ToArray();
    }

    protected virtual void Dispose(bool disposing)
    {
    }
}
