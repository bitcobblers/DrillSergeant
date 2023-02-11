using System;
using System.Threading.Tasks;

namespace JustBehave;

public class LambdaWhenStep<TContext, TInput, TResult> : WhenStep<TContext, TInput, TResult>
{
    public delegate TResult WhenMethod(TContext context, TInput input);
    public delegate Task<TResult> WhenAsyncMethod(TContext context, TInput input);

    private string? name;
    private WhenAsyncMethod? whenHandler;
    private Action? teardownHandler;

    public override string Name => this.name ?? this.whenHandler?.GetType().FullName ?? nameof(LambdaWhenStep<TContext, TInput, TResult>);

    public LambdaWhenStep<TContext, TInput, TResult> Named(string name)
    {
        this.name = name?.Trim();
        return this;
    }

    public LambdaWhenStep<TContext, TInput, TResult> Handle(WhenMethod? execute)
    {
        this.whenHandler = new WhenAsyncMethod(async (c, i) =>
        {
            if (execute == null)
            {
                return await Task.FromResult<TResult>(default!);
            }

            return execute(c, i);
        });

        return this;
    }

    public LambdaWhenStep<TContext, TInput, TResult> Handle(WhenAsyncMethod? execute)
    {
        this.whenHandler = execute ?? new WhenAsyncMethod((_, _) => Task.FromResult<TResult>(default!));
        return this;
    }

    public LambdaWhenStep<TContext, TInput, TResult> Teardown(Action teardown)
    {
        this.teardownHandler = teardown ?? new Action(() => { });
        return this;
    }

    public override Task<TResult> WhenAsync(TContext context, TInput input)
    {
        if (this.whenHandler != null)
        {
            return this.whenHandler(context, input);
        }

        return default!;
    }

    protected override void Teardown() => this.teardownHandler?.Invoke();
}