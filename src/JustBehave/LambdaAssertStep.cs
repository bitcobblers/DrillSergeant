using System;

namespace JustBehave
{
    public class LambdaAssertStep<TContext, TInput, TResult> : AssertStep<TContext, TInput, TResult>
    {
        public delegate void ExecuteMethod(TContext context, TInput input, TResult result);

        private string? name;
        private ExecuteMethod? execute;
        private Action? teardownMethod;

        public override string Name => this.name ?? this.execute?.GetType().FullName ?? nameof(LambdaArrangeStep<TContext, TInput>);

        public LambdaAssertStep<TContext, TInput, TResult> Named(string name)
        {
            this.name = name?.Trim();
            return this;
        }

        public LambdaAssertStep<TContext, TInput, TResult> Handle(ExecuteMethod? execute)
        {
            this.execute = execute ?? new ExecuteMethod((_, _, _) => { });
            return this;
        }

        public LambdaAssertStep<TContext, TInput, TResult> Teardown(Action teardown)
        {
            this.teardownMethod = teardown ?? new Action(() => { });
            return this;
        }

        public override void Assert(TContext context, TInput input, TResult result) => this.execute?.Invoke(context, input, result);

        protected override void Teardown() => this.teardownMethod?.Invoke();
    }
}