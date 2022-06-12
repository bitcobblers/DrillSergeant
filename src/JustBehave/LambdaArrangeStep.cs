using System;

namespace JustBehave
{
    public class LambdaArrangeStep<TContext, TInput> : ArrangeStep<TContext, TInput>
    {
        public delegate TContext ExecuteWithReturnMethod(TContext context, TInput input);
        public delegate void ExecuteNoReturnMethod(TContext context, TInput input);

        private string? name;
        private ExecuteWithReturnMethod? executeWithReturnHandler;
        private ExecuteNoReturnMethod? executeNoReturnHandler;
        private Action? teardownHandler;

        public override string Name => this.name ?? this.executeWithReturnHandler?.GetType().FullName ?? this.executeNoReturnHandler?.GetType().FullName ?? nameof(LambdaArrangeStep<TContext, TInput>);

        public LambdaArrangeStep<TContext, TInput> Named(string name)
        {
            this.name = name?.Trim();
            return this;
        }

        public LambdaArrangeStep<TContext, TInput> Handle(ExecuteNoReturnMethod? execute)
        {
            this.executeNoReturnHandler = execute ?? new ExecuteNoReturnMethod((_, _) => { });
            this.executeWithReturnHandler = null;
            return this;
        }

        public LambdaArrangeStep<TContext, TInput> Handle(ExecuteWithReturnMethod? execute)
        {
            this.executeNoReturnHandler = null;
            this.executeWithReturnHandler = execute ?? new ExecuteWithReturnMethod((c, _) => c);
            return this;
        }

        public LambdaArrangeStep<TContext, TInput> Teardown(Action teardown)
        {
            this.teardownHandler = teardown ?? new Action(() => { });
            return this;
        }

        public override TContext Arrange(TContext context, TInput input)
        {
            if (this.executeWithReturnHandler != null)
            {
                return this.executeWithReturnHandler(context, input);
            }

            this.executeNoReturnHandler?.Invoke(context, input);
            return context;
        }

        protected override void Teardown()
        {
            this.teardownHandler?.Invoke();
        }
    }
}