using System;

namespace JustBehave
{
    public class LambdaThenStep<TContext, TInput, TResult> : ThenStep<TContext, TInput, TResult>
    {
        public delegate void ThenMethod(TContext context, TInput input, TResult result);

        private string? name;
        private ThenMethod? thenHandler;
        private Action? teardownHandler;

        public override string Name => this.name ?? this.thenHandler?.GetType().FullName ?? nameof(LambdaThenStep<TContext, TInput, TResult>);

        public LambdaThenStep<TContext, TInput, TResult> Named(string name)
        {
            this.name = name?.Trim();
            return this;
        }

        public LambdaThenStep<TContext, TInput, TResult> Handle(ThenMethod? execute)
        {
            this.thenHandler = execute ?? new ThenMethod((_, _, _) => { });
            return this;
        }

        public LambdaThenStep<TContext, TInput, TResult> Teardown(Action teardown)
        {
            this.teardownHandler = teardown ?? new Action(() => { });
            return this;
        }

        public override void Then(TContext context, TInput input, TResult result) => this.thenHandler?.Invoke(context, input, result);

        protected override void Teardown() => this.teardownHandler?.Invoke();
    }
}