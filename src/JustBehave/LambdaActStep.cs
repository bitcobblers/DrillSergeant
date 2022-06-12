using System;

namespace JustBehave
{

    public class LambdaActStep<TContext, TInput, TResult> : ActStep<TContext, TInput, TResult>
    {
        public delegate TResult ExecuteMethod(TContext context, TInput input);

        private string? name;
        private ExecuteMethod? execute;
        private Action? teardownHandler;

        public override string Name => this.name ?? nameof(LambdaActStep<TContext, TInput, TResult>);

        public LambdaActStep<TContext, TInput, TResult> Handle(ExecuteMethod? execute) => this.Handle(execute?.GetType().FullName, execute);

        public LambdaActStep<TContext, TInput, TResult> Handle(string? name, ExecuteMethod? execute)
        {
            this.execute = execute;
            this.name = string.IsNullOrWhiteSpace(name) ? execute?.GetType().FullName : name.Trim();
            return this;
        }

        public LambdaActStep<TContext, TInput, TResult> Teardown(Action teardown)
        {
            this.teardownHandler = teardown ?? new Action(() => { });
            return this;
        }

        public override TResult Act(TContext context, TInput input)
        {
            if (this.execute != null)
            {
                return this.execute(context, input);
            }

            return default!;
        }

        protected override void Teardown() => this.teardownHandler?.Invoke();
    }
}