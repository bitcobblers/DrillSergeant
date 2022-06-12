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

        public BehaviorBuilderWithInput<TContext, TInput> Arrange(ArrangeStep<TContext, TInput> step) => this;
        public BehaviorBuilderWithInput<TContext, TInput> Arrange(string description, Func<TContext, TInput, TContext> step) => this;
        public BehaviorBuilderWithInput<TContext, TInput> Arrange(Func<TContext, TInput, TContext> step) => this;

        public BehaviorBuilderWithResult<TContext, TInput, TIntermediate> Act<TIntermediate>(ActStep<TContext, TInput, TIntermediate> step) => new(this.Description);
        public BehaviorBuilderWithResult<TContext, TInput, TIntermediate> Act<TIntermediate>(Func<TContext, TInput, TIntermediate> step) => new(this.Description);
    }

    public class BehaviorBuilderWithResult<TContext, TInput, TIntermediate> : BehaviorBuilderWithInput<TContext, TInput>
    {
        public BehaviorBuilderWithResult(string description) : base(description) { }

        public BehaviorBuilderWithResult<TContext, TInput, TIntermediate> Assert(AssertStep<TContext, TInput, TIntermediate> step) => this;
        public BehaviorBuilderWithResult<TContext, TInput, TIntermediate> Assert(Action<TContext, TInput, TIntermediate> step) => this;

        public Behavior<TContext> Build() => new();
    }
}