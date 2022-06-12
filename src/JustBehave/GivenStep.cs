namespace JustBehave
{
    public abstract class GivenStep<TContext, TInput> : Step
    {
        public abstract TContext Given(TContext context, TInput input);
    }
}