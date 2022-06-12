namespace JustBehave
{
    public abstract class ActStep<TContext, TInput, TResult> : Step
    {
        public abstract TResult Act(TContext context, TInput input);

        public static LambdaActStep<TContext, TInput, TResult> Lamda() => new();
    }
}