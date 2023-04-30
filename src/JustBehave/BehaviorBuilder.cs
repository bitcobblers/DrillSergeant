using System;
using System.Threading.Tasks;

namespace JustBehave;

public class BehaviorBuilder<TContext>
{
    public string Name { get; }

    public BehaviorBuilder(string name)
    {
        this.Name = name;
    }

    public BehaviorBuilder<TContext, TInput> WithInput<TInput>() => new(this.Name);
    public BehaviorBuilder<TContext, TInput> WithInput<TInput>(Func<TInput> map) => new(this.Name);
}

public class BehaviorBuilder<TContext, TInput> : BehaviorBuilder<TContext>
{
    public BehaviorBuilder(string name) : base(name) { }

    // Experimental APIs.
    //public BehaviorBuilderWithInput<TContext, TInput> AddDependency<TDep>(string name="") => this;
    //public BehaviorBuilderWithInput<TContext, TInput> Given<TArg1>(string name, Func<TContext, TInput, TArg1, TContext> step) => this;
    //public BehaviorBuilderWithInput<TContext, TInput> Given<TArg1, TArg2>(string name, Func<TContext, TInput, TArg1, TArg2, TContext> step) => this;
    //public BehaviorBuilderWithResult<TContext, TInput, TResult> When<TArg1, TResult>(string name, Func<TContext, TInput, TArg1, TResult> step) => new(this.Name);
    
    public BehaviorBuilder<TContext, TInput> Given(GivenStep<TContext, TInput> step) => this;
    public BehaviorBuilder<TContext, TInput> Given<TStep>() where TStep : GivenStep<TContext, TInput>, new() => this;

    public BehaviorBuilder<TContext, TInput> Given(Func<TContext, TInput, TContext> step) => this;
    public BehaviorBuilder<TContext, TInput> Given(string name, Func<TContext, TInput, TContext> step) => this;

    public BehaviorBuilder<TContext, TInput> Given(Func<TContext, TInput, Task<TContext>> step) => this;
    public BehaviorBuilder<TContext, TInput> Given(string name, Func<TContext, TInput, Task<TContext>> step) => this;

    public BehaviorResultBuilder<TContext, TInput, TResult> When<TResult>(WhenStep<TContext, TInput, TResult> step) => new(this.Name);
    public BehaviorResultBuilder<TContext, TInput, TResult> When<TStep, TResult>() where TStep : WhenStep<TContext, TInput, TResult>, new() => new(this.Name);

    public BehaviorResultBuilder<TContext, TInput, TResult> When<TResult>(Func<TContext, TInput, TResult> step) => new(this.Name);
    public BehaviorResultBuilder<TContext, TInput, TResult> When<TResult>(string name, Func<TContext, TInput, TResult> step) => new(this.Name);

    public BehaviorResultBuilder<TContext, TInput, TResult> When<TResult>(Func<TContext, TInput, Task<TResult>> step) => new(this.Name);
    public BehaviorResultBuilder<TContext, TInput, TResult> When<TResult>(string name, Func<TContext, TInput, Task<TResult>> step) => new(this.Name);
}

public class BehaviorResultBuilder<TContext, TInput, TResult>
{
    public string Name { get; }

    public BehaviorResultBuilder(string name)
    {
        Name = name;
    }

    public BehaviorResultBuilder<TContext, TInput, TResult> Then(ThenStep<TContext, TInput, TResult> step) => this;
    public BehaviorResultBuilder<TContext, TInput, TResult> Then<TStep>() where TStep : ThenStep<TContext, TInput, TResult>, new() => this;

    public BehaviorResultBuilder<TContext, TInput, TResult> Then(Action<TContext, TInput, TResult> step) => this;
    public BehaviorResultBuilder<TContext, TInput, TResult> Then(string name, Action<TContext, TInput, TResult> step) => this;

    public BehaviorResultBuilder<TContext, TInput, TResult> Then(Func<TContext, TInput, TResult, Task> step) => this;
    public BehaviorResultBuilder<TContext, TInput, TResult> Then(string name, Func<TContext, TInput, TResult, Task> step) => this;

    public Behavior<TContext> Build() => new();
}