using System;
using System.Collections.Generic;

namespace JustBehave
{
    public class BehaviorBuilder<TContext, TInput>
    {
        public string Description { get; }

        public BehaviorBuilder(string description)
        {
            this.Description = description;
        }

        public BehaviorBuilder<TContext, TInput> WithInput(TInput input) => this;
        public BehaviorBuilder<TContext, TInput> WithInput(IEnumerable<TInput> collection) => this;

        public BehaviorBuilder<TContext, TInput> Arrange(ArrangeStep<TContext, TInput> step) => this;
        public BehaviorBuilder<TContext, TInput> Arrange(string description, Func<TContext, TInput, TContext> step) => this;

        public BehaviorBuilderWithIntermediate<TContext, TInput, TIntermediate> Act<TIntermediate>(ActStep<TContext, TInput, TIntermediate> step) => new(this.Description);
    }

    public class BehaviorBuilderWithIntermediate<TContext, TInput, TIntermediate> : BehaviorBuilder<TContext, TInput>
    {
        public BehaviorBuilderWithIntermediate(string description) : base(description) { }

        public BehaviorBuilderWithIntermediate<TContext, TInput, TIntermediate> Act<TIntermediate>(ActStep<TContext, TInput, TIntermediate> step) => new(this.Description);
        public BehaviorBuilderWithIntermediate<TContext, TInput, TIntermediate> Assert(AssertStep<TContext, TInput, TIntermediate> step) => this;

        public Behavior<TContext, TInput> Build() => new();
    }
}