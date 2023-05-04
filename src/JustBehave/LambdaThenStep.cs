using System;
using System.Threading.Tasks;

namespace JustBehave;

public class LambdaThenStep<TContext, TInput> : ThenStep<TContext, TInput>
{
    public delegate void ThenMethod(TContext context, TInput input);
    public delegate Task ThenAsyncMethod(TContext context, TInput input);

    private string? name;
    private ThenAsyncMethod? thenHandler;

    public override string Name => this.name ?? this.thenHandler?.GetType().FullName ?? nameof(LambdaThenStep<TContext, TInput>);

    public LambdaThenStep<TContext, TInput> Named(string name)
    {
        this.name = name?.Trim();
        return this;
    }

    public LambdaThenStep<TContext, TInput> Handle(ThenMethod? execute)
    {
        this.thenHandler = new ThenAsyncMethod((c, i) =>
        {
            execute?.Invoke(c, i);
            return Task.CompletedTask;
        });

        return this;
    }

    public LambdaThenStep<TContext, TInput> HandleAsync(ThenAsyncMethod? execute)
    {
        this.thenHandler = execute ?? new ThenAsyncMethod((_, _) => Task.CompletedTask);
        return this;
    }

    public override async Task ThenAsync(TContext context, TInput input)
    {
        if (this.thenHandler != null)
        {
            await this.thenHandler(context, input);
        }
    }
}