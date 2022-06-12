namespace JustBehave
{
    public abstract class AssertStep<TContext, TInput, TResult> : Step
    {
        public abstract void Assert(TContext context, TInput input, TResult result);

        public static LambdaAssertStep<TContext, TInput, TResult> Lambda() => new();
    }
}