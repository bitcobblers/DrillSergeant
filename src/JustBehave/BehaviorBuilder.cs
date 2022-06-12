using System;
using System.Collections.Generic;

namespace JustBehave
{
    public class BehaviorBuilder<TContext>
    {
        public string Description { get; }

        public BehaviorBuilder(string description)
        {
            this.Description = description;
        }

        public BehaviorBuilderWithInput<TContext, TInput> WithInput<TInput>(TInput input) => new(this.Description);
        public BehaviorBuilderWithInput<TContext, TInput> WithInput<TInput>(IEnumerable<TInput> collection) => new(this.Description);
    }

    public class BehaviorBuilderWithInput<TContext, TInput> : BehaviorBuilder<TContext>
    {
        public BehaviorBuilderWithInput(string description) : base(description)
        {
        }

        public BehaviorBuilderWithInput<TContext, TInput> Given(GivenStep<TContext, TInput> step) => this;
        public BehaviorBuilderWithInput<TContext, TInput> Given<TStep>() where TStep : GivenStep<TContext, TInput>, new() => this;
        public BehaviorBuilderWithInput<TContext, TInput> Given(string description, Func<TContext, TInput, TContext> step) => this;
        public BehaviorBuilderWithInput<TContext, TInput> Given(Func<TContext, TInput, TContext> step) => this;

        public BehaviorBuilderWithResult<TContext, TInput, TResult> When<TResult>(WhenStep<TContext, TInput, TResult> step) => new(this.Description);
        public BehaviorBuilderWithResult<TContext, TInput, TResult> When<TStep, TResult>() where TStep : WhenStep<TContext, TInput, TResult>, new() => new(this.Description);
        public BehaviorBuilderWithResult<TContext, TInput, TResult> When<TResult>(Func<TContext, TInput, TResult> step) => new(this.Description);
    }

    public class BehaviorBuilderWithResult<TContext, TInput, TResult> : BehaviorBuilderWithInput<TContext, TInput>
    {
        public BehaviorBuilderWithResult(string description) : base(description) { }

        public BehaviorBuilderWithResult<TContext, TInput, TResult> Then(ThenStep<TContext, TInput, TResult> step) => this;
        public BehaviorBuilderWithResult<TContext, TInput, TResult> Then<TStep>() where TStep : ThenStep<TContext, TInput, TResult>, new() => this;
        public BehaviorBuilderWithResult<TContext, TInput, TResult> Then(Action<TContext, TInput, TResult> step) => this;

        public Behavior<TContext> Build() => new();
    }
}