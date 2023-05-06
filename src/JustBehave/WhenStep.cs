using System.Threading.Tasks;

namespace JustBehave;

public class WhenStep<TContext, TInput> : Step<TContext, TInput>
{
    public WhenStep()
        : base("When")
    {
    }

    public virtual void When(TContext context, TInput input)
    {
        WhenAsync(context, input).Wait();
    }

    public virtual Task WhenAsync(TContext context, TInput input) => Task.CompletedTask;
}