using System;
using System.Threading.Tasks;

namespace JustBehave;

public class LambdaGivenStep<TContext, TInput> : GivenStep<TContext, TInput>
{
    private string? name;
    private Delegate? handler;
    private Action? teardownHandler;

    public override string Name => this.name ?? this.handler?.Method?.GetType().FullName ?? nameof(LambdaGivenStep<TContext, TInput>);

    public LambdaGivenStep<TContext, TInput> Named(string name)
    {
        this.name = name?.Trim();
        return this;
    }

    private LambdaGivenStep<TContext,TInput> SetHandler(Delegate? handler)
    {
        if(handler != null)
        {
            this.handler = handler;
        }
        
        return this;
    }

    public LambdaGivenStep<TContext, TInput> Handle(Action? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle(Action<TContext>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle(Action<TContext, TInput>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1>(Action<TContext, TInput, TArg1>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2>(Action<TContext, TInput, TArg1, TArg2>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3>(Action<TContext, TInput, TArg1, TArg2, TArg3>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4>(Action<TContext, TInput, TArg1, TArg2, TArg3, TArg4>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4, TArg5>(Action<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TArg5>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Action<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(Action<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(Action<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>? handler) => this.SetHandler(handler);


    public LambdaGivenStep<TContext, TInput> Handle(Func<TContext>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle(Func<TContext, TContext>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle(Func<TContext, TInput, TContext>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1>(Func<TContext, TInput, TArg1, TContext>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2>(Func<TContext, TInput, TArg1, TArg2, TContext>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3>(Func<TContext, TInput, TArg1, TArg2, TArg3, TContext>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TContext>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TContext>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TContext>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TContext>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TContext>? handler) => this.SetHandler(handler);

    public LambdaGivenStep<TContext, TInput> Handle(Func<Task>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle(Func<TContext, Task>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle(Func<TContext, TInput, Task>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1>(Func<TContext, TInput, TArg1, Task>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2>(Func<TContext, TInput, TArg1, TArg2, Task>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3>(Func<TContext, TInput, TArg1, TArg2, TArg3, Task>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, Task>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TArg5, Task>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, Task>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, Task>? handler) => this.SetHandler(handler);
    public LambdaGivenStep<TContext, TInput> Handle<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(Func<TContext, TInput, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, Task>? handler) => this.SetHandler(handler);

    public LambdaGivenStep<TContext, TInput> Teardown(Action teardown)
    {
        this.teardownHandler = teardown ?? new Action(() => { });
        return this;
    }

    protected override void Teardown()
    {
        this.teardownHandler?.Invoke();
    }

    internal override VerbMethod PickHandler()
    {
        var emptyMethod = new Action(() => { });
        var method = this.handler?.Method;

        if(method == null)
        {
            return new(emptyMethod.Method, emptyMethod, IsAsync: false);
        }

        return new(method, this.handler?.Target!, IsAsync(method.ReturnType));
    }
}