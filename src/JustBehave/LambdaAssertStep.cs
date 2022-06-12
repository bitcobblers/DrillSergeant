using System;

namespace JustBehave
{
    public class LambdaAssertStep<TContext, TInput, TResult> : AssertStep<TContext, TInput, TResult>
    {
        public delegate void ExecuteMethod(TContext context, TInput input, TResult result);

        private string? name;
        private ExecuteMethod? execute;
        private Action? teardownMethod;

        public override string Name => this.name ?? nameof(LambdaArrangeStep<TContext, TInput>);

        public LambdaAssertStep<TContext, TInput, TResult> Handle(ExecuteMethod? execute) => this.Handle(execute?.GetType().FullName, execute);

        public LambdaAssertStep<TContext, TInput, TResult> Handle(string? name, ExecuteMethod? execute)
        {
            this.execute = execute ?? new ExecuteMethod((_, _, _) => { });
            this.name = string.IsNullOrWhiteSpace(name) ? execute?.GetType().FullName : name.Trim();
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