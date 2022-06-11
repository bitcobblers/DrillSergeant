namespace JustBehave
{
    public class LambdaActStep<TContext, TInput, TResult> : ActStep<TContext, TInput, TResult>
    {
        public delegate TResult ExecuteMethod(TContext context, TInput input);

        private ExecuteMethod execute = (_, __) => default!;

        public LambdaActStep()
            : base(null)
        {
        }

        public LambdaActStep<TContext, TInput, TResult> Handle(ExecuteMethod execute)
        {
            this.execute = execute;
            return this;
        }

        public override TResult Act(TContext context, TInput input) => this.execute(context, input);
    }
}