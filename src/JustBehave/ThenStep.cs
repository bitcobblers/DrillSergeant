using System.Threading.Tasks;

namespace JustBehave;

public class ThenStep<TContext, TInput> : Step
{
    public ThenStep()
        : base("Then")
    {
    }

    public virtual void Then(TContext context, TInput input)
    {
        ThenAsync(context, input).Wait();
    }

    public virtual Task ThenAsync(TContext context, TInput input) => Task.CompletedTask;
}
