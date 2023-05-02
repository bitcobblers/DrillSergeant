using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JustBehave;

public class BehaviorBuilder<TContext, TInput>
{
    private readonly List<Step> _steps = new();

    // Experimental APIs.
    //public BehaviorBuilderWithInput<TContext, TInput> AddDependency<TDep>(string name="") => this;
    //public BehaviorBuilderWithInput<TContext, TInput> Given<TArg1>(string name, Func<TContext, TInput, TArg1, TContext> step) => this;
    //public BehaviorBuilderWithInput<TContext, TInput> Given<TArg1, TArg2>(string name, Func<TContext, TInput, TArg1, TArg2, TContext> step) => this;
    //public BehaviorBuilderWithResult<TContext, TInput, TResult> When<TArg1, TResult>(string name, Func<TContext, TInput, TArg1, TResult> step) => new(this.Name);

    public BehaviorBuilder<TContext, TInput> Given(GivenStep<TContext, TInput> step)
    {
        _steps.Add(step);
        return this;
    }

    public BehaviorBuilder<TContext, TInput> Given<TStep>() where TStep : GivenStep<TContext, TInput>, new()
    {
        _steps.Add(new TStep());
        return this;
    }

    public BehaviorBuilder<TContext, TInput> Given(Func<TContext, TInput, TContext> step)
    {
        _steps.Add(new LambdaGivenStep<TContext,TInput>().Handle(step));
        return this;
    }

    public BehaviorBuilder<TContext, TInput> Given(string name, Func<TContext, TInput, TContext> step)
    {
        _steps.Add(new LambdaGivenStep<TContext,TInput>().Named(name).Handle(step));
        return this;
    }

    public BehaviorBuilder<TContext, TInput> Given(Func<TContext, TInput, Task<TContext>> step) => throw new NotImplementedException();
    public BehaviorBuilder<TContext, TInput> Given(string name, Func<TContext, TInput, Task<TContext>> step) => throw new NotImplementedException();

    public BehaviorResultBuilder<TContext, TInput, TResult> When<TResult>(WhenStep<TContext, TInput, TResult> step)
    {
        _steps.Add(step);
        return new BehaviorResultBuilder<TContext, TInput, TResult>(_steps);
    }

    public BehaviorResultBuilder<TContext, TInput, TResult> When<TStep, TResult>() where TStep : WhenStep<TContext, TInput, TResult>, new() => throw new NotImplementedException();

    public BehaviorResultBuilder<TContext, TInput, TResult> When<TResult>(Func<TContext, TInput, TResult> step) => throw new NotImplementedException();
    public BehaviorResultBuilder<TContext, TInput, TResult> When<TResult>(string name, Func<TContext, TInput, TResult> step) => throw new NotImplementedException();

    public BehaviorResultBuilder<TContext, TInput, TResult> When<TResult>(Func<TContext, TInput, Task<TResult>> step) => throw new NotImplementedException();
    public BehaviorResultBuilder<TContext, TInput, TResult> When<TResult>(string name, Func<TContext, TInput, Task<TResult>> step) => throw new NotImplementedException();
}

public class BehaviorResultBuilder<TContext, TInput, TResult>
{
    private readonly List<Step> _steps = new();

    public BehaviorResultBuilder(IEnumerable<Step> steps) => _steps.AddRange(steps);

    public BehaviorResultBuilder<TContext, TInput, TResult> Then(ThenStep<TContext, TInput, TResult> step) => this;
    public BehaviorResultBuilder<TContext, TInput, TResult> Then<TStep>() where TStep : ThenStep<TContext, TInput, TResult>, new() => this;

    public BehaviorResultBuilder<TContext, TInput, TResult> Then(Action<TContext, TInput, TResult> step) => this;
    public BehaviorResultBuilder<TContext, TInput, TResult> Then(string name, Action<TContext, TInput, TResult> step) => this;

    public BehaviorResultBuilder<TContext, TInput, TResult> Then(Func<TContext, TInput, TResult, Task> step) => this;
    public BehaviorResultBuilder<TContext, TInput, TResult> Then(string name, Func<TContext, TInput, TResult, Task> step) => this;

    public Behavior Build() => new Behavior(_steps);
}