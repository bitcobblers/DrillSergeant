using System;
using System.Threading.Tasks;

namespace JustBehave
{
    public class LambdaGivenStep<TContext, TInput> : GivenStep<TContext, TInput>
    {
        public delegate TContext GivenWithReturnMethod(TContext context, TInput input);
        public delegate Task<TContext> GivenWithReturnAsyncMethod(TContext context, TInput input);

        public delegate void GivenNoReturnMethod(TContext context, TInput input);
        public delegate Task GivenNoReturnAsyncMethod(TContext context, TInput input);

        private string? name;
        private GivenWithReturnAsyncMethod? givenWithReturnHandler;
        private GivenNoReturnAsyncMethod? givenNoReturnHandler;
        private Action? teardownHandler;

        public override string Name => this.name ?? this.givenWithReturnHandler?.GetType().FullName ?? this.givenNoReturnHandler?.GetType().FullName ?? nameof(LambdaGivenStep<TContext, TInput>);

        public LambdaGivenStep<TContext, TInput> Named(string name)
        {
            this.name = name?.Trim();
            return this;
        }

        public LambdaGivenStep<TContext, TInput> Handle(GivenNoReturnMethod? execute)
        {
            if (execute != null)
            {
                this.givenNoReturnHandler = new GivenNoReturnAsyncMethod(async (c, i) =>
                {
                    execute(c, i);
                    await Task.CompletedTask;
                });

                this.givenWithReturnHandler = null;
            }

            return this;
        }

        public LambdaGivenStep<TContext, TInput> Handle(GivenNoReturnAsyncMethod? execute)
        {
            this.givenNoReturnHandler = execute ?? new GivenNoReturnAsyncMethod((_, _) => Task.CompletedTask);
            this.givenWithReturnHandler = null;
            return this;
        }

        public LambdaGivenStep<TContext, TInput> Handle(GivenWithReturnMethod? execute)
        {
            if (execute != null)
            {
                this.givenWithReturnHandler = new GivenWithReturnAsyncMethod((c, i) =>
                {
                    return Task.FromResult(execute(c, i));
                });

                this.givenNoReturnHandler = null;
            }

            return this;
        }

        public LambdaGivenStep<TContext, TInput> Handle(GivenWithReturnAsyncMethod? execute)
        {
            this.givenNoReturnHandler = null;
            this.givenWithReturnHandler = execute ?? new GivenWithReturnAsyncMethod((c, _) =>
            {
                return Task.FromResult(c);
            });

            return this;
        }

        public LambdaGivenStep<TContext, TInput> Teardown(Action teardown)
        {
            this.teardownHandler = teardown ?? new Action(() => { });
            return this;
        }

        public override async Task<TContext> GivenAsync(TContext context, TInput input)
        {
            if (this.givenWithReturnHandler != null)
            {
                return await this.givenWithReturnHandler(context, input);
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