using System;

namespace JustBehave
{
    public class LambdaGivenStep<TContext, TInput> : GivenStep<TContext, TInput>
    {
        public delegate TContext GivenWithReturnMethod(TContext context, TInput input);
        public delegate void GivenNoReturnMethod(TContext context, TInput input);

        private string? name;
        private GivenWithReturnMethod? givenWithReturnHandler;
        private GivenNoReturnMethod? givenNoReturnHandler;
        private Action? teardownHandler;

        public override string Name => this.name ?? this.givenWithReturnHandler?.GetType().FullName ?? this.givenNoReturnHandler?.GetType().FullName ?? nameof(LambdaGivenStep<TContext, TInput>);

        public LambdaGivenStep<TContext, TInput> Named(string name)
        {
            this.name = name?.Trim();
            return this;
        }

        public LambdaGivenStep<TContext, TInput> Handle(GivenNoReturnMethod? execute)
        {
            this.givenNoReturnHandler = execute ?? new GivenNoReturnMethod((_, _) => { });
            this.givenWithReturnHandler = null;
            return this;
        }

        public LambdaGivenStep<TContext, TInput> Handle(GivenWithReturnMethod? execute)
        {
            this.givenNoReturnHandler = null;
            this.givenWithReturnHandler = execute ?? new GivenWithReturnMethod((c, _) => c);
            return this;
        }

        public LambdaGivenStep<TContext, TInput> Teardown(Action teardown)
        {
            this.teardownHandler = teardown ?? new Action(() => { });
            return this;
        }

        public override TContext Given(TContext context, TInput input)
        {
            if (this.givenWithReturnHandler != null)
            {
                return this.givenWithReturnHandler(context, input);
            }

            this.givenNoReturnHandler?.Invoke(context, input);
            return context;
        }

        protected override void Teardown()
        {
            this.teardownHandler?.Invoke();
        }
    }
}