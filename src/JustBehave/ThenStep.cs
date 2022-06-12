namespace JustBehave
{
    public abstract class ThenStep<TContext, TInput, TResult> : Step
    {
        public abstract void Then(TContext context, TInput input, TResult result);
    }
}