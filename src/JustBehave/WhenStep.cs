using System.Threading.Tasks;

namespace JustBehave
{
    public class WhenStep<TContext, TInput, TResult> : Step
    {
        public WhenStep()
            : base("When")
        {
        }

        public virtual TResult When(TContext context, TInput input) => this.WhenAsync(context, input).Result;

        public virtual Task<TResult> WhenAsync(TContext context, TInput input) => Task.FromResult<TResult>(default!);
    }
}