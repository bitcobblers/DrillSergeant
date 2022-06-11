namespace JustBehave
{
    public abstract class ActStep<TContext, TInput, TResult> : Step
    {
        public ActStep() : base(null) { }

        public ActStep(string description) : base(description) { }

        public abstract TResult Act(TContext context, TInput input);

        public static LambdaActStep<TContext, TInput, TResult> Lamda() => new LambdaActStep<TContext, TInput, TResult>();
    }
}