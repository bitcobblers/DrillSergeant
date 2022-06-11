namespace JustBehave
{
    public abstract class ArrangeStep<TContext, TInput> : Step
    {
        public ArrangeStep() : base(null) { }

        public ArrangeStep(string description) : base(description) { }

        public abstract TContext Execute(TContext context, TInput input);

        public static LambdaArrangeStep<TContext, TInput> Lambda() => new LambdaArrangeStep<TContext, TInput>();
    }
}