using System.Threading.Tasks;

namespace JustBehave;

public class GivenStep<TContext, TInput> : Step
{
    public GivenStep()
        : base("Given")
    {
    }

    public virtual TContext Given(TContext context, TInput input) => Task.Run(() => this.GivenAsync(context, input)).Result;

    public virtual Task<TContext> GivenAsync(TContext context, TInput input) => Task.FromResult<TContext>(default!);
}