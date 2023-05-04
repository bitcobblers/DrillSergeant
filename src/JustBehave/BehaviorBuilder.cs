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

    // ---

    public BehaviorThenBuilder<TContext, TInput> When<TResult>(WhenStep<TContext, TInput> step)
    {
        _steps.Add(step);
        return new BehaviorThenBuilder<TContext, TInput>(_steps);
    }

    public BehaviorThenBuilder<TContext, TInput> When<TStep, TResult>() where TStep : WhenStep<TContext, TInput>, new() => throw new NotImplementedException();

    public BehaviorThenBuilder<TContext, TInput> When<TResult>(Func<TContext, TInput, TResult> step) => throw new NotImplementedException();
    public BehaviorThenBuilder<TContext, TInput> When(string name, Func<TContext, TInput, TContext> step)
    {
        _steps.Add(new LambdaWhenStep<TContext, TInput>().Named(name).Handle(step));
        return new BehaviorThenBuilder<TContext, TInput>(_steps);
    }

    public BehaviorThenBuilder<TContext, TInput> When<TResult>(Func<TContext, TInput, Task<TResult>> step) => throw new NotImplementedException();
    public BehaviorThenBuilder<TContext, TInput> When<TResult>(string name, Func<TContext, TInput, Task<TResult>> step) => throw new NotImplementedException();
}

public class BehaviorThenBuilder<TContext, TInput>
{
    private readonly List<Step> _steps = new();

    public BehaviorThenBuilder(IEnumerable<Step> steps) => _steps.AddRange(steps);

    public BehaviorThenBuilder<TContext, TInput> Then(ThenStep<TContext, TInput> step) => this;
    public BehaviorThenBuilder<TContext, TInput> Then<TStep>() where TStep : ThenStep<TContext, TInput>, new() => this;

    public BehaviorThenBuilder<TContext, TInput> Then(Action<TContext, TInput> step) => this;
    public BehaviorThenBuilder<TContext, TInput> Then(string name, Action<TContext, TInput> step) => this;

    public BehaviorThenBuilder<TContext, TInput> Then(Func<TContext, TInput, Task> step) => this;
    public BehaviorThenBuilder<TContext, TInput> Then(string name, Func<TContext, TInput, Task> step) => this;

    public Behavior Build() => new (_steps);
}