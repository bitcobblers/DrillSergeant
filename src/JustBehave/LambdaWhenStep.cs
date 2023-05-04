using System;
using System.Threading.Tasks;

namespace JustBehave;

public class LambdaWhenStep<TContext, TInput> : WhenStep<TContext, TInput>
{
    public delegate TContext WhenMethod(TContext context, TInput input);
    public delegate Task<TContext> WhenAsyncMethod(TContext context, TInput input);

    private string? name;
    private WhenAsyncMethod? whenHandler;
    private Action? teardownHandler;

    public override string Name => this.name ?? this.whenHandler?.GetType().FullName ?? nameof(LambdaWhenStep<TContext, TInput>);

    public LambdaWhenStep<TContext, TInput> Named(string name)
    {
        this.name = name?.Trim();
        return this;
    }

    public LambdaWhenStep<TContext, TInput> Handle(Func<TContext, TInput, TContext>? execute)
    {
        this.whenHandler = new WhenAsyncMethod((c, i) =>
        {
            if (execute == null)
            {
                return Task.FromResult(c);
            }

            return Task.FromResult(execute.Invoke(c, i));
        });

        return this;
    }

    public LambdaWhenStep<TContext, TInput> Handle(WhenAsyncMethod? execute)
    {
        this.whenHandler = execute ?? new WhenAsyncMethod((c, _) => Task.FromResult(c));
        return this;
    }

    public LambdaWhenStep<TContext, TInput> Teardown(Action teardown)
    {
        this.teardownHandler = teardown ?? new Action(() => { });
        return this;
    }

    public override async Task WhenAsync(TContext context, TInput input)
    {
        if (this.whenHandler != null)
        {
            await this.whenHandler(context, input);
        }
    }

    protected override void Teardown() => this.teardownHandler?.Invoke();
}