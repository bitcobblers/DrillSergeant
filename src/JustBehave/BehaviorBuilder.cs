using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JustBehave
{
    public class BehaviorBuilder<TContext>
    {
        public string Name { get; }

        public BehaviorBuilder(string name)
        {
            this.Name = name;
        }

        public BehaviorBuilderWithInput<TContext, TInput> WithInput<TInput>(TInput input) => new(this.Name);
        public BehaviorBuilderWithInput<TContext, TInput> WithInputs<TInput>(IEnumerable<TInput> collection) => new(this.Name);
    }

    public class BehaviorBuilderWithInput<TContext, TInput> : BehaviorBuilder<TContext>
    {
        public BehaviorBuilderWithInput(string name) : base(name) { }

        // Experimental APIs.
        //public BehaviorBuilderWithInput<TContext, TInput> AddDependency<TDep>(string name="") => this;
        //public BehaviorBuilderWithInput<TContext, TInput> Given<TArg1>(string name, Func<TContext, TInput, TArg1, TContext> step) => this;
        //public BehaviorBuilderWithInput<TContext, TInput> Given<TArg1, TArg2>(string name, Func<TContext, TInput, TArg1, TArg2, TContext> step) => this;
        //public BehaviorBuilderWithResult<TContext, TInput, TResult> When<TArg1, TResult>(string name, Func<TContext, TInput, TArg1, TResult> step) => new(this.Name);
        
        public BehaviorBuilderWithInput<TContext, TInput> Given(GivenStep<TContext, TInput> step) => this;
        public BehaviorBuilderWithInput<TContext, TInput> Given<TStep>() where TStep : GivenStep<TContext, TInput>, new() => this;

        public BehaviorBuilderWithInput<TContext, TInput> Given(Func<TContext, TInput, TContext> step) => this;
        public BehaviorBuilderWithInput<TContext, TInput> Given(string name, Func<TContext, TInput, TContext> step) => this;

        public BehaviorBuilderWithInput<TContext, TInput> Given(Func<TContext, TInput, Task<TContext>> step) => this;
        public BehaviorBuilderWithInput<TContext, TInput> Given(string name, Func<TContext, TInput, Task<TContext>> step) => this;

        public BehaviorBuilderWithResult<TContext, TInput, TResult> When<TResult>(WhenStep<TContext, TInput, TResult> step) => new(this.Name);
        public BehaviorBuilderWithResult<TContext, TInput, TResult> When<TStep, TResult>() where TStep : WhenStep<TContext, TInput, TResult>, new() => new(this.Name);

        public BehaviorBuilderWithResult<TContext, TInput, TResult> When<TResult>(Func<TContext, TInput, TResult> step) => new(this.Name);
        public BehaviorBuilderWithResult<TContext, TInput, TResult> When<TResult>(string name, Func<TContext, TInput, TResult> step) => new(this.Name);

        public BehaviorBuilderWithResult<TContext, TInput, TResult> When<TResult>(Func<TContext, TInput, Task<TResult>> step) => new(this.Name);
        public BehaviorBuilderWithResult<TContext, TInput, TResult> When<TResult>(string name, Func<TContext, TInput, Task<TResult>> step) => new(this.Name);
    }

    public class BehaviorBuilderWithResult<TContext, TInput, TResult> : BehaviorBuilderWithInput<TContext, TInput>
    {
        public BehaviorBuilderWithResult(string name) : base(name) { }

        public BehaviorBuilderWithResult<TContext, TInput, TResult> Then(ThenStep<TContext, TInput, TResult> step) => this;
        public BehaviorBuilderWithResult<TContext, TInput, TResult> Then<TStep>() where TStep : ThenStep<TContext, TInput, TResult>, new() => this;

        public BehaviorBuilderWithResult<TContext, TInput, TResult> Then(Action<TContext, TInput, TResult> step) => this;
        public BehaviorBuilderWithResult<TContext, TInput, TResult> Then(string name, Action<TContext, TInput, TResult> step) => this;

        public BehaviorBuilderWithResult<TContext, TInput, TResult> Then(Func<TContext, TInput, TResult, Task> step) => this;
        public BehaviorBuilderWithResult<TContext, TInput, TResult> Then(string name, Func<TContext, TInput, TResult, Task> step) => this;

        public Behavior<TContext> Build() => new();
    }
}