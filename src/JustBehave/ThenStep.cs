using System.Threading.Tasks;

namespace JustBehave
{
    public abstract class ThenStep<TContext, TInput, TResult> : Step
    {
        public virtual void Then(TContext context, TInput input, TResult result) => this.ThenAsync(context, input, result).Wait();

        public virtual Task ThenAsync(TContext context, TInput input, TResult result) => Task.CompletedTask;
    }
}