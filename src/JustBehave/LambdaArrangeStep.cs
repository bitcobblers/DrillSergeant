using System;

namespace JustBehave
{

    public class LambdaArrangeStep<TContext, TInput> : ArrangeStep<TContext, TInput>, IDisposable
    {
        public delegate TContext ExecuteWithReturnMethod(TContext context, TInput input);
        public delegate void ExecuteNoReturnMethod(TContext context, TInput input);
        public delegate void TeardownMethod();

        private ExecuteWithReturnMethod executeWithReturnHandler = (context, __) => context;
        private ExecuteNoReturnMethod executeNoReturnHandler = (_, __) => { };
        private TeardownMethod teardownHandler = () => { };

        private bool isDisposed;

        public LambdaArrangeStep()
            : this(null)
        {
        }

        public LambdaArrangeStep(string? description)
            : base(description)
        {
        }

        ~LambdaArrangeStep()
        {

        }

        public LambdaArrangeStep<TContext, TInput> Handle(ExecuteWithReturnMethod execute)
        {
            this.executeWithReturnHandler = execute;
            return this;
        }

        public LambdaArrangeStep<TContext, TInput> Handle(ExecuteNoReturnMethod execute)
        {
            this.executeNoReturnHandler = execute;
            return this;
        }

        public LambdaArrangeStep<TContext, TInput> Teardown(TeardownMethod teardown)
        {
            this.teardownHandler = teardown;
            return this;
        }

        public override TContext Execute(TContext context, TInput input)
        {
            if (this.executeWithReturnHandler != null)
            {
                return this.executeWithReturnHandler(context, input);
            }

            this.executeNoReturnHandler?.Invoke(context, input);
            return context;
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !this.isDisposed)
            {
                this.teardownHandler?.Invoke();
            }

            this.isDisposed = true;
        }
    }
}