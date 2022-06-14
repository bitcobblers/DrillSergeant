using System.Threading.Tasks;

namespace JustBehave
{
    public abstract class WhenStep<TContext, TInput, TResult> : Step
    {
        public virtual TResult When(TContext context, TInput input) => this.WhenAsync(context, input).Result;

        public virtual Task<TResult> WhenAsync(TContext context, TInput input) => Task.FromResult<TResult>(default!);
    }
}