namespace JustBehave
{
    public abstract class ArrangeStep<TContext, TInput> : Step
    {
        public abstract TContext Arrange(TContext context, TInput input);

        public static LambdaArrangeStep<TContext, TInput> Lambda() => new();
    }
}