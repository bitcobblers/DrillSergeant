namespace JustBehave
{
    public abstract class AssertStep<TContext, TInput, TResult> : Step
    {
        public AssertStep() : base(null) { }

        public AssertStep(string description) : base(description) { }

        public abstract void Execute(TContext context, TInput input, TResult result);

        public static LambdaAssertStep<TContext, TInput, TResult> Lambda() => new LambdaAssertStep<TContext, TInput, TResult>();
    }
}