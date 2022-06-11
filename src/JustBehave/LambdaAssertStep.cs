namespace JustBehave
{
    public class LambdaAssertStep<TContext, TInput, TResult> : AssertStep<TContext, TInput, TResult>
    {
        public delegate void ExecuteMethod(TContext context, TInput input, TResult result);

        private ExecuteMethod execute = (_, __, ___) => { }!;

        public LambdaAssertStep()
            : base(null)
        {
        }

        public LambdaAssertStep<TContext, TInput, TResult> Handle(ExecuteMethod execute)
        {
            this.execute = execute;
            return this;
        }

        public override void Execute(TContext context, TInput input, TResult result) => this.execute(context, input, result);
    }
}