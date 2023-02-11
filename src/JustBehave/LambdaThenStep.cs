using System;
using System.Threading.Tasks;

namespace JustBehave;

public class LambdaThenStep<TContext, TInput, TResult> : ThenStep<TContext, TInput, TResult>
{
    public delegate void ThenMethod(TContext context, TInput input, TResult result);
    public delegate Task ThenAsyncMethod(TContext context, TInput input, TResult result);

    private string? name;
    private ThenAsyncMethod? thenHandler;
    private Action? teardownHandler;

    public override string Name => this.name ?? this.thenHandler?.GetType().FullName ?? nameof(LambdaThenStep<TContext, TInput, TResult>);

    public LambdaThenStep<TContext, TInput, TResult> Named(string name)
    {
        this.name = name?.Trim();
        return this;
    }

    public LambdaThenStep<TContext, TInput, TResult> Handle(ThenMethod? execute)
    {
        this.thenHandler = new ThenAsyncMethod(async (c, i, r) =>
        {
            execute?.Invoke(c, i, r);
            await Task.CompletedTask;
        });

        return this;
    }

    public LambdaThenStep<TContext, TInput, TResult> HandleAsync(ThenAsyncMethod? execute)
    {
        this.thenHandler = execute ?? new ThenAsyncMethod((_, _, _) => Task.CompletedTask);
        return this;
    }

    public LambdaThenStep<TContext, TInput, TResult> Teardown(Action teardown)
    {
        this.teardownHandler = teardown ?? new Action(() => { });
        return this;
    }

    public override async Task ThenAsync(TContext context, TInput input, TResult result)
    {
        if (this.thenHandler != null)
        {
            await this.thenHandler(context, input, result);
        }
    }

    protected override void Teardown() => this.teardownHandler?.Invoke();
}