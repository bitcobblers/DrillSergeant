using System;

namespace JustBehave
{

    public class LambdaWhenStep<TContext, TInput, TResult> : WhenStep<TContext, TInput, TResult>
    {
        public delegate TResult WhenMethod(TContext context, TInput input);

        private string? name;
        private WhenMethod? whenHandler;
        private Action? teardownHandler;

        public override string Name => this.name ?? this.whenHandler?.GetType().FullName ?? nameof(LambdaWhenStep<TContext, TInput, TResult>);

        public LambdaWhenStep<TContext, TInput, TResult> Named(string name)
        {
            this.name = name?.Trim();
            return this;
        }

        public LambdaWhenStep<TContext, TInput, TResult> Handle(WhenMethod? execute)
        {
            this.whenHandler = execute;
            return this;
        }

        public LambdaWhenStep<TContext, TInput, TResult> Teardown(Action teardown)
        {
            this.teardownHandler = teardown ?? new Action(() => { });
            return this;
        }

        public override TResult When(TContext context, TInput input)
        {
            if (this.whenHandler != null)
            {
                return this.whenHandler(context, input);
            }

            return default!;
        }

        protected override void Teardown() => this.teardownHandler?.Invoke();
    }
}