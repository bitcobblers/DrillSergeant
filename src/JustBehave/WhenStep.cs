namespace JustBehave
{
    public abstract class WhenStep<TContext, TInput, TResult> : Step
    {
        public abstract TResult When(TContext context, TInput input);
    }
}